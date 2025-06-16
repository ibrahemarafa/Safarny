using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using APIs_Graduation.Migrations;
using APIs_Graduation.Models;
using APIs_Graduation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanMyTripController : ControllerBase
    {

       // private readonly HuggingFaceService _huggingFaceService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PlanMyTripController> _logger;

        //public PlanMyTripController(HuggingFaceService huggingFaceService, ApplicationDbContext context,ILogger<PlanMyTripController>logger)
        //{
        //    _huggingFaceService = huggingFaceService;
        //    _context = context;
        //    _logger = logger;
        //}





        

        private IQueryable<TouristPlaces> ApplyPlaceFilters(
     IQueryable<TouristPlaces> query,
     SimplePrompt filters)
        {
            if (!string.IsNullOrEmpty(filters.Landmark))
            {
                // Exact match for specific landmarks
                query = query.Where(p => p.Name.ToLower().Contains(filters.Landmark));
            }
            else if (!string.IsNullOrEmpty(filters.Interest))
            {
                // Category or tag matching
                query = query.Where(p =>
                    p.Category.Equals(filters.Interest, StringComparison.OrdinalIgnoreCase));
                    
            }

            // Price filtering
            if (filters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filters.MaxPrice);
            }

            return query;
        }


        // Helper method for opening hours
        private string GetOpeningHours(string siteName)
        {
            return siteName switch
            {
                string s when s.Contains("Al-Azhar") => "Daily 9AM-5PM (Closed during prayers)",
                string s when s.Contains("Citadel") => "8AM-4PM (Extended in Ramadan)",
                _ => "Typically 9AM-5PM (Confirm locally)"
            };
        }


       
        private async Task<SimplePrompt> ParseEgyptianPromptAsync(string prompt)
        {
            var lowerPrompt = prompt.ToLower();
            var result = new SimplePrompt();

            // Get all available cities (case insensitive)
            var cities = await _context.Cities
                .Select(c => c.Name.ToLower())
                .ToListAsync();

            // Detect location (case insensitive)
            result.Location = cities.FirstOrDefault(c => lowerPrompt.Contains(c));

            // Special handling for Egyptian landmarks
            if (result.Location == null)
            {
                if (lowerPrompt.Contains("pyramid") || lowerPrompt.Contains("giza"))
                    result.Location = "giza";
                else if (lowerPrompt.Contains("cairo"))
                    result.Location = "cairo";
            }
            result.IncludeHotels = lowerPrompt.Contains("hotel") || lowerPrompt.Contains("stay") || lowerPrompt.Contains("accommodation");


            // Detect interests (case insensitive)
            var interests = new[] { "historical", "cultural", "islamic", "shopping", "religious" };
            result.Interest = interests.FirstOrDefault(i => lowerPrompt.Contains(i));

            // Detect budget
            var budgetMatch = Regex.Match(lowerPrompt, @"(\d+)\s*(egp|le|egyptian pound)");
            if (!budgetMatch.Success)
                budgetMatch = Regex.Match(lowerPrompt, @"\$(\d+)");

            if (budgetMatch.Success)
                result.MaxPrice = double.Parse(budgetMatch.Groups[1].Value);

            return result;
        }



        [HttpPost("generate-egypt-trip")]
        public async Task<IActionResult> GenerateEgyptTrip([FromBody] AITripRequestDTO request)
        {
            try
            {
                var parsed = await ParseEgyptianPromptAsync(request.Prompt);

                if (string.IsNullOrEmpty(parsed.Location))
                    return BadRequest("Please specify a valid location in Egypt");

                // 1. الحصول على الأماكن السياحية
                var placesQuery = _context.TouristPlaces
                    .Include(p => p.City)
                    .Where(p => p.City.Name.ToLower() == parsed.Location.ToLower());

                if (!string.IsNullOrEmpty(parsed.Interest))
                {
                    placesQuery = placesQuery.Where(p =>
                        p.Category.ToLower() == parsed.Interest.ToLower());
                }

                if (parsed.MaxPrice.HasValue)
                {
                    placesQuery = placesQuery.Where(p => p.Price <= parsed.MaxPrice);
                }

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

                // 2. الحصول على الفنادق (الحل المعدل)
                var hotelList = new List<object>(); // استخدمنا object بدلاً من List<object> في التهيئة
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
                        .ToListAsync<object>(); // التحويل الصريح هنا
                }
              

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Location = parsed.Location,
                    Places = processedPlaces,
                    Hotels = hotelList, // لن تكون هناك مشكلة في التحويل الآن
                    TotalCost = processedPlaces.Where(p => p.FormattedPrice != "Free")
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