using PayOS;
using PayOS.Exceptions;
using PayOS.Models.V2.PaymentRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Helper
{
    public static class PayOSHelper
    {
        private static readonly PayOSClient _client;

        // Khởi tạo client một lần, đọc từ ENV (cách khuyến nghị)
        static PayOSHelper()
        {
            _client = new PayOSClient();
        }

        /// <summary>
        /// Tạo link thanh toán (đơn hàng)
        /// </summary>
        public static async Task<CreatePaymentLinkResponse?> CreateOrderAsync(
            long orderCode,
            long amount,
            string description,
            string returnUrl,
            string cancelUrl)
        {
            try
            {
                var request = new CreatePaymentLinkRequest
                {
                    OrderCode = orderCode,
                    Amount = amount,
                    Description = description,
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                };

                var response = await _client.PaymentRequests.CreateAsync(request);
                return response;
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"API Error: {ex.Message} (Code: {ex.ErrorCode}, Status: {ex.StatusCode})");
            }
            catch (PayOSException ex)
            {
                Console.WriteLine($"PayOS Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }

            return null;
        }
    }
}
