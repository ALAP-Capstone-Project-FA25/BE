using ALAP.BLL.Interface;
using Base.API;
using Base.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : BaseAPIController
    {
        private readonly IEmailBizLogic _emailService;

        public EmailController(IEmailBizLogic emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        [HAuthorize(RoleConstant.ADMIN)]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            bool success = await _emailService.SendEmailAsync(request.To, request.Subject, request.Body, request.IsHtml);

            if (success)
                return Ok(new { message = "Email sent successfully!" });
            else
                return StatusCode(500, new { message = "Failed to send email." });
        }
    }
    public class EmailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = false;
    }
}
