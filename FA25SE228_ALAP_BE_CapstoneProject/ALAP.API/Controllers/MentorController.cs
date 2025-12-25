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
    public class MentorController : BaseAPIController
    {
        private readonly IIdentityBiz _identityBiz;
        private readonly ILogger<MentorController> _logger;

        public MentorController(IIdentityBiz identityBiz, ILogger<MentorController> logger)
        {
            _identityBiz = identityBiz;
            _logger = logger;
        }

        /// <summary>
        /// Tạo tài khoản mentor mới (chỉ admin)
        /// </summary>
        /// <param name="dto">Thông tin mentor</param>
        /// <returns>Thông tin mentor đã tạo</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateMentor([FromBody] CreateMentorRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var mentor = await _identityBiz.CreateMentor(dto);

                return GetSuccess(new
                {
                    id = mentor.Id,
                    firstName = mentor.FirstName,
                    lastName = mentor.LastName,
                    email = mentor.Email,
                    phone = mentor.Phone,
                    address = mentor.Address,
                    avatar = mentor.Avatar,
                    bio = mentor.Bio,
                    role = mentor.Role.ToString(),
                    createdAt = mentor.CreatedAt,
                    message = "Mentor account created successfully. Welcome email has been sent."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[CreateMentor] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }
    }
}
