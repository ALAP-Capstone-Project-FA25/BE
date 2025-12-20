using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using AutoMapper;
using Base.Common;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class CategoryBizLogic : ICategoryBizLogic
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryBizLogic(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateUpdateCategory(CreateUpdateCategoryDto dto)
        {
            if(dto.Id > 0)
            {
                var existingCategory = await _categoryRepository.GetById(dto.Id);
                if (existingCategory == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy danh mục.");
                }

                existingCategory.Name = dto.Name;
                existingCategory.Description = dto.Description;
                existingCategory.ImageUrl = dto.ImageUrl;
                existingCategory.MajorId = dto.MajorId;
                existingCategory.UpdatedAt = Utils.GetCurrentVNTime();

                return await _categoryRepository.Update(existingCategory);
            }
            else
            {
                var categoryModel = new CategoryModel
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl,
                    MajorId = dto.MajorId,
                };
                return await _categoryRepository.Create(categoryModel);
            }
        }

        public async Task<bool> DeleteCategory(long id)
        {
            return await _categoryRepository.Delete(id);
        }

        public async Task<CategoryModel> GetCategoryById(long id)
        {
            return await _categoryRepository.GetById(id) ?? throw new KeyNotFoundException($"Không tìm thấy danh mục.");
        }

        public async Task<PagedResult<CategoryDto>> GetListCategoriesByPaging(PagingModel pagingModel)
        {
            var model = await _categoryRepository.GetListByPaging(pagingModel);
        
            var dtoList = model.ListObjects.Select(category => _mapper.Map<CategoryDto>(category)).ToList();
            return new PagedResult<CategoryDto>(dtoList, model.TotalRecords, model.PageNumber, model.PageSize);
        }
    }
}
