using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public CitiesController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [HttpPost("add-city")]
        public async Task<IActionResult> AddCities([FromBody] List<City> cities)
        {
            if (cities == null || !cities.Any())
            {
                return BadRequest("Invalid input data");
            }

            await context.Cities.AddRangeAsync(cities);
            await context.SaveChangesAsync();

            return Ok("Cities added successfully");

        }
       
        
        [HttpGet("all-cities")]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await context.Cities.Select(c => new
            {
                c.Id,
                c.Name,
                PictureUrl = $"http://safarny.runasp.net/City/{Uri.EscapeDataString(c.PictureUrl.Replace("City/", ""))}",
                c.type
            }).ToListAsync();
            return Ok(cities);
        }

        [HttpGet("filter-city")]
        public async Task<IActionResult> GetCitiesByType([FromQuery] string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return BadRequest("Type parameter is required.");
            }

            var cities = await context.Cities
                .Where(c => c.type.Contains(type))
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    PictureUrl = $"http://safarny.runasp.net/City/{Uri.EscapeDataString(c.PictureUrl.Replace("City/", ""))}",
                    c.type
                })
                .ToListAsync();

            return Ok(cities);
        }


       
    }
}
