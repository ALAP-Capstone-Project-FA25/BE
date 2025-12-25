using ALAP.BLL.Interface;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class MajorBizLogic : AppBaseBizLogic, IMajorBizLogic
    {
        private readonly IMajorRepository _majorRepository;
        private readonly BaseDBContext _dbContext;
        public MajorBizLogic(BaseDBContext dbContext, IMajorRepository majorRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _majorRepository = majorRepository;
        }



        public async Task<bool> CreateUpdateMajor(CreateUpdateMajorDto dto)
        {
            if (dto.Id > 0)
            {
                var model = new MajorModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description ?? string.Empty,
                    UpdatedAt = Utils.GetCurrentVNTime(),
                };
                return await _majorRepository.Update(model);
            }
            else
            {
                var model = new MajorModel
                {
                    Name = dto.Name,
                    Description = dto.Description ?? string.Empty
                };
                return await _majorRepository.Create(model);
            }
        }

        public async Task<bool> DeleteMajor(long id)
        {
            return await _majorRepository.Delete(id);
        }

        public async Task<MajorModel> GetMajorById(long id)
        {
            return await _majorRepository.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy ngành học.");
        }

        public async Task<PagedResult<MajorModel>> GetListMajorsByPaging(PagingModel pagingModel)
        {
            return await _majorRepository.GetListByPaging(pagingModel);
        }

        public async Task<bool> UpdateUserMajor(long userId, long majorId)
        {
            var existingMajor = await _majorRepository.GetById(majorId);
            var existingUser = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (existingMajor == null)
            {
                throw new KeyNotFoundException("Ngành học không tồn tại.");
            }
            if (existingUser == null)
            {
                throw new KeyNotFoundException("Người dùng không tồn tại.");
            }
            existingUser.MajorId = majorId;
            _dbContext.Users.Update(existingUser);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}

