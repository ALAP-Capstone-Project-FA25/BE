using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeetController : BaseAPIController
    {
        private readonly HttpClient _httpClient;

        public MeetController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Tạo link Google Meet mới cho mentor và student
        /// </summary>
        /// <returns>Link Google Meet</returns>
        [HttpPost("generate-link")]
        public async Task<IActionResult> GenerateMeetLink()
        {
            try
            {
                var meetLink = await GenerateMeetLinkAsync();
                return GetSuccess(new { meetLink });
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi tạo Meet link: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo link Google Meet nhanh - trả về link trực tiếp
        /// </summary>
        /// <returns>Link Google Meet</returns>
        [HttpPost("quick")]
        public async Task<IActionResult> QuickMeet()
        {
            try
            {
                var link = await GenerateMeetLinkAsync();
                return GetSuccess(link);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi tạo Meet link: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo link Google Meet cho cuộc họp giữa mentor và student cụ thể
        /// </summary>
        /// <param name="mentorId">ID của mentor</param>
        /// <param name="studentId">ID của student</param>
        /// <returns>Link Google Meet với thông tin cuộc họp</returns>
        [HttpGet("generate-link/{mentorId}/{studentId}")]
        public async Task<IActionResult> GenerateMeetLinkForSession(long mentorId, long studentId)
        {
            try
            {
                var meetLink = await GenerateMeetLinkAsync();

                // Có thể lưu thông tin cuộc họp vào database nếu cần
                var meetingInfo = new
                {
                    meetLink,
                    mentorId,
                    studentId,
                    createdAt = DateTime.UtcNow,
                    createdBy = UserId
                };

                return GetSuccess(meetingInfo);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi tạo Meet link: {ex.Message}");
            }
        }

        private async Task<string> GenerateMeetLinkAsync()
        {
            // Generate link trực tiếp với format: https://meet.google.com/xxx-yyyy-zzz
            return await Task.FromResult(GenerateGoogleMeetLink());
        }

        private string GenerateGoogleMeetLink()
        {
            // Trả về link Google Meet cố định
            const string meetLink = "https://meet.google.com/odm-qtej-sgs";
            return meetLink;
        }
    }
}
