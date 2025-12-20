using ALAP.BLL.Interface;
using ALAP.DAL.DataBase;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class ChatRoomBizLogic : AppBaseBizLogic, IChatRoomBizLogic
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly BaseDBContext _dbContext;
        public ChatRoomBizLogic(BaseDBContext dbContext, IChatRoomRepository chatRoomRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _chatRoomRepository = chatRoomRepository;
        }

        public async Task<bool> CreateUpdateChatRoom(CreateUpdateChatRoomDto dto)
        {
            if (dto.Id > 0)
            {
                var model = new ChatRoomModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    CreatedById = dto.CreatedById,
                    ParticipantId = dto.ParticipantId,
                    UpdatedAt = Utils.GetCurrentVNTime(),
                };
                return await _chatRoomRepository.Update(model);
            }
            else
            {
                var model = new ChatRoomModel
                {
                    Name = dto.Name,
                    CreatedById = dto.CreatedById,
                    ParticipantId = dto.ParticipantId,
                };
                return await _chatRoomRepository.Create(model);
            }
        }

        public async Task<bool> DeleteChatRoom(long id)
        {
            return await _chatRoomRepository.Delete(id);
        }

        public async Task<ChatRoomModel> GetChatRoomByCourseId(long courseId, long userId)
        {
            var existingChatRoom = await _dbContext.ChatRooms
                .Include(cr => cr.Messages)
                .FirstOrDefaultAsync(cr => cr.CourseId == courseId && cr.CreatedById == userId);

            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var existingCourse = await _dbContext.Courses 
                .FirstOrDefaultAsync(c => c.Id == courseId) ?? throw new KeyNotFoundException("Không tìm thấy khóa học.");

            if (existingChatRoom != null)
            {
                var chatRoomDto = new ChatRoomWithMessageDto
                {
                    ChatRoom = existingChatRoom,
                    Messages = existingChatRoom.Messages.ToList()
                };
                return existingChatRoom;
            }
            else
            {
                ChatRoomModel newChatRoom = new ChatRoomModel
                {
                    Name = $"{existingCourse.Title} - {existingUser.Username}",
                    CreatedById = userId,
                    ParticipantId = existingCourse.MentorId,
                    CourseId = courseId
                };
                await _chatRoomRepository.Create(newChatRoom);
                return existingChatRoom;
            }
        }

        public async Task<ChatRoomModel> GetChatRoomById(long id)
        {
            return await _chatRoomRepository.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy phòng chat.");
        }

        public async Task<List<ChatRoomModel>> GetListChatRoomByMentorId(long mentorId)
        {
            var chatRooms = await  _dbContext.Courses
                .Where(cr => cr.MentorId == mentorId)
                .Join(_dbContext.ChatRooms,
                      course => course.Id,
                      chatRoom => chatRoom.CourseId,
                      (course, chatRoom) => chatRoom)
                .Include(cr => cr.Course)
                .Include(cr => cr.Messages )
                .Include(cr => cr.CreatedBy)
                .ToListAsync();
            return chatRooms;

        }


        public async Task<PagedResult<ChatRoomModel>> GetListChatRoomsByPaging(PagingModel pagingModel)
        {
            return await _chatRoomRepository.GetListByPaging(pagingModel);
        }
    }
}

