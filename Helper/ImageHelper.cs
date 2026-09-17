using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace UGB.MVC.Aplicaciones.Seguras.Helper
{
    public static class ImageHelper
    {
        public const long MaxSizeBytes = 2 * 1024 * 1024;

        public static bool IsSupportedImage(byte[] bytes, out string extension)
        {
            if(bytes.Length >= 3 &&
               bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            {
                extension = ".jpg";
                return true;
            }

            if(bytes.Length >= 8 &&
               bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
               bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A)
            {
                extension = ".png";
                return true;
            }

            extension = string.Empty;
            return false;
        }

        public static bool HasAllowedExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension is ".jpg" or ".jpeg" or ".png";
        }

        public static string ComputeSha256(byte[] data)
        {
            byte[] hash = SHA256.HashData(data);
            return Convert.ToHexStringLower(hash);
        }

        public static string SanitizeFileName(string originalName, string extension)
        {
            string nameOnly = Path.GetFileNameWithoutExtension(originalName);
            string sanitized = Regex.Replace(nameOnly, @"[^a-zA-Z0-9_-]", "_");
            sanitized = Regex.Replace(sanitized, "_+", "_").Trim('_');

            if(string.IsNullOrEmpty(sanitized))
            {
                sanitized = "imagen";
            }

            const int maxBaseLength = 40;
            if(sanitized.Length > maxBaseLength)
            {
                sanitized = sanitized[..maxBaseLength];
            }

            return sanitized + extension;
        }
    }
}
