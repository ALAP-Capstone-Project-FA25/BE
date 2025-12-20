using PayOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ALAP.BLL.Helper
{
    public sealed class PaymentItem
    {
        [JsonPropertyName("name")] public string Name { get; set; } = default!;
        [JsonPropertyName("quantity")] public int Quantity { get; set; }
        [JsonPropertyName("price")] public int Price { get; set; }   // VND (đơn vị nhỏ nhất)
    }

    public sealed class CreatePaymentRequest
    {
        [JsonPropertyName("amount")] public int Amount { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; } = default!;
        [JsonPropertyName("orderCode")] public long OrderCode { get; set; }      // sẽ auto set nếu = 0
        [JsonPropertyName("cancelUrl")] public string CancelUrl { get; set; } = default!;
        [JsonPropertyName("returnUrl")] public string ReturnUrl { get; set; } = default!;
        [JsonPropertyName("buyerName")] public string? BuyerName { get; set; }
        [JsonPropertyName("buyerCompanyName")] public string? BuyerCompanyName { get; set; }
        [JsonPropertyName("buyerTaxCode")] public string? BuyerTaxCode { get; set; }
        [JsonPropertyName("buyerEmail")] public string? BuyerEmail { get; set; }
        [JsonPropertyName("buyerPhone")] public string? BuyerPhone { get; set; }
        [JsonPropertyName("buyerAddress")] public string? BuyerAddress { get; set; }
        [JsonPropertyName("items")] public List<PaymentItem> Items { get; set; } = new();

        [JsonPropertyName("expiredAt")] public long? ExpiredAt { get; set; }     
        [JsonPropertyName("signature")] public string? Signature { get; set; }   
    }

    public sealed class ApiResponse<T>
    {
        [JsonPropertyName("code")] public string? Code { get; set; }
        [JsonPropertyName("desc")] public string? Desc { get; set; }
        [JsonPropertyName("data")] public T? Data { get; set; }
        [JsonPropertyName("signature")] public string? Signature { get; set; }
    }

    public sealed class PaymentCreated
    {
        [JsonPropertyName("paymentLinkId")] public string PaymentLinkId { get; set; } = default!;
        [JsonPropertyName("status")] public string Status { get; set; } = default!;
        [JsonPropertyName("checkoutUrl")] public string CheckoutUrl { get; set; } = default!;
        [JsonPropertyName("qrCode")] public string QrCode { get; set; } = default!;
        [JsonPropertyName("orderCode")] public long OrderCode { get; set; }
        [JsonPropertyName("amount")] public int Amount { get; set; }
    }

    public sealed class PayOsClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly string _checksumKey;

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public PayOsClient(string apiMerchantHost, string clientId, string apiKey, string checksumKey,
                           HttpMessageHandler? handler = null, TimeSpan? timeout = null)
        {
            _checksumKey = checksumKey;

            handler ??= new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            };

            _http = new HttpClient(handler, disposeHandler: true)
            {
                BaseAddress = new Uri(apiMerchantHost.TrimEnd('/') + "/"),
                Timeout = timeout ?? TimeSpan.FromSeconds(30)
            };

            _http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _http.DefaultRequestHeaders.Add("x-client-id", clientId);
            _http.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }

        private static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        private static long NowSec() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        private static string HmacSha256Hex(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2")); // hex lowercase
            return sb.ToString();
        }

        /// <summary>
        /// </summary>
        private string CreateSignatureBrowser(CreatePaymentRequest body)
        {
            var raw = $"amount={body.Amount}" +
                      $"&cancelUrl={body.CancelUrl}" +
                      $"&description={body.Description}" +
                      $"&orderCode={body.OrderCode}" +
                      $"&returnUrl={body.ReturnUrl}";
            return HmacSha256Hex(_checksumKey, raw);
        }

        public async Task<ApiResponse<PaymentCreated>> CreatePaymentAsync(
            CreatePaymentRequest body, CancellationToken ct = default)
        {
            if (body.OrderCode == 0)
                body.OrderCode = NowMs();             

            body.ExpiredAt ??= NowSec() + 3600;     

            body.Signature = CreateSignatureBrowser(body);

            var content = new StringContent(JsonSerializer.Serialize(body, JsonOpts), Encoding.UTF8, "application/json");
            using var res = await _http.PostAsync("v2/payment-requests", content, ct);
            var payload = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
                throw new HttpRequestException($"CreatePayment failed ({(int)res.StatusCode}): {payload}");

            var parsed = JsonSerializer.Deserialize<ApiResponse<PaymentCreated>>(payload, JsonOpts)
                         ?? throw new InvalidOperationException("Empty response");
            return parsed;
        }

        public void Dispose() => _http.Dispose();
    }


}
