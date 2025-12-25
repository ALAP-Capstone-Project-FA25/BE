using ALAP.BLL.Interface;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPackageController : BaseAPIController
    {
        private readonly IUserPackageBizLogic _userPackageBizLogic;
        public UserPackageController(IUserPackageBizLogic userPackageBizLogic)
        {
            _userPackageBizLogic = userPackageBizLogic;
        }

        [HttpGet("get-user-packages")]
        [Authorize]
        public async Task<IActionResult> GetUserPackages()
        {
            try
            {
                var result = await _userPackageBizLogic.GetUserPackageByUserId(UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
