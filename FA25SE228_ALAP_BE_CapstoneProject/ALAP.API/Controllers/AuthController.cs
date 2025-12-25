using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request.User;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models.Wapper;
using Base.API;
using EventZ.API.MiddleWare;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static QRCoder.PayloadGenerator;
using System.Security.Claims;
using System.Text.Json;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : BaseAPIController
    {
        private readonly IIdentityBiz _identityBiz;
        private readonly ILogger<AuthController> _logger;
        private readonly IIdentityRepository _identityRepository;
        private readonly IConfiguration _configuration;
        public AuthController(IIdentityBiz identityBiz, ILogger<AuthController> logger, IIdentityRepository identityRepository, IConfiguration configuration)
        {
            this._identityBiz = identityBiz;
            _logger = logger;
            _identityRepository = identityRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequestDTO dto)
        {
            try
            {
                var result = await _identityBiz.Login(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[AuthController] {0}. TK: {1}. MK: {2}. {3}", ex.Message, dto.UserName, dto.Password, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }


        [HttpPost("login-sysadmin")]
        public async Task<IActionResult> LoginGoogleAuthenticator(TwoFactorAuthRequest dto)
        {
            try
            {
                var result = await _identityBiz.LoginGoogleAuthenticator(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[LoginGoogleAuthenticator] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("get-info")]
        public async Task<IActionResult> GetInfo()
        {
            try
            {
                var result = await _identityBiz.GetInfo(UserId);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetInfo] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bool emailExitsting = await _identityBiz.CheckEmailAlready(request.Email);

                if (emailExitsting)
                {
                    throw new Exception("Email already exists");
                }

                var result = await _identityRepository.Register(request);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Register] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }


        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            try
            {
                var result = await _identityBiz.VerifyEmailTokenAsync(token);
                var clientURL = _configuration["ClientURL"] ?? "http://localhost:3000";

                if (result.Contains("Verified"))
                    return Redirect($"{clientURL}/email-verified");
                else
                    return Redirect($"{clientURL}/email-verified?error=true");
            }
            catch (Exception ex)
            {
                _logger.LogError("[VerifyEmail] {0} {1}", ex.Message, ex.StackTrace);
                var clientURL = _configuration["ClientURL"] ?? "http://localhost:3000";
                return Redirect($"{clientURL}/email-verified?error=true");
            }
        }


        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            try
            {
                int userID = JWTHandler.GetUserIdFromHttpContext(HttpContext);
                if (request == null)
                {
                    return BadRequest();
                }

                var response = await _identityBiz.ChangePassword(UserId, request);
                if (response == "Old password incorrect")
                {
                    return BadRequest(response);
                }

                else if (response == "User Not Found")
                {
                    return NotFound(response);
                }

                else
                {
                    return Success(response);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError("[Change passwrod] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }



        [HttpGet("test-telegram-error")]
        public IActionResult TestTelegramError()
        {
            _logger.LogError("Hello tôi là bug rất vui được gặp.");
            return Ok("Đã gửi lỗi qua Telegram.");
        }

        [HttpGet("get-chat-bot-id")]
        public async Task<IActionResult> GetChatBotID()
        {

            string botToken = _configuration["Serilog:WriteTo:2:Args:token"]!;
            string url = $"https://api.telegram.org/bot{botToken}/getUpdates";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<TelegramResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (jsonResponse?.Ok == true && jsonResponse.Result?.Any() == true)
                    {
                        var chatId = jsonResponse.Result.First().Message.Chat.Id;
                        return Ok(new { ChatId = chatId });
                    }

                    return Ok(new { Message = "Không tìm thấy cập nhật nào. Hãy nhắn tin cho bot trước." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Error = ex.Message });
                }
            }
        }


        [HttpGet("get-profile-by-user")]
        public async Task<IActionResult> GetProfileByUser()
        {
            try
            {

                int userID = JWTHandler.GetUserIdFromHttpContext(HttpContext);
                var response = await _identityBiz.GetProfileByUser(userID);

                if (response == null) return NotFound(new { Status = false, Message = "User not found", StatusCode = 404 });

                return Success("Get user profile is success", response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Get profile by user] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }

        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO request)
        {
            try
            {
                int userID = JWTHandler.GetUserIdFromHttpContext(HttpContext);
                var response = await _identityBiz.UpdateProfile(userID, request);

                if (response == null) return NotFound(new { Status = false, Message = "User not found", StatusCode = 404 });

                return Success("Update profile successful", response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Update profile] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpPost("send-mail-reset-password")]
        public async Task<IActionResult> SendMailResetPassword([FromBody] ForgotPasswordRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _identityBiz.SendMailResetPassword(dto.Email);

                return result.Contains("success", StringComparison.OrdinalIgnoreCase)
                    ? Success(result)
                    : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Send Mail Reset Password] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
        {
            try
            {
                var result = await _identityBiz.ResetPassword(request.Token, request.NewPassword);

                if (result == "Password reset successful")
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Reset Password] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpGet("get-user-by-paging")]
        public async Task<IActionResult> GetUserByPaging([FromQuery] PagingModel model)
        {
            try
            {
                var result = await _identityBiz.GetUserByPaging(model);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Get User By Paging] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpPost("e-login")]
        public async Task<IActionResult> LoginEvent([FromBody] UserLoginRequestDTO dto)
        {
            try
            {
                var result = await _identityBiz.LoginEvent(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Login Event] {0} {1}", ex.Message, ex.StackTrace);
                return Error(ex.Message);
            }
        }

        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal?.Claims;

            var userId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var avatar = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

            var UserLoginGoogleDTO = new UserLoginGoogleDTO
            {
                GoogleId = userId,
                Email = email,
                Name = name,
                Avatar = avatar
            };

            var token = await _identityBiz.CreateUserGoogle(UserLoginGoogleDTO);
            var clientURL = _configuration["ClientURL"]!;
            return Redirect($"{clientURL}/auth/google-callback?token={token}");
        }


        [HttpGet("test-api")]
        public async Task<IActionResult> TestAPI()
        {
            try
            {
                var testResult = new
                {
                    message = "TEST API is working correctly!",
                };
                return GetSuccess(testResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Test API] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpPut("toggle-user-status/{userId}")]
        [Authorize]
        public async Task<IActionResult> ToggleUserStatus(int userId, [FromBody] bool isActive)
        {
            try
            {
                var result = await _identityBiz.ChangeIsActive(isActive, userId);

                if (result)
                {
                    return Success("Cập nhật trạng thái người dùng thành công");
                }
                else
                {
                    return BadRequest("Không thể cập nhật trạng thái người dùng");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("[Toggle User Status] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }
    }
}
