using ALAP.BLL.Interface;
using ALAP.Entity.DTO.Request.User;
using Base.API;
using Base.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleConstant.ADMIN)]
    public class SpeakerController : BaseAPIController
    {
        private readonly IIdentityBiz _identityBiz;
        private readonly ILogger<SpeakerController> _logger;

        public SpeakerController(IIdentityBiz identityBiz, ILogger<SpeakerController> logger)
        {
            _identityBiz = identityBiz;
            _logger = logger;
        }

        /// <summary>
        /// Tạo tài khoản speaker mới (chỉ admin)
        /// </summary>
        /// <param name="dto">Thông tin speaker</param>
        /// <returns>Thông tin speaker đã tạo</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateSpeaker([FromBody] CreateSpeakerRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var speaker = await _identityBiz.CreateSpeaker(dto);

                return GetSuccess(new
                {
                    id = speaker.Id,
                    firstName = speaker.FirstName,
                    lastName = speaker.LastName,
                    email = speaker.Email,
                    phone = speaker.Phone,
                    address = speaker.Address,
                    avatar = speaker.Avatar,
                    role = speaker.Role.ToString(),
                    createdAt = speaker.CreatedAt,
                    message = "Speaker account created successfully. Welcome email has been sent."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[CreateSpeaker] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách speakers với phân trang
        /// </summary>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Số lượng mỗi trang</param>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách speakers</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetSpeakers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string keyword = "")
        {
            try
            {
                var pagingModel = new ALAP.Entity.Models.Wapper.PagingModel
                {
                    PageNumber = page,
                    PageSize = pageSize,
                    Keyword = keyword,
                    UserRole = ALAP.Entity.Models.Enums.UserRole.SPEAKER
                };

                var result = await _identityBiz.GetUserByPaging(pagingModel);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetSpeakers] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết speaker bao gồm thông tin về events và payments
        /// </summary>
        /// <param name="id">ID của speaker</param>
        /// <returns>Thông tin chi tiết speaker</returns>
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetSpeakerDetails(long id)
        {
            try
            {
                var result = await _identityBiz.GetSpeakerDetails(id);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetSpeakerDetails] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }
    }
}
