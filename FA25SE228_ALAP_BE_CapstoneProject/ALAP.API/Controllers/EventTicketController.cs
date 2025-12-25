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
    public class EventTicketController : BaseAPIController
    {
        private readonly IEventTicketBizLogic _biz;

        public EventTicketController(IEventTicketBizLogic biz)
        {
            _biz = biz;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _biz.GetEventTicketById(id);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListByPaging([FromQuery] PagingModel pagingModel, [FromQuery] long eventId)
        {
            try
            {
                var result = await _biz.GetListEventTicketsByPaging(pagingModel, eventId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-my-tickets")]
        [Authorize]
        public async Task<IActionResult> GetMyTickets()
        {
            try
            {
                var result = await _biz.GetMyTickets(UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }


        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdate([FromBody] CreateUpdateEventTicketDto dto)
        {
            try
            {
                var result = await _biz.CreateUpdateEventTicket(dto);
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
                var result = await _biz.DeleteEventTicket(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPost("buy-ticket/{eventId}")]
        [Authorize]
        public async Task<IActionResult> BuyTicket(long eventId)
        {
            try
            {
                var paymentUrl = await _biz.BuyTicket(eventId, UserId);
                return GetSuccess(new { PaymentUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("refund-list")]
        public async Task<IActionResult> GetRefundList([FromQuery] ALAP.Entity.Models.Wapper.RefundFilterModel filter)
        {
            try
            {
                var result = await _biz.GetRefundList(filter);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("update-refund")]
        public async Task<IActionResult> UpdateRefundStatus([FromBody] ALAP.Entity.DTO.Request.UpdateRefundDto dto)
        {
            try
            {
                var result = await _biz.UpdateRefundStatus(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpGet("refund-statistics/{eventId}")]
        public async Task<IActionResult> GetRefundStatistics(long eventId)
        {
            try
            {
                var result = await _biz.GetRefundStatistics(eventId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("refund-statistics-overall")]
        public async Task<IActionResult> GetRefundStatisticsOverall()
        {
            try
            {
                var result = await _biz.GetRefundStatisticsOverall();
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("check-user-ticket/{eventId}")]
        [Authorize]
        public async Task<IActionResult> CheckUserTicket(long eventId)
        {
            try
            {
                var result = await _biz.CheckUserTicket(eventId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
