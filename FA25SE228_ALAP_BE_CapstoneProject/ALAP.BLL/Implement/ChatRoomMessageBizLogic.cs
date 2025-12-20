using ALAP.BLL.Interface;
using ALAP.DAL.DataBase;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class ChatRoomMessageBizLogic : IChatRoomMessageBizLogic
    {
        private readonly IChatRoomMessageRepository _repo;
        private readonly BaseDBContext _dbContext;

        public ChatRoomMessageBizLogic(IChatRoomMessageRepository repo, BaseDBContext dbContext)
        {
            _repo = repo;
            _dbContext = dbContext;
        }

        public async Task<bool> CreateUpdateChatRoomMessage(CreateUpdateChatRoomMessageDto dto)
        {
            if (dto.Id > 0)
            {
                var model = new ChatRoomMessageModel
                {
                    Id = dto.Id,
                    ChatRoomId = dto.ChatRoomId,
                    Content = dto.Content,
                    IsUser = dto.IsUser,
                    IsRead = dto.IsRead,
                    MessageLink = dto.MessageLink,
                    MessageType = dto.MessageType,
                    UpdatedAt = Utils.GetCurrentVNTime()
                };
                return await _repo.Update(model);
            }
            else
            {
                var model = new ChatRoomMessageModel
                {
                    ChatRoomId = dto.ChatRoomId,
                    Content = dto.Content,
                    MessageLink = dto.MessageLink,
                    IsUser = dto.IsUser,
                    IsRead = dto.IsRead,
                    MessageType = dto.MessageType
                };
                var result = await _repo.Create(model);

                // Create notification when mentor sends message (IsUser = false means mentor)
                if (result && !dto.IsUser)
                {
                    await CreateMentorMessageNotificationAsync(dto.ChatRoomId, dto.Content);
                }

                return result;
            }
        }

        public async Task<bool> DeleteChatRoomMessage(long id)
        {
            return await _repo.Delete(id);
        }

        public async Task<ChatRoomMessageModel> GetChatRoomMessageById(long id)
        {
            return await _repo.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy tin nhắn.");
        }

        public async Task<PagedResult<ChatRoomMessageModel>> GetListChatRoomMessagesByPaging(PagingModel pagingModel)
        {
            return await _repo.GetListByPaging(pagingModel);
        }

        public async Task<List<ChatRoomMessageModel>> GetMessagesByChatRoomId(long chatRoomId)
        {
            return await _repo.GetByChatRoomId(chatRoomId);
        }

        private async Task CreateMentorMessageNotificationAsync(long chatRoomId, string messageContent)
        {
            try
            {
                var chatRoom = await _dbContext.ChatRooms
                    .Include(cr => cr.CreatedBy)
                    .Include(cr => cr.Course)
                    .FirstOrDefaultAsync(cr => cr.Id == chatRoomId);

                if (chatRoom == null || chatRoom.CreatedBy == null)
                    return;

                var notification = new NotificationModel
                {
                    UserId = chatRoom.CreatedById, // User who created the chat room
                    Type = NotificationType.MENTOR_MESSAGE,
                    Title = $"Mentor đã nhắn tin cho bạn",
                    Message = messageContent.Length > 100
                        ? messageContent.Substring(0, 100) + "..."
                        : messageContent,
                    LinkUrl = $"/mentor/chat/{chatRoomId}",
                    IsRead = false,
                    CreatedAt = Utils.GetCurrentVNTime(),
                    UpdatedAt = Utils.GetCurrentVNTime()
                };

                await _dbContext.Notifications.AddAsync(notification);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                // Silently fail - notification is not critical
            }
        }
    }
}

