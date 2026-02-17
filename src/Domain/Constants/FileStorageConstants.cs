using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Constants;
public class FileStorageConstants
{
    public static class MaxSize
    {
        public const long Image = 5 * 1024 * 1024;  // 5MB
        public const long Document = 10 * 1024 * 1024;  // 10MB
        public const long Video = 50 * 1024 * 1024;  // 50MB
    }

    // ===== Allowed Extensions =====
    public static class Extensions
    {
        public static readonly string[] Images =
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        public static readonly string[] Documents =
        {
            ".pdf", ".doc", ".docx"
        };

        public static readonly string[] ImagesAndDocuments =
        {
            ".jpg", ".jpeg", ".png", ".webp", ".pdf"
        };
    }

    // ===== Allowed MIME Types =====
    public static class MimeTypes
    {
        public static readonly Dictionary<string, string[]> Images = new()
        {
            { ".jpg",  new[] { "image/jpeg" } },
            { ".jpeg", new[] { "image/jpeg" } },
            { ".png",  new[] { "image/png"  } },
            { ".webp", new[] { "image/webp" } }
        };

        public static readonly Dictionary<string, string[]> Documents = new()
        {
            { ".pdf",  new[] { "application/pdf" } },
            { ".doc",  new[] { "application/msword" } },
            { ".docx", new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" } }
        };

        public static readonly Dictionary<string, string[]> ImagesAndDocuments = new()
        {
            { ".jpg",  new[] { "image/jpeg" } },
            { ".jpeg", new[] { "image/jpeg" } },
            { ".png",  new[] { "image/png"  } },
            { ".webp", new[] { "image/webp" } },
            { ".pdf",  new[] { "application/pdf" } }
        };
    }

    // ===== Module Folders =====
    public static class Modules
    {
        public const string SmartDHA = "uploads/smartdha";
    }

}
