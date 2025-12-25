using ALAP.BLL.Interface;
using Base.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : BaseAPIController
    {
        private readonly ISearchBizLogic _searchBizLogic;

        public SearchController(ISearchBizLogic searchBizLogic)
        {
            _searchBizLogic = searchBizLogic;
        }

        [HttpGet("all")]
        public async Task<IActionResult> SearchAll([FromQuery] string keyword, [FromQuery] int limit = 10)
        {
            try
            {
                var result = await _searchBizLogic.SearchAll(keyword ?? string.Empty, limit);
                return GetSuccess(result);
            }
            catch (System.Exception ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
