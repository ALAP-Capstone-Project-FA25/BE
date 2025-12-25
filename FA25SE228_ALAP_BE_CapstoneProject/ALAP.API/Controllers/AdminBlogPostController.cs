using ALAP.BLL.Interface;
using ALAP.Entity.DTO.Request;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminBlogPostController : BaseAPIController
    {
        private readonly IBlogPostBizLogic _blogPostBizLogic;

        public AdminBlogPostController(IBlogPostBizLogic blogPostBizLogic)
        {
            _blogPostBizLogic = blogPostBizLogic;
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetBlogPostsByPaging([FromQuery] BlogPostFilterDto filter)
        {
            try
            {
                var result = await _blogPostBizLogic.GetBlogPostsByPaging(filter, includeInactive: true);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("{id}/toggle-active")]
        public async Task<IActionResult> ToggleBlogPostActive(long id)
        {
            try
            {
                var result = await _blogPostBizLogic.ToggleBlogPostActive(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(long id)
        {
            try
            {
                var result = await _blogPostBizLogic.DeleteBlogPost(id, (long)UserId, isAdmin: true);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
