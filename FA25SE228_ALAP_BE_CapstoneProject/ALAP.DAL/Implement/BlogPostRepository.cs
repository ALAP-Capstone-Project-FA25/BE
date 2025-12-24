using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.DTO.Request;
using App.Entity.Models;
using App.Entity.Models.Enums;
using App.Entity.Models.Wapper;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.DAL.Implement
{
    public class BlogPostRepository : AppBaseRepository, IBlogPostRepository
    {
        private readonly BaseDBContext _dbContext;

        public BlogPostRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BlogPostModel?> GetById(long id, bool includeInactive = false)
        {
            var query = _dbContext.BlogPosts
                .Include(x => x.Author)
                .Include(x => x.Sections.OrderBy(s => s.OrderIndex))
                .Include(x => x.Comments.Where(c => c.ParentCommentId == null).OrderByDescending(c => c.CreatedAt))
                    .ThenInclude(c => c.User)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.Replies.OrderBy(r => r.CreatedAt))
                        .ThenInclude(r => r.User)
                .Include(x => x.Likes)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedResult<BlogPostModel>> GetByPaging(BlogPostFilterDto filter, bool includeInactive = false)
        {
            var query = _dbContext.BlogPosts
                .Include(x => x.Author)
                .Include(x => x.Sections)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .AsQueryable();

            // Filter by IsActive
            if (!includeInactive)
            {
                query = query.Where(x => x.IsActive);
            }
            else if (filter.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == filter.IsActive.Value);
            }

            // Filter by TargetAudience
            if (filter.TargetAudience.HasValue)
            {
                var targetAudience = filter.TargetAudience.Value;
                query = query.Where(x => x.TargetAudience == targetAudience || x.TargetAudience == BlogPostTargetAudience.BOTH);
            }

            // Filter by Tag
            if (!string.IsNullOrEmpty(filter.Tag))
            {
                query = query.Where(x => x.Tags != null && x.Tags.Contains(filter.Tag));
            }

            // Filter by Keyword
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(x => x.Title.Contains(filter.Keyword));
            }

            // Order by CreatedAt descending
            query = query.OrderByDescending(x => x.CreatedAt);

            var pagingModel = new PagingModel
            {
                PageNumber = filter.Page,
                PageSize = filter.PageSize,
                Keyword = filter.Keyword
            };

            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<bool> Create(BlogPostModel model)
        {
            await _dbContext.BlogPosts.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(BlogPostModel model)
        {
            model.UpdatedAt = Utils.GetCurrentVNTime();
            _dbContext.BlogPosts.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var entity = await _dbContext.BlogPosts
                .Include(x => x.Sections)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new KeyNotFoundException("Không tìm thấy bài viết.");
            }

            // Delete related entities
            _dbContext.BlogPostSections.RemoveRange(entity.Sections);
            _dbContext.BlogPostComments.RemoveRange(entity.Comments);
            _dbContext.BlogPostLikes.RemoveRange(entity.Likes);
            _dbContext.BlogPosts.Remove(entity);

            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ToggleActive(long id)
        {
            var entity = await _dbContext.BlogPosts.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Không tìm thấy bài viết.");
            }

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = Utils.GetCurrentVNTime();
            _dbContext.BlogPosts.Update(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<BlogPostLikeModel?> GetLike(long userId, long blogPostId)
        {
            return await _dbContext.BlogPostLikes
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BlogPostId == blogPostId);
        }

        public async Task<bool> AddLike(BlogPostLikeModel like)
        {
            await _dbContext.BlogPostLikes.AddAsync(like);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> RemoveLike(long userId, long blogPostId)
        {
            var like = await GetLike(userId, blogPostId);
            if (like == null)
            {
                return false;
            }

            _dbContext.BlogPostLikes.Remove(like);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<int> GetLikeCount(long blogPostId)
        {
            return await _dbContext.BlogPostLikes
                .CountAsync(x => x.BlogPostId == blogPostId);
        }

        public async Task<bool> UserHasLiked(long userId, long blogPostId)
        {
            return await _dbContext.BlogPostLikes
                .AnyAsync(x => x.UserId == userId && x.BlogPostId == blogPostId);
        }
    }
}
