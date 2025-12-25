using ALAP.BLL.Interface;
using ALAP.Entity.Models.Enums;
using ALAP.Entity.Models.Wapper;
using Base.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayOS;
using PayOS.Models.Webhooks;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseAPIController
    {
        private readonly PayOSClient _client;
        private readonly IPaymentBizLogic _paymentBizLogic;

        public PaymentController(PayOSClient client, IPaymentBizLogic paymentBizLogic)
        {
            _client = client;
            _paymentBizLogic = paymentBizLogic;
        }

        [HttpPost("hook")]
        public async Task<ActionResult> VerifyPayment([FromBody] Webhook webhook)
        {
            if (webhook == null)
                return Ok(new
                {
                    message = "Webhook processed successfully",
                });

            try
            {
                var order = await _paymentBizLogic.UpdatePaymentByOrderCode(
                    webhook.Data.OrderCode,
                    PaymentStatus.SUCCESS
                );

                return Ok(new
                {
                    message = "Webhook processed successfully",
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    message = "Webhook processed successfully",
                });
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetPaymentByPaging([FromQuery] PagingModel pagingModel, [FromQuery] PaymentStatus? status = null)
        {
            try
            {
                var payments = await _paymentBizLogic.GetPaymentByPaging(pagingModel, status);
                return GetSuccess(payments);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("update-status/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(long id, [FromBody] ALAP.Entity.DTO.Request.UpdatePaymentStatusDto dto)
        {
            try
            {
                var paymentStatus = (PaymentStatus)dto.Status;
                var payment = await _paymentBizLogic.UpdatePaymentStatus(id, paymentStatus);
                return SaveSuccess(payment);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetPaymentStatistics()
        {
            try
            {
                var stats = await _paymentBizLogic.GetPaymentStatistics();
                return GetSuccess(stats);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
