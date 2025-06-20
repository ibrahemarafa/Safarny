using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace APIs_Graduation.Controllers
{
    [Route("api/planMyTrip")]
    [ApiController]
    public class PlanMyTripController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PlanMyTripController> _logger;

        public PlanMyTripController(ApplicationDbContext context, ILogger<PlanMyTripController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("generateEgyptTrip")]
        public async Task<IActionResult> GenerateEgyptTrip([FromBody] AITripRequestDTO request)
        {
            try
            {
                var parsed = await ParseEgyptianPromptAsync(request.Prompt);

                if (string.IsNullOrEmpty(parsed.Location))
                    return BadRequest("Please specify a valid location in Egypt");

                // تصفية الأماكن السياحية حسب المدخلات
                var placesQuery = _context.TouristPlaces
                    .Include(p => p.City)
                    .Where(p => p.City.Name.ToLower() == parsed.Location.ToLower());

                if (!string.IsNullOrEmpty(parsed.Interest))
                    placesQuery = placesQuery.Where(p => p.Category.ToLower() == parsed.Interest.ToLower());

                if (parsed.MaxPrice.HasValue)
                    placesQuery = placesQuery.Where(p => p.Price <= parsed.MaxPrice);

                var places = await placesQuery
                    .OrderByDescending(p => p.Rate)
                    .Take(10)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Category,
                        Price = p.Price,
                        p.Address,
                        p.PictureUrl,
                    })
                    .ToListAsync();

                var processedPlaces = places.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Category,
                    FormattedPrice = p.Price == 0 ? "Free" : $"{p.Price} EGP",
                    p.Address,
                    p.PictureUrl,
                }).ToList();

                // جلب الفنادق إن طلب المستخدم شمل الفنادق
                var hotelList = new List<object>();
                if (parsed.IncludeHotels)
                {
                    hotelList = await _context.Hotels
                        .Where(h => h.City.Name.ToLower() == parsed.Location.ToLower())
                        .OrderByDescending(h => h.Rate)
                        .Take(5)
                        .Select(h => new
                        {
                            h.Id,
                            h.Name,
                            h.Address,
                            StartPrice = h.StartPrice == 0 ? "Free" : $"{h.StartPrice} EGP",
                            h.Rate,
                            h.PictureUrl,
                        })
                        .ToListAsync<object>();
                }

                return Ok(new
                {
                    Success = true,
                    Location = parsed.Location,
                    Places = processedPlaces,
                    Hotels = hotelList,
                    TotalCost = processedPlaces
                        .Where(p => p.FormattedPrice != "Free")
                        .Sum(p => decimal.Parse(p.FormattedPrice.Replace(" EGP", "")))
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Egypt trip");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error processing your request"
                });
            }
        }

        private async Task<SimplePrompt> ParseEgyptianPromptAsync(string prompt)
        {
            var lowerPrompt = prompt.ToLower();
            var result = new SimplePrompt();

            // جلب أسماء المدن مع تحويلها لحروف صغيرة
            var cities = await _context.Cities.Select(c => c.Name.ToLower()).ToListAsync();

            // البحث عن المدينة في النص المدخل
            result.Location = cities.FirstOrDefault(c => lowerPrompt.Contains(c));

            // تعويض بعض المواقع المشهورة في مصر لو ما اتعرفتش المدينة
            if (result.Location == null)
            {
                if (lowerPrompt.Contains("pyramid") || lowerPrompt.Contains("giza"))
                    result.Location = "giza";
                else if (lowerPrompt.Contains("cairo"))
                    result.Location = "cairo";
            }

            result.IncludeHotels = lowerPrompt.Contains("hotel") || lowerPrompt.Contains("stay") || lowerPrompt.Contains("accommodation");

            // البحث عن الاهتمامات
            var interests = new[] { "historical", "cultural", "islamic", "shopping", "religious" };
            result.Interest = interests.FirstOrDefault(i => lowerPrompt.Contains(i));

            // البحث عن ميزانية (بالجنيه أو الدولار)
            var budgetMatch = Regex.Match(lowerPrompt, @"(\d+)\s*(egp|le|egyptian pound)");
            if (!budgetMatch.Success)
                budgetMatch = Regex.Match(lowerPrompt, @"\$(\d+)");

            if (budgetMatch.Success)
                result.MaxPrice = double.Parse(budgetMatch.Groups[1].Value);

            return result;
        }

        // صنف داخلي بسيط لتمثيل بيانات الفلترة
        private class SimplePrompt
        {
            public string Landmark { get; set; }
            public string Location { get; set; }
            public string Interest { get; set; }
            public bool IncludeHotels { get; set; }
            public double? MaxPrice { get; set; }
        }
    }
}
