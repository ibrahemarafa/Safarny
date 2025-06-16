using APIs_Graduation.Data;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageInclusionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PackageInclusionsController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        [HttpGet("{packageId:int}")]
        public async Task<ActionResult<object>> GetPackageDetails(int packageId)
        {
            var package = await _context.Packages
                .Include(p => p.PackageInclusions)
                .Include(p => p.PackageExclusions)
                .FirstOrDefaultAsync(p => p.PackageId == packageId);

            if (package == null)
                return NotFound($"Package with ID {packageId} not found.");

            return Ok(new
            {
                package.PackageId,
                package.Name,
                Inclusions = package.PackageInclusions.Select(i => new { i.Id, i.Name }),
                Exclusions = package.PackageExclusions.Select(e => new { e.Id, e.Name })
            });
        }

       
        [HttpPost("Add_Inclusions/{packageId}")]
        public async Task<ActionResult> AddInclusions(int packageId, [FromBody] List<string> inclusionNames)
        {
            if (inclusionNames == null || !inclusionNames.Any())
                return BadRequest("At least one inclusion must be provided.");

            var package = await _context.Packages.FindAsync(packageId);
            if (package == null)
                return NotFound($"Package with ID {packageId} not found.");

            var inclusions = inclusionNames.Select(name => new PackageInclusion { Name = name, PackageId = packageId }).ToList();
            _context.PackageInclusions.AddRange(inclusions);
            await _context.SaveChangesAsync();

            return Ok("Inclusions added successfully.");
        }

    
        [HttpPost("Add_Exclusions/{packageId}")]
        public async Task<ActionResult> AddExclusions(int packageId, [FromBody] List<string> exclusionNames)
        {
            if (exclusionNames == null || !exclusionNames.Any())
                return BadRequest("At least one exclusion must be provided.");

            var package = await _context.Packages.FindAsync(packageId);
            if (package == null)
                return NotFound($"Package with ID {packageId} not found.");

            var exclusions = exclusionNames.Select(name => new PackageExclusion { Name = name, PackageId = packageId }).ToList();
            _context.PackageExclusions.AddRange(exclusions);
            await _context.SaveChangesAsync();

            return Ok("Exclusions added successfully.");
        }
    }
}
