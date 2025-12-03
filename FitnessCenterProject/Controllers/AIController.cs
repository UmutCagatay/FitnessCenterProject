using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FitnessCenterProject.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AIController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePlan(int yas, int kilo, int boy, string hedef)
        {
            string apiKey = _configuration["GeminiApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                return Json(new { success = false, message = "API Anahtarı bulunamadı." });
            }

            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            string prompt = $"Danışan Profili: {yas} yaşında, {kilo} kg ağırlığında, {boy} cm boyunda. Hedef: {hedef}. " +
                            $"Bu kişi için haftalık egzersiz ve beslenme tavsiyesi hazırla. " +
                            $"Kurallar: " +
                            $"1. Asla 'Merhaba Ben' deme. Direkt konuya gir. " +
                            $"2. Cevabı saf HTML formatında ver (Başlıklar için <h4>, kalın yazılar için <strong>, listeler için <ul> ve <li> etiketlerini kullan). " +
                            $"3. Asla Markdown sembolleri kullanma. Sadece HTML. " +
                            $"4. Kod bloğu (```) içine alma.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = prompt } }
                    }
                }
            };

            string jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(apiUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseString);
                    string aiResult = geminiResponse?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "Cevap alınamadı.";
                    aiResult = aiResult.Replace("```html", "").Replace("```", "");
                    return Json(new { success = true, message = aiResult });
                }
                else
                {
                    return Json(new { success = false, message = $"Hata Kodu: {response.StatusCode}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bağlantı hatası: " + ex.Message });
            }
        }
    }

    public class GeminiApiResponse { [JsonPropertyName("candidates")] public Candidate[] Candidates { get; set; } }
    public class Candidate { [JsonPropertyName("content")] public Content Content { get; set; } }
    public class Content { [JsonPropertyName("parts")] public Part[] Parts { get; set; } }
    public class Part { [JsonPropertyName("text")] public string Text { get; set; } }
}