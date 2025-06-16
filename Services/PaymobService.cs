using System.Net.Http.Json;
using System.Text.Json;

namespace APIs_Graduation.Services
{
    public class PaymobService
    {
        private readonly HttpClient _httpClient;
        private const string PaymobApiKey = "ZXlKaGJHY2lPaUpJVXpVeE1pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SmpiR0Z6Y3lJNklrMWxjbU5vWVc1MElpd2ljSEp2Wm1sc1pWOXdheUk2TVRBeU9EYzNOaXdpYm1GdFpTSTZJakUzTkRFeU1qazBOekF1TWpBMU5qRTBJbjAuYUtOM18tcHAtQjJXdkhrc05ZNXhpMVBWY0pBNDlOaUFMR2Fjd1lqNWZONG9fRXVqNHJFV3llNFNRcmRpaFA3SkZ5dkxvZENpRmVqbGI1dEtSbGtuRlE=";
        private const int PaymobIntegrationId = 5004741;
        private const string Currency = "EGP";

        public PaymobService()
        {
            _httpClient = new HttpClient();
        }

        // 🔹 استخراج توكن المصادقة
        public async Task<string> GetTokenAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens",
                new { api_key = PaymobApiKey });

            var jsonString = await response.Content.ReadAsStringAsync();
            using (var doc = JsonDocument.Parse(jsonString))
            {
                return doc.RootElement.GetProperty("token").GetString();
            }
        }

        // 🔹 إنشاء طلب
        public async Task<int> CreateOrderAsync(string token, decimal totalPrice)
        {
            var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", new
            {
                auth_token = token,
                amount_cents = (int)(totalPrice * 100),
                currency = Currency,
                items = new object[] { }
            });

            var jsonString = await response.Content.ReadAsStringAsync();
            using (var doc = JsonDocument.Parse(jsonString))
            {
                return doc.RootElement.GetProperty("id").GetInt32();
            }
        }

        // 🔹 استخراج مفتاح الدفع
        public async Task<string> GetPaymentKeyAsync(string token, int orderId, decimal totalPrice)
        {
            var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", new
            {
                auth_token = token,
                amount_cents = (int)(totalPrice * 100),
                order_id = orderId,
                currency = Currency,
                integration_id = PaymobIntegrationId,
                billing_data = new
                {
                    first_name = "User",
                    last_name = "Test",
                    email = "user@test.com",
                    phone_number = "01000000000",
                    apartment = "NA",
                    floor = "NA",
                    street = "NA",
                    building = "NA",
                    city = "NA",
                    country = "NA"
                }
            });

            var jsonString = await response.Content.ReadAsStringAsync();
            using (var doc = JsonDocument.Parse(jsonString))
            {
                return doc.RootElement.GetProperty("token").GetString();
            }
        }
    }
}
