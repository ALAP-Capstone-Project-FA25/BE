using ALAP.Entity.DTO.Response;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Base.Utils
{
    public class ApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();


        public static async Task<UploadResponse> UploadFileAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File không hợp lệ hoặc rỗng.");

                using var formData = new MultipartFormDataContent();

                using var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

                formData.Add(fileContent, "file", file.FileName);

                var uploadUrl = "https://cdn.webradarcs2.com/upload";
                var response = await _httpClient.PostAsync(uploadUrl, formData);

                var responseBody = await response.Content.ReadAsStringAsync();


                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Lỗi upload: " + responseBody);
                    throw new Exception($"Upload thất bại. StatusCode={response.StatusCode}");
                }

                var result = JsonSerializer.Deserialize<UploadResponse>(responseBody);

                if (result == null || !result.success)
                {
                    throw new Exception("Upload không thành công hoặc parse JSON thất bại.");
                }

                Console.WriteLine($"Upload thành công. DownloadUrl = {result.data.downloadUrl}");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception khi upload file: {ex.Message}");
                throw;
            }
        }


        public static async Task<UploadResponse> UploadFileAsyncWithPath(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File không tồn tại.", filePath);

            using var formData = new MultipartFormDataContent();

            var fileStream = File.OpenRead(filePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            formData.Add(fileContent, "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync("https://cdn.webradarcs2.com/upload", formData);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Upload lỗi: {response.StatusCode} - {body}");
            }

            return JsonSerializer.Deserialize<UploadResponse>(body);
        }

    }
}
