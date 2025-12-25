using Base.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class UploadController : BaseAPIController
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IConfiguration _configuration;

        public UploadController(ILogger<UploadController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [RequestSizeLimit(100 * 1024 * 1024)]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return SaveError("File không hợp lệ hoặc trống.");
                }

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var baseUrl = _configuration["ServerURL"] ?? "http://localhost:3000";

                var fileUrl = $"{baseUrl}/uploads/{fileName}";

                _logger.LogInformation("Upload file thành công: {0}", fileUrl);

                var responseData = new InnerData
                {
                    downloadUrl = fileUrl,
                    fileName = fileName,
                    fileSizeMb = Math.Round(file.Length / (1024.0 * 1024.0), 2),
                    fileType = file.ContentType ?? "application/octet-stream",
                    uploadTime = DateTime.UtcNow
                };

                return SaveSuccess(responseData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi upload file");
                return SaveError(ex.Message);
            }
        }

        public class InnerResponse
        {
            public string message { get; set; }
            public bool success { get; set; }
            public int statusCode { get; set; }
            public InnerData data { get; set; }
        }

        public class InnerData
        {
            public string downloadUrl { get; set; }
            public string fileName { get; set; }
            public double fileSizeMb { get; set; }
            public string fileType { get; set; }
            public DateTime uploadTime { get; set; }
        }
    }
}
