namespace PropertyManagementSystem.Application.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _uploadPath;

        public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
            _uploadPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads");

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder = "uploads")
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty or null");

                var folderPath = Path.Combine(_uploadPath, folder);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativeUrl = $"/uploads/{folder}/{uniqueFileName}";
                _logger.LogInformation("File uploaded successfully: {FileName}", uniqueFileName);

                return relativeUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file: {FileName}", file?.FileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileUrl))
                    return false;

                var fileName = fileUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                var filePath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, fileName);

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                    _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                    return true;
                }

                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file: {FileUrl}", fileUrl);
                return false;
            }
        }

        public async Task<byte[]?> DownloadFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileUrl))
                    return null;

                var fileName = fileUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                var filePath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, fileName);

                if (File.Exists(filePath))
                {
                    return await File.ReadAllBytesAsync(filePath);
                }

                _logger.LogWarning("File not found for download: {FilePath}", filePath);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download file: {FileUrl}", fileUrl);
                return null;
            }
        }

        public bool IsValidFileType(IFormFile file, string[] allowedExtensions)
        {
            try
            {
                if (file == null)
                    return false;

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return allowedExtensions.Contains(extension);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "failed to check Valid File Type");
                return false;
            }
        }

        public bool IsValidFileSize(IFormFile file, long maxSizeInBytes = 10485760)
        {
            try
            {
                if (file == null)
                    return false;

                return file.Length <= maxSizeInBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "failed to check Valid File Size");
                return false;
            }
        }

        public string GetFileExtension(IFormFile file)
        {
            try
            {
                if (file == null)
                    return string.Empty;

                return Path.GetExtension(file.FileName).ToLowerInvariant();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return string.Empty;
            }
        }

        public long GetFileSize(IFormFile file)
        {
            try
            {
                return file?.Length ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return 0;
            }
        }
    }
}