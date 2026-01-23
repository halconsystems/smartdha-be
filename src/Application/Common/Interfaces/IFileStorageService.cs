using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IFileStorageService
{
    Task<string> SaveFileNonMemeberAsync(IFormFile file, string folderName, CancellationToken ct, long maxBytes = 10 * 1024 * 1024, string[]? allowedExtensions = null);
    Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken ct);
    Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken ct, long maxBytes, string[]? allowedExtensions);
    Task<string> SaveAudioAsync(IFormFile file, string folderName, CancellationToken ct, long maxBytes, string[]? allowedExtensions);
    Task<string> SavePMSDocumentAsync(IFormFile file, string folderName, CancellationToken ct, long maxBytes, string[]? allowedExtensions);
    Task<List<string>> SaveFilesAsync(IEnumerable<IFormFile> files, string folderName, CancellationToken ct, long maxBytes = 10 * 1024 * 1024, string[]? allowedExtensions = null);
    // Save single complaint image
    Task<string> SaveComplaintFileAsync(
        IFormFile file,
        Guid complaintId,
        CancellationToken ct,
        long maxBytes = 10 * 1024 * 1024,
        string[]? allowedExtensions = null);

    // Save multiple complaint images
    Task<List<string>> SaveComplaintFilesAsync(
        IEnumerable<IFormFile> files,
        Guid complaintId,
        CancellationToken ct,
        long maxBytes = 10 * 1024 * 1024,
        string[]? allowedExtensions = null);
    Task<bool> DeleteFileAsync(string relativePath, CancellationToken ct);
    string GetPublicUrl(string relativePath, string? baseUrl = null);
    string GetPublicUrlOfComplaint(string relativePath, string? baseUrl = null);

    Task<(string Path, PanicDispatchMediaType MediaType)> SaveImageOrVideoAsync(
     IFormFile file,
     string folderName,
     CancellationToken ct,
     long maxImageBytes = 10 * 1024 * 1024,
     long maxVideoBytes = 50 * 1024 * 1024
 );

    Task<string> SaveFileMemeberrequestAsync(IFormFile file, string folderName, CancellationToken ct, long maxBytes = 10 * 1024 * 1024, string[]? allowedExtensions = null);

    Task<(string Path, FMType MediaType)> FemugationSaveImageOrVideoAsync(
     IFormFile file,
     string folderName,
     CancellationToken ct,
     long maxImageBytes = 10 * 1024 * 1024,
     long maxVideoBytes = 50 * 1024 * 1024
 );

}



public enum MediaKind
{
    Image = 1,
    Video = 2
}
