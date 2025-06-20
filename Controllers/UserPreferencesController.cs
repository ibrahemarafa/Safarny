using APIs_Graduation.Data;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPreferencesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserPreferencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("savePreferences")]
        public async Task<IActionResult> SavePreferences([FromBody] UserPreference preference)
        {
            if (preference == null)
                return BadRequest(new { message = "Invalid preference data." });

            int[] allowedDurations = Enumerable.Range(1, 10).ToArray();
            decimal[] allowedBudgets = Enumerable.Range(1, 20).Select(i => i * 100m).ToArray();
            var errors = new List<string>();

            if (!allowedDurations.Contains(preference.StayDuration))
                errors.Add("Invalid stay duration.");

            if (!allowedBudgets.Contains(preference.Budget))
                errors.Add("Invalid budget.");

            if (!Enum.IsDefined(typeof(Category), preference.CategoryPreference))
                errors.Add("Invalid category preference.");

            if (errors.Any())
                return BadRequest(new { message = "Validation errors", errors });

            var existingPreference = await _context.UserPreferences
                .FirstOrDefaultAsync(p => p.Username.ToLower() == preference.Username.ToLower());

            if (existingPreference != null)
            {
                existingPreference.StayDuration = preference.StayDuration;
                existingPreference.Budget = preference.Budget;
                existingPreference.CategoryPreference = preference.CategoryPreference;
            }
            else
            {
                _context.UserPreferences.Add(preference);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Preferences saved successfully." });
        }

        [HttpGet("getRecommendedPackages/{username}")]
        public async Task<IActionResult> GetRecommendedPackages(string username)
        {
            var preference = await _context.UserPreferences
                .FirstOrDefaultAsync(p => p.Username.ToLower() == username.ToLower());

            if (preference == null)
                return NotFound(new { message = "No preferences found for this user." });

            string categoryPreference = preference.CategoryPreference.ToString().Trim().ToLower();

            var allPackages = await _context.Packages
                .Where(p => p.Category.ToLower() == categoryPreference || p.Price <= preference.Budget)
                .ToListAsync();

            var filteredPackages = allPackages
                .Where(p =>
                    !string.IsNullOrEmpty(p.Duration) &&
                    p.Duration.Split(' ').Length > 0 &&
                    int.TryParse(p.Duration.Split(' ')[0].Trim(), out int duration) &&
                    duration <= preference.StayDuration
                )
                .ToList();

            var recommendedPackages = allPackages
                .Union(filteredPackages)
                .Distinct()
                .Select(p => new
                {
                    p.PackageId,
                    p.Name,
                    p.Description,
                    p.Duration,
                    p.Price,
                    p.Category,
                    p.CompanyName,
                    p.FacebookPage,
                    ImageUrl = $"http://safarny.runasp.net/Packages/{Uri.EscapeDataString(p.ImageUrl.Replace("Packages/", ""))}"
                })
                .ToList();

            if (!recommendedPackages.Any())
                return NotFound(new
                {
                    message = "No suitable packages found.",
                    reason = new
                    {
                        AvailablePackages = allPackages.Count,
                        Category = categoryPreference,
                        Budget = preference.Budget,
                        StayDuration = preference.StayDuration
                    }
                });

            return Ok(recommendedPackages);
        }
    }
}
