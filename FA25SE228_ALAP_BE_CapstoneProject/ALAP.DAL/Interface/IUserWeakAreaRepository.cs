using App.Entity.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface IUserWeakAreaRepository
    {
        Task<UserWeakArea?> GetByUserAndLesson(long userId, long lessonId);
        Task<UserWeakArea> CreateOrUpdate(UserWeakArea weakArea);
        Task<List<UserWeakArea>> GetByUserId(long userId);
        Task<List<UserWeakArea>> GetByCourseId(long userId, long courseId);
        Task<List<UserWeakArea>> GetTopWeakAreas(long userId, int topCount);
    }
}

