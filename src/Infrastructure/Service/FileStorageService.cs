using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Service;

public class FileStorageService : IFileStorageService
{
    private static readonly string[] DefaultAllowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp", ".mp3", ".aac", ".pdf" };
    private readonly IWebHostEnvironment _env;   // <-- add this
    private readonly FileStorageOptions _opt;
    private readonly FileExtensionContentTypeProvider _mime = new();


    public FileStorageService(IWebHostEnvironment env, IOptionsSnapshot<FileStorageOptions> options) // <-- inject here
    {
        _env = env;
        _opt = options.Value;
    }

    // Simple overload → validated overload with defaults
    public Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken ct)
        => SaveFileAsync(file, folderName, ct, 10 * 1024 * 1024, DefaultAllowedExt);
    public async Task<string> SaveFileNonMemeberAsync(
     IFormFile file,
     string folderName,
     CancellationToken ct,
     long maxBytes = 10 * 1024 * 1024,           // default 10 MB (adjust as you like)
     string[]? allowedExtensions = null)         // e.g. new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" }
    {
        // Default extensions (keep your existing list)
        var extensions = allowedExtensions ?? DefaultAllowedExt;

        if (extensions == null || extensions.Length == 0)
            throw new InvalidOperationException("No allowed extensions configured.");

        // Build MIME map dynamically for images
        var allowedMimeTypes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var ext in extensions)
        {
            if (_mime.TryGetContentType("file" + ext, out var mime))
            {
                allowedMimeTypes[ext.ToLowerInvariant()] = new[] { mime };
            }
        }

        // IMPORTANT:
        // moduleFolder → "uploads"
        // subFolder → folderName
        // because SaveFileInternalAsync handles root path itself

        var relativeUrl = await SaveFileInternalAsync(
            file,
            moduleFolder: "uploads",
            subFolder: folderName,
            ct: ct,
            maxBytes: maxBytes,
            allowedExtensions: extensions,
            allowedMimeTypes: allowedMimeTypes
        );

        return relativeUrl;
    }

    public async Task<string> SaveFileAsync(
     IFormFile file,
     string folderName,
     CancellationToken ct,
     long maxBytes,
     string[]? allowedExtensions)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Empty file.");

        var extensions = allowedExtensions ?? DefaultAllowedExt;

        if (extensions == null || extensions.Length == 0)
            throw new InvalidOperationException("No allowed extensions configured.");

        // Build MIME dictionary for images only (same logic as before)
        var allowedMimeTypes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var ext in extensions)
        {
            if (_mime.TryGetContentType("file" + ext, out var mime))
            {
                if (!mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    continue;

                allowedMimeTypes[ext.ToLowerInvariant()] = new[] { mime };
            }
        }

        if (allowedMimeTypes.Count == 0)
            throw new InvalidOperationException("No valid image MIME types resolved.");

        // Delegate everything to centralized secure storage
        return await SaveFileInternalAsync(
            file,
            moduleFolder: _opt.RequestPath ?? "Uploads",
            subFolder: folderName,
            ct: ct,
            maxBytes: maxBytes,
            allowedExtensions: extensions,
            allowedMimeTypes: allowedMimeTypes
        );
    }

    public async Task<string> SaveAudioAsync(
    IFormFile file,
    string folderName,
    CancellationToken ct,
    long maxBytes = 10 * 1024 * 1024,
    string[]? allowedExtensions = null)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Empty audio file.");

        // Default allowed extensions
        var extensions = allowedExtensions ?? new[] { ".mp3", ".aac" };

        if (extensions.Length == 0)
            throw new InvalidOperationException("No allowed audio extensions configured.");

        // Build MIME mapping for allowed audio extensions
        var allowedMimeTypes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var ext in extensions)
        {
            if (_mime.TryGetContentType("file" + ext, out var mime))
            {
                if (!mime.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
                    continue;

                allowedMimeTypes[ext.ToLowerInvariant()] = new[] { mime };
            }
        }

        if (allowedMimeTypes.Count == 0)
            throw new InvalidOperationException("No valid audio MIME types resolved.");

        // Delegate to centralized storage logic
        return await SaveFileInternalAsync(
            file,
            moduleFolder: _opt.RequestPath ?? "Uploads",   // keeps your previous behavior
            subFolder: folderName,
            ct: ct,
            maxBytes: maxBytes,
            allowedExtensions: extensions,
            allowedMimeTypes: allowedMimeTypes
        );
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

    // ✅ Save single complaint file
    public async Task<string> SaveComplaintFileAsync(
      IFormFile file,
      Guid complaintId,
      CancellationToken ct,
      long maxBytes = 10 * 1024 * 1024,
      string[]? allowedExtensions = null)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Empty file.");

        var extensions = allowedExtensions ?? new[] { ".jpg", ".jpeg", ".png", ".webp" };

        if (extensions.Length == 0)
            throw new InvalidOperationException("No allowed extensions configured.");

        // Build MIME dictionary (image only)
        var allowedMimeTypes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var ext in extensions)
        {
            if (_mime.TryGetContentType("file" + ext, out var mime))
            {
                if (!mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    continue;

                allowedMimeTypes[ext.ToLowerInvariant()] = new[] { mime };
            }
        }

        if (allowedMimeTypes.Count == 0)
            throw new InvalidOperationException("No valid image MIME types resolved.");

        // We want: uploads/complaints
        var moduleFolder = "complaints";
        var subFolder = "complaints";

        var relativeUrl = await SaveFileInternalAsync(
            file,
            moduleFolder: moduleFolder,
            subFolder: subFolder,
            ct: ct,
            maxBytes: maxBytes,
            allowedExtensions: extensions,
            allowedMimeTypes: allowedMimeTypes
        );

        return relativeUrl;
    }


    // ✅ Save multiple complaint files
    public async Task<List<string>> SaveComplaintFilesAsync(
     IEnumerable<IFormFile> files,
     Guid complaintId,
     CancellationToken ct,
     long maxBytes = 10 * 1024 * 1024,
     string[]? allowedExtensions = null)
    {
        if (files is null)
            throw new ArgumentException("No files provided.");

        var fileList = files as IList<IFormFile> ?? files.ToList();

        if (fileList.Count == 0)
            throw new ArgumentException("No files provided.");

        var results = new List<string>(fileList.Count);

        foreach (var file in fileList)
        {
            ct.ThrowIfCancellationRequested();

            var path = await SaveComplaintFileAsync(
                file,
                complaintId,
                ct,
                maxBytes,
                allowedExtensions);

            results.Add(path);
        }

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

    public string GetPublicUrlOfComplaint(string relativePath, string? baseUrl = null)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return string.Empty;

        // Normalize slashes
        var urlPath = relativePath.Replace('\\', '/');
        if (!urlPath.StartsWith("/"))
            urlPath = "/" + urlPath;

        // Resolve base URL (from parameter or app settings)
        var host = baseUrl ?? _opt.PublicBaseUrl;

        // Get physical wwwroot path
        var webRoot = _env.WebRootPath ?? throw new InvalidOperationException("WebRootPath is not configured.");

        // Build absolute physical path (wwwroot + relative path)
        var absPath = Path.Combine(webRoot, relativePath.TrimStart('/', '\\'))
            .Replace('\\', '/');

        // Combine with baseUrl (for external/public access)
        var fullUrl = string.IsNullOrWhiteSpace(host)
            ? absPath
            : $"{host.TrimEnd('/')}/wwwroot{urlPath}";

        return fullUrl;
    }
    public async Task<(string Path, PanicDispatchMediaType MediaType)> SaveImageOrVideoAsync(
      IFormFile file,
      string folderName,
      CancellationToken ct,
      long maxImageBytes = 10 * 1024 * 1024,
      long maxVideoBytes = 50 * 1024 * 1024)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Empty file.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        var imageExts = new[] { ".jpg", ".jpeg", ".png" };
        var videoExts = new[] { ".mp4" };

        PanicDispatchMediaType mediaType;
        long maxBytes;
        string[] allowedExtensions;

        if (imageExts.Contains(ext))
        {
            mediaType = PanicDispatchMediaType.Image;
            maxBytes = maxImageBytes;
            allowedExtensions = imageExts;
        }
        else if (videoExts.Contains(ext))
        {
            mediaType = PanicDispatchMediaType.Video;
            maxBytes = maxVideoBytes;
            allowedExtensions = videoExts;
        }
        else
        {
            throw new InvalidOperationException($"Extension '{ext}' not allowed.");
        }

        // Build MIME dictionary based on type
        var allowedMimeTypes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var extension in allowedExtensions)
        {
            if (_mime.TryGetContentType("file" + extension, out var mime))
            {
                if (mediaType == PanicDispatchMediaType.Image &&
                    mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    allowedMimeTypes[extension] = new[] { mime };
                }

                if (mediaType == PanicDispatchMediaType.Video &&
                    mime.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                {
                    allowedMimeTypes[extension] = new[] { mime };
                }
            }
        }

        if (allowedMimeTypes.Count == 0)
            throw new InvalidOperationException("No valid MIME types resolved.");

        // Delegate to centralized storage
        var relativeUrl = await SaveFileInternalAsync(
            file,
            moduleFolder: _opt.RequestPath ?? "Uploads",
            subFolder: folderName,
            ct: ct,
            maxBytes: maxBytes,
            allowedExtensions: allowedExtensions,
            allowedMimeTypes: allowedMimeTypes
        );

        return (relativeUrl, mediaType);
    }

    public async Task<string> SaveFileMemeberrequestAsync(
      IFormFile file,
      string folderName,
      CancellationToken ct,
      long maxBytes = 10 * 1024 * 1024,
      string[]? allowedExtensions = null)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Empty file.");

        var extensions = allowedExtensions ?? DefaultAllowedExt;

        if (extensions == null || extensions.Length == 0)
            throw new InvalidOperationException("No allowed extensions configured.");

        // Build MIME dictionary (image only)
        var allowedMimeTypes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var ext in extensions)
        {
            if (_mime.TryGetContentType("file" + ext, out var mime))
            {
                if (!mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    continue;

                allowedMimeTypes[ext.ToLowerInvariant()] = new[] { mime };
            }
        }

        if (allowedMimeTypes.Count == 0)
            throw new InvalidOperationException("No valid image MIME types resolved.");

        // Delegate to centralized storage
        return await SaveFileInternalAsync(
            file,
            moduleFolder: "membership",
            subFolder: folderName,
            ct: ct,
            maxBytes: maxBytes,
            allowedExtensions: extensions,
            allowedMimeTypes: allowedMimeTypes
        );
    }


    public async Task<(string Path, FMType MediaType)> FemugationSaveImageOrVideoAsync(
     IFormFile file,
     string folderName,
     CancellationToken ct,
     long maxImageBytes = 10 * 1024 * 1024,
     long maxVideoBytes = 50 * 1024 * 1024)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Empty file.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        var imageExts = new[] { ".jpg", ".jpeg", ".png" };
        var videoExts = new[] { ".mp4" };

        FMType mediaType;
        long maxBytes;
        string[] allowedExtensions;

        if (imageExts.Contains(ext))
        {
            mediaType = FMType.Image;
            maxBytes = maxImageBytes;
            allowedExtensions = imageExts;
        }
        else if (videoExts.Contains(ext))
        {
            mediaType = FMType.Video;
            maxBytes = maxVideoBytes;
            allowedExtensions = videoExts;
        }
        else
        {
            throw new InvalidOperationException($"Extension '{ext}' not allowed.");
        }

        // Build MIME dictionary dynamically
        var allowedMimeTypes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var extension in allowedExtensions)
        {
            if (_mime.TryGetContentType("file" + extension, out var mime))
            {
                if (mediaType == FMType.Image &&
                    mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    allowedMimeTypes[extension] = new[] { mime };
                }

                if (mediaType == FMType.Video &&
                    mime.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                {
                    allowedMimeTypes[extension] = new[] { mime };
                }
            }
        }

        if (allowedMimeTypes.Count == 0)
            throw new InvalidOperationException("No valid MIME types resolved.");

        var relativeUrl = await SaveFileInternalAsync(
            file,
            moduleFolder: "fumigation",
            subFolder: folderName,
            ct: ct,
            maxBytes: maxBytes,
            allowedExtensions: allowedExtensions,
            allowedMimeTypes: allowedMimeTypes
        );

        return (relativeUrl, mediaType);
    }

    public async Task<(string Path, FMType MediaType)> HomeCareSaveImageOrVideoAsync(
    IFormFile file,
    string folderName,
    CancellationToken ct,
    long maxImageBytes = 10 * 1024 * 1024,
    long maxVideoBytes = 50 * 1024 * 1024)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Empty file.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        var imageExts = new[] { ".jpg", ".jpeg", ".png" };
        var videoExts = new[] { ".mp4" };

        FMType mediaType;
        long maxBytes;
        string[] allowedExtensions;

        if (imageExts.Contains(ext))
        {
            mediaType = FMType.Image;
            maxBytes = maxImageBytes;
            allowedExtensions = imageExts;
        }
        else if (videoExts.Contains(ext))
        {
            mediaType = FMType.Video;
            maxBytes = maxVideoBytes;
            allowedExtensions = videoExts;
        }
        else
        {
            throw new InvalidOperationException($"Extension '{ext}' not allowed.");
        }

        // Build MIME dictionary dynamically
        var allowedMimeTypes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var extension in allowedExtensions)
        {
            if (_mime.TryGetContentType("file" + extension, out var mime))
            {
                if (mediaType == FMType.Image &&
                    mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    allowedMimeTypes[extension] = new[] { mime };
                }

                if (mediaType == FMType.Video &&
                    mime.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                {
                    allowedMimeTypes[extension] = new[] { mime };
                }
            }
        }

        if (allowedMimeTypes.Count == 0)
            throw new InvalidOperationException("No valid MIME types resolved.");

        var relativeUrl = await SaveFileInternalAsync(
            file,
            moduleFolder: _opt.RequestPath ?? "Uploads",
            subFolder: folderName,
            ct: ct,
            maxBytes: maxBytes,
            allowedExtensions: allowedExtensions,
            allowedMimeTypes: allowedMimeTypes
        );

        return (relativeUrl, mediaType);
    }


    public Task<string> SavePMSDocumentAsync(
    IFormFile file,
    string folderName,
    CancellationToken ct)
    {
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

        var mimeMap = new Dictionary<string, string[]>
        {
            [".jpg"] = new[] { "image/jpeg" },
            [".jpeg"] = new[] { "image/jpeg" },
            [".png"] = new[] { "image/png" },
            [".pdf"] = new[] { "application/pdf" }
        };

        return SaveFileInternalAsync(
            file,
            moduleFolder: "pms",          // module fixed here
            subFolder: folderName,        // only subfolder from caller
            ct: ct,
            maxBytes: 10 * 1024 * 1024,   // fixed limit
            allowedExtensions: allowed,
            allowedMimeTypes: mimeMap);
    }



    // Centralized secure file saving logic with all validations
    public async Task<string> SaveFileInternalAsync(
     IFormFile file,
     string moduleFolder,
     string? subFolder,
     CancellationToken ct,
     long maxBytes,
     string[] allowedExtensions,
     Dictionary<string, string[]> allowedMimeTypes)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Empty file.");

        if (file.Length > maxBytes)
            throw new InvalidOperationException(
                $"File exceeds {maxBytes / (1024 * 1024)} MB limit.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(ext))
            throw new InvalidOperationException($"Extension '{ext}' not allowed.");

        if (_mime.TryGetContentType(file.FileName, out var mappedType))
        {
            if (!allowedMimeTypes.TryGetValue(ext, out var validMimes) ||
                !validMimes.Contains(mappedType, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Invalid file type. Detected MIME: {mappedType}");
            }
        }

        if (string.IsNullOrWhiteSpace(_opt.RootPath))
            throw new InvalidOperationException("Storage RootPath not configured.");

        // ========================
        // PHYSICAL STORAGE
        // ========================

        var safeModule = moduleFolder.Trim().ToLowerInvariant();

        var basePhysical = Path.Combine(_opt.RootPath, safeModule);
        Directory.CreateDirectory(basePhysical);

        // Sanitize subfolder
        var safeSubFolder = (subFolder ?? string.Empty)
            .Trim()
            .TrimStart('/', '\\');

        if (safeSubFolder.Contains("..", StringComparison.Ordinal))
            throw new InvalidOperationException("Invalid folder name.");

        string absFolder = basePhysical;

        if (!string.IsNullOrWhiteSpace(safeSubFolder))
        {
            var subParts = safeSubFolder
                .Split('/', '\\', StringSplitOptions.RemoveEmptyEntries);

            absFolder = Path.Combine(basePhysical, Path.Combine(subParts));
        }

        Directory.CreateDirectory(absFolder);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var absPath = Path.Combine(absFolder, fileName);

        await using (var stream = new FileStream(
            absPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            64 * 1024,
            useAsync: true))
        {
            await file.CopyToAsync(stream, ct);
        }

        // ========================
        // RELATIVE URL (STORE IN DB)
        // ========================

        var urlSegments = new List<string>
    {
        safeModule
    };

        if (!string.IsNullOrWhiteSpace(safeSubFolder))
        {
            urlSegments.AddRange(
                safeSubFolder
                    .Split('/', '\\', StringSplitOptions.RemoveEmptyEntries));
        }

        urlSegments.Add(fileName);

        var relativeUrl = "/" + string.Join("/", urlSegments);

        return relativeUrl;
    }
    public string GetPublicUrl(string relativePath, string? baseUrl = null)
    {

        Console.WriteLine(" called with relativePath: " + relativePath + " and baseUrl: " + baseUrl);
        if (string.IsNullOrWhiteSpace(relativePath))
            return string.Empty;

        //if (Uri.TryCreate(relativePath, UriKind.Absolute, out _))
        //    return relativePath;

        var urlPath = relativePath.Replace('\\', '/').Trim();

        if (!urlPath.StartsWith("/"))
            urlPath = "/" + urlPath;

        var host = !string.IsNullOrWhiteSpace(baseUrl)
    ? baseUrl
    : _opt.PublicBaseUrl;

        Console.WriteLine("Configured PublicBaseUrl: " + _opt.PublicBaseUrl);


        Console.WriteLine("Host url:" + host);
        if (string.IsNullOrWhiteSpace(host))
            throw new InvalidOperationException(
                "PublicBaseUrl is not configured in FileStorage section.");

        return $"{host.TrimEnd('/')}{urlPath}";
    }
    public string GetPublicUrl(string relativePath)
    {

        Console.WriteLine(" called with relativePath: " + relativePath);
        if (string.IsNullOrWhiteSpace(relativePath))
            return string.Empty;

        //if (Uri.TryCreate(relativePath, UriKind.Absolute, out _))
        //    return relativePath;

        var urlPath = relativePath.Replace('\\', '/').Trim();

        if (!urlPath.StartsWith("/"))
            urlPath = "/" + urlPath;

        var host = _opt.PublicBaseUrl;

        Console.WriteLine("Configured PublicBaseUrl: " + _opt.PublicBaseUrl);


        Console.WriteLine("Host url:" + host);
        if (string.IsNullOrWhiteSpace(host))
            throw new InvalidOperationException(
                "PublicBaseUrl is not configured in FileStorage section.");

        //return $"{host.TrimEnd('/')}{urlPath}";
        return $"{host.TrimEnd('/')}{urlPath}";

    }
}
