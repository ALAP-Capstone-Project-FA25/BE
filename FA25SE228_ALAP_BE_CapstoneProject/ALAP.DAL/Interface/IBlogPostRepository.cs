using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace ALAP.DAL.Interface
{
    public interface IBlogPostRepository
    {
        Task<BlogPostModel?> GetById(long id, bool includeInactive = false);
        Task<PagedResult<BlogPostModel>> GetByPaging(BlogPostFilterDto filter, bool includeInactive = false);
        Task<bool> Create(BlogPostModel model);
        Task<bool> Update(BlogPostModel model);
        Task<bool> Delete(long id);
        Task<bool> ToggleActive(long id);
        Task<BlogPostLikeModel?> GetLike(long userId, long blogPostId);
        Task<bool> AddLike(BlogPostLikeModel like);
        Task<bool> RemoveLike(long userId, long blogPostId);
        Task<int> GetLikeCount(long blogPostId);
        Task<bool> UserHasLiked(long userId, long blogPostId);
    }
}
