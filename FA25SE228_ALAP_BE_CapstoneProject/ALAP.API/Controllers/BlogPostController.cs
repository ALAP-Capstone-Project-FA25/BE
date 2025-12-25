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
    public class BlogPostController : BaseAPIController
    {
        private readonly IBlogPostBizLogic _blogPostBizLogic;

        public BlogPostController(IBlogPostBizLogic blogPostBizLogic)
        {
            _blogPostBizLogic = blogPostBizLogic;
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetBlogPostsByPaging([FromQuery] BlogPostFilterDto filter)
        {
            try
            {
                var result = await _blogPostBizLogic.GetBlogPostsByPaging(filter, includeInactive: false);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogPostById(long id)
        {
            try
            {
                var result = await _blogPostBizLogic.GetBlogPostById(id, includeInactive: false);
                if (result == null)
                {
                    return GetError("Không tìm thấy bài viết.");
                }
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        [Authorize]
        public async Task<IActionResult> CreateUpdateBlogPost([FromBody] CreateUpdateBlogPostDto dto)
        {
            try
            {
                var result = await _blogPostBizLogic.CreateUpdateBlogPost(dto, (long)UserId);
                return SaveSuccess(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlogPost(long id)
        {
            try
            {
                var result = await _blogPostBizLogic.DeleteBlogPost(id, (long)UserId, isAdmin: false);
                return SaveSuccess(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikeBlogPost(long id)
        {
            try
            {
                var hasLiked = await _blogPostBizLogic.UserHasLiked(id, (long)UserId);
                bool result;

                if (hasLiked)
                {
                    result = await _blogPostBizLogic.UnlikeBlogPost(id, (long)UserId);
                }
                else
                {
                    result = await _blogPostBizLogic.LikeBlogPost(id, (long)UserId);
                }

                var likeCount = await _blogPostBizLogic.GetLikeCount(id);
                var isLiked = await _blogPostBizLogic.UserHasLiked(id, (long)UserId);

                return SaveSuccess(new
                {
                    IsLiked = isLiked,
                    LikeCount = likeCount
                });
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPost("{id}/comment")]
        [Authorize]
        public async Task<IActionResult> CreateComment(long id, [FromBody] CreateBlogPostCommentDto dto)
        {
            try
            {
                dto.BlogPostId = id;
                var result = await _blogPostBizLogic.CreateComment(dto, (long)UserId);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("comment/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(long commentId)
        {
            try
            {
                var result = await _blogPostBizLogic.DeleteComment(commentId, (long)UserId);
                return SaveSuccess(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpGet("tags/popular")]
        public async Task<IActionResult> GetPopularTags([FromQuery] int limit = 10)
        {
            try
            {
                var result = await _blogPostBizLogic.GetPopularTags(limit);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
