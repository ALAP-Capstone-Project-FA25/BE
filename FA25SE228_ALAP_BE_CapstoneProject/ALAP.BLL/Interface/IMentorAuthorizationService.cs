using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IMentorAuthorizationService
    {
        /// <summary>
        /// Check if a mentor has permission to view a specific student's progress
        /// </summary>
        /// <param name="mentorId">Mentor user ID</param>
        /// <param name="studentId">Student user ID</param>
        /// <returns>True if mentor can view student's progress</returns>
        Task<bool> CanViewStudentProgress(long mentorId, long studentId);

        /// <summary>
        /// Check if a mentor has permission to view a specific student's course progress
        /// </summary>
        /// <param name="mentorId">Mentor user ID</param>
        /// <param name="studentId">Student user ID</param>
        /// <param name="courseId">Course ID</param>
        /// <returns>True if mentor can view student's course progress</returns>
        Task<bool> CanViewStudentCourseProgress(long mentorId, long studentId, long courseId);

        /// <summary>
        /// Get list of student IDs that a mentor can access
        /// </summary>
        /// <param name="mentorId">Mentor user ID</param>
        /// <returns>List of student IDs</returns>
        Task<long[]> GetAccessibleStudentIds(long mentorId);
    }
}