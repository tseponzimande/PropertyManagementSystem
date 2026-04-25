namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder = "uploads");
        Task<bool> DeleteFileAsync(string fileUrl);
        Task<byte[]?> DownloadFileAsync(string fileUrl);
        bool IsValidFileType(IFormFile file, string[] allowedExtensions);
        bool IsValidFileSize(IFormFile file, long maxSizeInBytes = 10485760);
        string GetFileExtension(IFormFile file);
        long GetFileSize(IFormFile file);
    }
}
