using ALAP.BLL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models.Wapper;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : BaseAPIController
    {
        private readonly IPackageBizLogic _packageBizLogic;

        public PackageController(IPackageBizLogic packageBizLogic)
        {
            _packageBizLogic = packageBizLogic;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackageById(long id)
        {
            try
            {
                var result = await _packageBizLogic.GetPackageById(id);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListPackagesByPaging([FromQuery] PagingModel pagingModel)
        {
            try
            {
                var result = await _packageBizLogic.GetListPackagesByPaging(pagingModel);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdatePackage([FromBody] CreateUpdatePackageDto dto)
        {
            try
            {
                var result = await _packageBizLogic.CreateUpdatePackage(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePackage(long id)
        {
            try
            {
                var result = await _packageBizLogic.DeletePackage(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }


        [HttpPost("buy-package/{packageId}")]
        [Authorize]
        public async Task<IActionResult> BuyPackage(long packageId)
        {
            try
            {
                var paymentUrl = await _packageBizLogic.BuyPackage(packageId, UserId);
                // Implementation for buying a package goes here

                return GetSuccess(new
                {
                    PaymentUrl = paymentUrl
                });
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
