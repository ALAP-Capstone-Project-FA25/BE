using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IBlogPostBizLogic
    {
        Task<BlogPostResponseDto?> GetBlogPostById(long id, bool includeInactive = false);
        Task<PagedResult<BlogPostModel>> GetBlogPostsByPaging(BlogPostFilterDto filter, bool includeInactive = false);
        Task<bool> CreateUpdateBlogPost(CreateUpdateBlogPostDto dto, long userId);
        Task<bool> DeleteBlogPost(long id, long userId, bool isAdmin = false);
        Task<bool> ToggleBlogPostActive(long id);
        Task<bool> LikeBlogPost(long blogPostId, long userId);
        Task<bool> UnlikeBlogPost(long blogPostId, long userId);
        Task<bool> UserHasLiked(long blogPostId, long userId);
        Task<int> GetLikeCount(long blogPostId);
        Task<BlogPostCommentModel> CreateComment(CreateBlogPostCommentDto dto, long userId);
        Task<bool> DeleteComment(long commentId, long userId);
        Task<List<string>> GetPopularTags(int limit = 10);
    }
}
