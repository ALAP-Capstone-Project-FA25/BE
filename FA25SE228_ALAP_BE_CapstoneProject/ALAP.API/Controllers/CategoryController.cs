using ALAP.BLL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models.Wapper;
using Base.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseAPIController
    {
        private readonly ICategoryBizLogic _categoryBizLogic;

        public CategoryController(ICategoryBizLogic categoryBizLogic)
        {
            _categoryBizLogic = categoryBizLogic;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(long id)
        {
            try
            {
                var category = await _categoryBizLogic.GetCategoryById(id);
                return GetSuccess(category);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListCategoriesByPaging([FromQuery] PagingModel pagingModel)
        {
            try
            {

                var categories = await _categoryBizLogic.GetListCategoriesByPaging(pagingModel);
                return GetSuccess(categories);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
        [HttpGet("get-by-paging-by-major")]
        public async Task<IActionResult> GetListCategoriesByPagingAndMajor([FromQuery] PagingModel pagingModel)
        {
            try
            {
                var categories = await _categoryBizLogic.GetListCategoriesByPaging(pagingModel);
                return GetSuccess(categories);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdateCategory([FromBody] CreateUpdateCategoryDto dto)
        {
            try
            {
                var result = await _categoryBizLogic.CreateUpdateCategory(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            try
            {
                var result = await _categoryBizLogic.DeleteCategory(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
