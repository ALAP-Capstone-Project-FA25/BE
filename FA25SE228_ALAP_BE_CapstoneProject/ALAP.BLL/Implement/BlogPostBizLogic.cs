using ALAP.BLL.Interface;
using ALAP.DAL.DataBase;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class BlogPostBizLogic : AppBaseBizLogic, IBlogPostBizLogic
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly INotificationBizLogic _notificationBizLogic;

        public BlogPostBizLogic(BaseDBContext dbContext, IBlogPostRepository blogPostRepository, INotificationBizLogic notificationBizLogic) : base(dbContext)
        {
            _blogPostRepository = blogPostRepository;
            _notificationBizLogic = notificationBizLogic;
        }

        public async Task<BlogPostResponseDto?> GetBlogPostById(long id, bool includeInactive = false)
        {
            var model = await _blogPostRepository.GetById(id, includeInactive);
            if (model == null)
            {
                return null;
            }

            // Map to DTO
            var dto = new BlogPostResponseDto
            {
                Id = model.Id,
                Title = model.Title,
                ImageUrl = model.ImageUrl,
                TargetAudience = model.TargetAudience,
                AuthorId = model.AuthorId,
                Tags = model.Tags,
                IsActive = model.IsActive,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                Author = model.Author != null ? new UserResponseDTO
                {
                    Id = (int)model.Author.Id,
                    FirstName = model.Author.FirstName ?? string.Empty,
                    LastName = model.Author.LastName ?? string.Empty,
                    Email = model.Author.Email ?? string.Empty,
                    Avatar = model.Author.Avatar ?? string.Empty,
                    Gender = model.Author.Gender.ToString(),
                    Role = model.Author.Role,
                    EmailConfirmed = model.Author.EmailConfirmed,
                    CreatedAt = model.Author.CreatedAt,
                    UpdatedAt = model.Author.UpdatedAt
                } : null,
                Sections = model.Sections?.Select(s => new BlogPostSectionResponseDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Content = s.Content,
                    OrderIndex = s.OrderIndex,
                    BlogPostId = s.BlogPostId,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }).ToList() ?? new List<BlogPostSectionResponseDto>(),
                Comments = MapComments(model.Comments?.Where(c => c.ParentCommentId == null).ToList() ?? new List<BlogPostCommentModel>()),
                Likes = model.Likes?.Select(l => new BlogPostLikeResponseDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    BlogPostId = l.BlogPostId,
                    CreatedAt = l.CreatedAt
                }).ToList() ?? new List<BlogPostLikeResponseDto>()
            };

            return dto;
        }

        private List<BlogPostCommentResponseDto> MapComments(List<BlogPostCommentModel> comments)
        {
            return comments.Select(c => new BlogPostCommentResponseDto
            {
                Id = c.Id,
                Content = c.Content,
                UserId = c.UserId,
                BlogPostId = c.BlogPostId,
                ParentCommentId = c.ParentCommentId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                User = c.User != null ? new UserResponseDTO
                {
                    Id = (int)c.User.Id,
                    FirstName = c.User.FirstName ?? string.Empty,
                    LastName = c.User.LastName ?? string.Empty,
                    Email = c.User.Email ?? string.Empty,
                    Avatar = c.User.Avatar ?? string.Empty,
                    Gender = c.User.Gender.ToString(),
                    Role = c.User.Role,
                    EmailConfirmed = c.User.EmailConfirmed,
                    CreatedAt = c.User.CreatedAt,
                    UpdatedAt = c.User.UpdatedAt
                } : null,
                Replies = MapComments(c.Replies?.ToList() ?? new List<BlogPostCommentModel>())
            }).ToList();
        }

        public async Task<PagedResult<BlogPostModel>> GetBlogPostsByPaging(BlogPostFilterDto filter, bool includeInactive = false)
        {
            return await _blogPostRepository.GetByPaging(filter, includeInactive);
        }

        public async Task<bool> CreateUpdateBlogPost(CreateUpdateBlogPostDto dto, long userId)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException("Tiêu đề không được để trống.");
            }

            if (dto.Sections == null || !dto.Sections.Any())
            {
                throw new ArgumentException("Bài viết phải có ít nhất một section.");
            }

            // Serialize tags to JSON
            string tagsJson = dto.Tags != null && dto.Tags.Any()
                ? JsonSerializer.Serialize(dto.Tags)
                : null;

            if (dto.Id > 0)
            {
                // Update existing post
                var existingPost = await _blogPostRepository.GetById(dto.Id, includeInactive: true);
                if (existingPost == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy bài viết.");
                }

                // Check permission - only author can edit
                if (existingPost.AuthorId != userId)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa bài viết này.");
                }

                existingPost.Title = dto.Title;
                existingPost.ImageUrl = dto.ImageUrl;
                existingPost.TargetAudience = dto.TargetAudience;
                existingPost.Tags = tagsJson;
                existingPost.UpdatedAt = Utils.GetCurrentVNTime();

                // Update sections
                var existingSections = _dbContext.BlogPostSections.Where(s => s.BlogPostId == dto.Id).ToList();
                _dbContext.BlogPostSections.RemoveRange(existingSections);

                foreach (var sectionDto in dto.Sections.OrderBy(s => s.OrderIndex))
                {
                    var section = new BlogPostSectionModel
                    {
                        Title = sectionDto.Title,
                        Content = sectionDto.Content,
                        OrderIndex = sectionDto.OrderIndex,
                        BlogPostId = dto.Id,
                        CreatedAt = Utils.GetCurrentVNTime()
                    };
                    await _dbContext.BlogPostSections.AddAsync(section);
                }

                return await _blogPostRepository.Update(existingPost);
            }
            else
            {
                // Create new post
                var newPost = new BlogPostModel
                {
                    Title = dto.Title,
                    ImageUrl = dto.ImageUrl,
                    TargetAudience = dto.TargetAudience,
                    Tags = tagsJson,
                    AuthorId = userId,
                    IsActive = true,
                    CreatedAt = Utils.GetCurrentVNTime()
                };

                await _blogPostRepository.Create(newPost);

                // Add sections
                foreach (var sectionDto in dto.Sections.OrderBy(s => s.OrderIndex))
                {
                    var section = new BlogPostSectionModel
                    {
                        Title = sectionDto.Title,
                        Content = sectionDto.Content,
                        OrderIndex = sectionDto.OrderIndex,
                        BlogPostId = newPost.Id,
                        CreatedAt = Utils.GetCurrentVNTime()
                    };
                    await _dbContext.BlogPostSections.AddAsync(section);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteBlogPost(long id, long userId, bool isAdmin = false)
        {
            var post = await _blogPostRepository.GetById(id, includeInactive: true);
            if (post == null)
            {
                throw new KeyNotFoundException("Không tìm thấy bài viết.");
            }

            // Check permission - only author or admin can delete
            if (!isAdmin && post.AuthorId != userId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa bài viết này.");
            }

            return await _blogPostRepository.Delete(id);
        }

        public async Task<bool> ToggleBlogPostActive(long id)
        {
            return await _blogPostRepository.ToggleActive(id);
        }

        public async Task<bool> LikeBlogPost(long blogPostId, long userId)
        {
            var hasLiked = await _blogPostRepository.UserHasLiked(userId, blogPostId);
            if (hasLiked)
            {
                return false; // Already liked
            }

            var post = await _blogPostRepository.GetById(blogPostId);
            if (post == null)
            {
                throw new KeyNotFoundException("Không tìm thấy bài viết.");
            }

            var like = new BlogPostLikeModel
            {
                UserId = userId,
                BlogPostId = blogPostId,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            var result = await _blogPostRepository.AddLike(like);

            // Create notification for post author if not liking own post
            if (result && post.AuthorId != userId)
            {
                try
                {
                    var liker = await _dbContext.Users.FindAsync(userId);
                    var likerName = liker != null ? $"{liker.FirstName} {liker.LastName}".Trim() : "Ai đó";
                    await _notificationBizLogic.CreateNotification(
                        post.AuthorId,
                        NotificationType.BLOG_POST_LIKED,
                        $"{likerName} đã thích bài viết của bạn",
                        $"Bài viết: {post.Title}",
                        $"/blog/{blogPostId}"
                    );
                }
                catch
                {
                    // Silently fail - notification is not critical
                }
            }

            return result;
        }

        public async Task<bool> UnlikeBlogPost(long blogPostId, long userId)
        {
            return await _blogPostRepository.RemoveLike(userId, blogPostId);
        }

        public async Task<bool> UserHasLiked(long blogPostId, long userId)
        {
            return await _blogPostRepository.UserHasLiked(userId, blogPostId);
        }

        public async Task<int> GetLikeCount(long blogPostId)
        {
            return await _blogPostRepository.GetLikeCount(blogPostId);
        }

        public async Task<BlogPostCommentModel> CreateComment(CreateBlogPostCommentDto dto, long userId)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                throw new ArgumentException("Nội dung bình luận không được để trống.");
            }

            var post = await _blogPostRepository.GetById(dto.BlogPostId);
            if (post == null)
            {
                throw new KeyNotFoundException("Không tìm thấy bài viết.");
            }

            // If this is a reply, validate parent comment exists
            BlogPostCommentModel? parentComment = null;
            if (dto.ParentCommentId.HasValue)
            {
                parentComment = await _dbContext.BlogPostComments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == dto.ParentCommentId.Value && c.BlogPostId == dto.BlogPostId);

                if (parentComment == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy bình luận cha.");
                }
            }

            var comment = new BlogPostCommentModel
            {
                Content = dto.Content,
                UserId = userId,
                BlogPostId = dto.BlogPostId,
                ParentCommentId = dto.ParentCommentId,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.BlogPostComments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();

            // Load user for response
            await _dbContext.Entry(comment).Reference(c => c.User).LoadAsync();

            // Create notification
            try
            {
                var commenter = await _dbContext.Users.FindAsync(userId);
                var commenterName = commenter != null ? $"{commenter.FirstName} {commenter.LastName}".Trim() : "Ai đó";

                if (parentComment != null)
                {
                    // Notify parent comment author (if not replying to own comment)
                    if (parentComment.UserId != userId)
                    {
                        await _notificationBizLogic.CreateNotification(
                            parentComment.UserId,
                            NotificationType.BLOG_COMMENT_REPLY,
                            $"{commenterName} đã trả lời bình luận của bạn",
                            dto.Content.Length > 100 ? dto.Content.Substring(0, 100) + "..." : dto.Content,
                            $"/blog/{dto.BlogPostId}"
                        );
                    }
                }
                else
                {
                    // Notify post author (if not commenting on own post)
                    if (post.AuthorId != userId)
                    {
                        await _notificationBizLogic.CreateNotification(
                            post.AuthorId,
                            NotificationType.BLOG_COMMENT_REPLY,
                            $"{commenterName} đã bình luận bài viết của bạn",
                            dto.Content.Length > 100 ? dto.Content.Substring(0, 100) + "..." : dto.Content,
                            $"/blog/{dto.BlogPostId}"
                        );
                    }
                }
            }
            catch
            {
                // Silently fail - notification is not critical
            }

            return comment;
        }

        public async Task<bool> DeleteComment(long commentId, long userId)
        {
            var comment = await _dbContext.BlogPostComments
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException("Không tìm thấy bình luận.");
            }

            // Check permission - only author can delete
            if (comment.UserId != userId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa bình luận này.");
            }

            _dbContext.BlogPostComments.Remove(comment);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetPopularTags(int limit = 10)
        {
            var allPosts = await _dbContext.BlogPosts
                .Where(x => x.IsActive && !string.IsNullOrEmpty(x.Tags))
                .Select(x => x.Tags)
                .ToListAsync();

            var tagCounts = new Dictionary<string, int>();

            foreach (var tagsJson in allPosts)
            {
                try
                {
                    var tags = JsonSerializer.Deserialize<List<string>>(tagsJson);
                    if (tags != null)
                    {
                        foreach (var tag in tags)
                        {
                            if (!string.IsNullOrWhiteSpace(tag))
                            {
                                var normalizedTag = tag.Trim().ToLower();
                                if (tagCounts.ContainsKey(normalizedTag))
                                {
                                    tagCounts[normalizedTag]++;
                                }
                                else
                                {
                                    tagCounts[normalizedTag] = 1;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Skip invalid JSON
                }
            }

            return tagCounts
                .OrderByDescending(x => x.Value)
                .Take(limit)
                .Select(x => x.Key)
                .ToList();
        }
    }
}
