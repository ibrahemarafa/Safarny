using APIs_Graduation.Data;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagePlansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PackagePlansController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        [HttpGet("{packageId:int}")]
        public async Task<ActionResult<IEnumerable<PackagePlan>>> GetPlansByPackage(int packageId)
        {
            var plans = await _context.PackagePlans
                .Where(p => p.PackageId == packageId)
                .ToListAsync();

            if (!plans.Any())
            {
                return NotFound("No plans found for this package.");
            }

            return Ok(plans);
        }


        [HttpPost("Add_Plan")]
        public async Task<ActionResult> AddPlan([FromBody] PackagePlan plan)
        {
            if (plan == null || plan.PackageId == 0)
            {
                return BadRequest("Invalid plan data.");
            }

            bool packageExists = await _context.Packages.AnyAsync(p => p.PackageId == plan.PackageId);
            if (!packageExists)
            {
                return NotFound($"Package with ID {plan.PackageId} does not exist.");
            }

            _context.PackagePlans.Add(plan);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                plan.Id,
                plan.Title,
                plan.Description,
                plan.PackageId
            });
        }


    }

}
