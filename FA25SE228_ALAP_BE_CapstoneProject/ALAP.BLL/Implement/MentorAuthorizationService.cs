using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class MentorAuthorizationService : IMentorAuthorizationService
    {
        private readonly IChatRoomRepository _chatRoomRepository;

        public MentorAuthorizationService(IChatRoomRepository chatRoomRepository)
        {
            _chatRoomRepository = chatRoomRepository;
        }

        public async Task<bool> CanViewStudentProgress(long mentorId, long studentId)
        {
            // Check if mentor has any chat rooms with this student
            // This indicates they are assigned to mentor this student
            var chatRooms = await _chatRoomRepository.GetByMentorId(mentorId);
            return chatRooms.Any(room => room.CreatedById == studentId);
        }

        public async Task<bool> CanViewStudentCourseProgress(long mentorId, long studentId, long courseId)
        {
            // First check if mentor can view student at all
            var canViewStudent = await CanViewStudentProgress(mentorId, studentId);
            if (!canViewStudent)
                return false;

            // TODO: Add course-specific authorization if needed
            // For now, if mentor can view student, they can view all their courses
            return true;
        }

        public async Task<long[]> GetAccessibleStudentIds(long mentorId)
        {
            // Get all students that have chat rooms with this mentor
            var chatRooms = await _chatRoomRepository.GetByMentorId(mentorId);
            return chatRooms.Select(room => room.CreatedById).Distinct().ToArray();
        }
    }
}