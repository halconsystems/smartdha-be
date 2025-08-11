using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Service;

public class FileStorageService : IFileStorageService
{
    private static readonly string[] DefaultAllowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly IWebHostEnvironment _env;   // <-- add this
    private readonly FileStorageOptions _opt;
    private readonly FileExtensionContentTypeProvider _mime = new();

    public FileStorageService(IWebHostEnvironment env, IOptions<FileStorageOptions> opt) // <-- inject here
    {
        _env = env;
        _opt = opt.Value;
    }

    // Simple overload → validated overload with defaults
    public Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken ct)
        => SaveFileAsync(file, folderName, ct, 10 * 1024 * 1024, DefaultAllowedExt);

    public async Task<string> SaveFileAsync(
    IFormFile file,
    string folderName,
    CancellationToken ct,
    long maxBytes,
    string[]? allowedExtensions)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Empty file.");
        if (file.Length > maxBytes)
            throw new InvalidOperationException($"File exceeds {maxBytes / (1024 * 1024)} MB limit.");

        var ext = Path.GetExtension(file.FileName);
        var allowed = allowedExtensions ?? DefaultAllowedExt;
        if (!allowed.Contains(ext, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Extension '{ext}' not allowed.");

        if (_mime.TryGetContentType(file.FileName, out var mappedType))
        {
            if (!mappedType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Only image uploads are allowed. Detected: {mappedType}");
        }

        // Physical base = <ContentRoot>\{RequestPathTrimmed}  e.g., C:\...\YourApp\CBMS
        var baseDir = _env.ContentRootPath;
        if (string.IsNullOrWhiteSpace(baseDir))
            baseDir = AppContext.BaseDirectory;

        var requestFolder = string.IsNullOrWhiteSpace(_opt.RequestPath)
            ? "CBMS"
            : _opt.RequestPath.Trim('/', '\\'); // "CBMS"

        var basePhysical = Path.Combine(baseDir, requestFolder);
        Directory.CreateDirectory(basePhysical);

        // Subfolder under CBMS (e.g., rooms/{roomId})
        var relFolder = (folderName ?? string.Empty).Trim().TrimStart('/', '\\');
        var absFolder = string.IsNullOrEmpty(relFolder) ? basePhysical : Path.Combine(basePhysical, relFolder);
        Directory.CreateDirectory(absFolder);

        // Save file
        var fileName = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
        var absPath = Path.Combine(absFolder, fileName);

        await using (var stream = new FileStream(absPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 64 * 1024, useAsync: true))
            await file.CopyToAsync(stream, ct);

        // Public relative URL under /CBMS
        var relUrl = $"/{requestFolder}/{(string.IsNullOrEmpty(relFolder) ? "" : relFolder.Replace('\\', '/') + "/")}{fileName}";
        // collapse any accidental double slashes
        relUrl = relUrl.Replace("//", "/");
        return relUrl;
    }
    public async Task<List<string>> SaveFilesAsync(
        IEnumerable<IFormFile> files,
        string folderName,
        CancellationToken ct,
        long maxBytes = 10 * 1024 * 1024,
        string[]? allowedExtensions = null)
    {
        var results = new List<string>();
        foreach (var f in files)
            results.Add(await SaveFileAsync(f, folderName, ct, maxBytes, allowedExtensions));
        return results;
    }
    public async Task<bool> DeleteFileAsync(string relativePath, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return false;

        // Accept with/without RequestPath prefix
        var path = relativePath.Replace('\\', '/').Trim();
        if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            // strip base url if someone passed absolute
            var baseUrl = (_opt.PublicBaseUrl ?? string.Empty).TrimEnd('/');
            if (!string.IsNullOrEmpty(baseUrl) && path.StartsWith(baseUrl, StringComparison.OrdinalIgnoreCase))
                path = path.Substring(baseUrl.Length);
        }
        if (path.StartsWith(_opt.RequestPath, StringComparison.OrdinalIgnoreCase))
            path = path.Substring(_opt.RequestPath.Length);

        path = path.TrimStart('/');

        var full = Path.Combine(_opt.RootPath, path.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(full)) return false;

        ct.ThrowIfCancellationRequested();
        try { File.Delete(full); await Task.CompletedTask; return true; }
        catch { return false; }
    }
    public string GetPublicUrl(string relativePath, string? baseUrl = null)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return string.Empty;
        var urlPath = relativePath.Replace('\\', '/');
        if (!urlPath.StartsWith("/")) urlPath = "/" + urlPath;

        var host = baseUrl ?? _opt.PublicBaseUrl;
        return string.IsNullOrWhiteSpace(host) ? urlPath : $"{host.TrimEnd('/')}{urlPath}";
    }

    private string GetCbmsPhysicalBase()
    {
        // Prefer ContentRootPath; fallback to AppContext.BaseDirectory
        var baseDir = _env.ContentRootPath;
        if (string.IsNullOrWhiteSpace(baseDir))
            baseDir = AppContext.BaseDirectory;

        var requestFolder = _opt.RequestPath.Trim('/', '\\'); // "CBMS"
        return Path.Combine(baseDir, requestFolder);          // <content-root>\CBMS
    }
}
