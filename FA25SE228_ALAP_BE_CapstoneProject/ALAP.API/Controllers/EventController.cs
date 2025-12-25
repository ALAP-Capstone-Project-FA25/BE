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
    public class EventController : BaseAPIController
    {
        private readonly IEventBizLogic _biz;

        public EventController(IEventBizLogic biz)
        {
            _biz = biz;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _biz.GetEventById(id);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListByPaging([FromQuery] PagingModel pagingModel)
        {
            try
            {
                var result = await _biz.GetListEventsByPaging(pagingModel);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdate([FromBody] CreateUpdateEventDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate that end date is after start date
                if (dto.EndDate <= dto.StartDate)
                {
                    ModelState.AddModelError("EndDate", "The end date must be after the start date.");
                    return BadRequest(ModelState);
                }

                var result = await _biz.CreateUpdateEvent(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _biz.DeleteEvent(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPost("send-commission/{eventId}")]
        public async Task<IActionResult> SendCommissionToSpeaker(long eventId, [FromBody] SendCommissionDto dto)
        {
            try
            {
                var result = await _biz.SendCommissionToSpeaker(eventId, dto.PaymentProofImageUrl);
                return SaveSuccess(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPost("cancel/{eventId}")]
        public async Task<IActionResult> CancelEvent(long eventId)
        {
            try
            {
                var result = await _biz.CancelEvent(eventId);
                return GetSuccess(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
