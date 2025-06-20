using APIs_Graduation.Data;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs_Graduation.Controllers
{
    [Route("api/packages")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PackagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/packages/allPackages
        [HttpGet("allPackages")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllPackages()
        {
            var packages = await _context.Packages
                .Include(p => p.PackagePlans)
                .Include(p => p.PackageInclusions)
                .Include(p => p.PackageExclusions)
                .Select(p => new
                {
                    p.PackageId,
                    p.Name,
                    p.Description,
                    p.Duration,
                    p.Price,
                    p.Category,
                    ImageUrl = $"http://safarny.runasp.net/Packages/{Uri.EscapeDataString(p.ImageUrl.Replace("Packages/", ""))}",
                    p.CompanyName,
                    p.FacebookPage,
                    PackagePlans = p.PackagePlans.Select(plan => new { plan.Id, plan.Title, plan.Description }),
                    PackageInclusions = p.PackageInclusions.Select(inc => new { inc.Id, inc.Name }),
                    PackageExclusions = p.PackageExclusions.Select(exc => new { exc.Id, exc.Name })
                }).ToListAsync();

            if (!packages.Any())
            {
                return NotFound("No packages found.");
            }

            return Ok(packages);
        }

        // GET: api/packages/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetPackageById(int id)
        {
            var package = await _context.Packages
                .Where(p => p.PackageId == id)
                .Include(p => p.PackagePlans)
                .Include(p => p.PackageInclusions)
                .Include(p => p.PackageExclusions)
                .Select(p => new
                {
                    p.PackageId,
                    p.Name,
                    p.Description,
                    p.Duration,
                    p.Price,
                    p.Category,
                    ImageUrl = $"http://safarny.runasp.net/Packages/{Uri.EscapeDataString(p.ImageUrl.Replace("Packages/", ""))}",
                    p.CompanyName,
                    p.FacebookPage,
                    PackagePlans = p.PackagePlans.Select(plan => new { plan.Id, plan.Title, plan.Description }),
                    PackageInclusions = p.PackageInclusions.Select(inc => new { inc.Id, inc.Name }),
                    PackageExclusions = p.PackageExclusions.Select(exc => new { exc.Id, exc.Name })
                })
                .FirstOrDefaultAsync();

            if (package == null)
            {
                return NotFound($"Package with ID {id} not found.");
            }

            return Ok(package);
        }

        // POST: api/packages/addPackage
        [HttpPost("addPackage")]
        public async Task<ActionResult<Package>> AddPackage([FromBody] Package package)
        {
            if (package == null)
            {
                return BadRequest("Package data is required.");
            }

            bool exists = await _context.Packages.AnyAsync(p => p.Name == package.Name);
            if (exists)
            {
                return Conflict($"A package with the name '{package.Name}' already exists.");
            }

            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPackageById), new { id = package.PackageId }, package);
        }

        // POST: api/packages/addPackagesBulk
        [HttpPost("addPackagesBulk")]
        public async Task<ActionResult<IEnumerable<Package>>> AddPackagesBulk([FromBody] List<Package> packages)
        {
            if (packages == null || !packages.Any())
            {
                return BadRequest("Package list is required.");
            }

            var existingNames = await _context.Packages.Select(p => p.Name).ToListAsync();
            var duplicates = packages.Where(p => existingNames.Contains(p.Name)).Select(p => p.Name).ToList();

            if (duplicates.Any())
            {
                return Conflict($"The following packages already exist: {string.Join(", ", duplicates)}");
            }

            _context.Packages.AddRange(packages);
            await _context.SaveChangesAsync();

            return Ok(packages);
        }

        // PUT: api/packages/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePackage(int id, [FromBody] Package updatedPackage)
        {
            if (updatedPackage == null || id != updatedPackage.PackageId)
            {
                return BadRequest("Invalid package data.");
            }

            var existingPackage = await _context.Packages
                .Include(p => p.PackagePlans)
                .Include(p => p.PackageInclusions)
                .Include(p => p.PackageExclusions)
                .FirstOrDefaultAsync(p => p.PackageId == id);

            if (existingPackage == null)
            {
                return NotFound($"Package with ID {id} not found.");
            }

            existingPackage.Name = updatedPackage.Name;
            existingPackage.Description = updatedPackage.Description;
            existingPackage.Duration = updatedPackage.Duration;
            existingPackage.Price = updatedPackage.Price;
            existingPackage.Category = updatedPackage.Category;
            existingPackage.ImageUrl = updatedPackage.ImageUrl;
            existingPackage.CompanyName = updatedPackage.CompanyName;
            existingPackage.FacebookPage = updatedPackage.FacebookPage;

            _context.PackagePlans.RemoveRange(existingPackage.PackagePlans);
            existingPackage.PackagePlans = updatedPackage.PackagePlans;

            _context.PackageInclusions.RemoveRange(existingPackage.PackageInclusions);
            existingPackage.PackageInclusions = updatedPackage.PackageInclusions;

            _context.PackageExclusions.RemoveRange(existingPackage.PackageExclusions);
            existingPackage.PackageExclusions = updatedPackage.PackageExclusions;

            await _context.SaveChangesAsync();

            return Ok($"Package with ID {id} has been updated successfully.");
        }

        // DELETE: api/packages/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var package = await _context.Packages
                .Include(p => p.PackagePlans)
                .Include(p => p.PackageInclusions)
                .Include(p => p.PackageExclusions)
                .FirstOrDefaultAsync(p => p.PackageId == id);

            if (package == null)
            {
                return NotFound($"Package with ID {id} not found.");
            }

            var packageImages = _context.Package_Images.Where(img => img.PackageId == id);
            _context.Package_Images.RemoveRange(packageImages);

            _context.PackagePlans.RemoveRange(package.PackagePlans);
            _context.PackageInclusions.RemoveRange(package.PackageInclusions);
            _context.PackageExclusions.RemoveRange(package.PackageExclusions);
            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();

            return Ok($"Package with ID {id} and its images have been deleted.");
        }

        // GET: api/packages/{id}/images
        [HttpGet("{id:int}/images")]
        public async Task<ActionResult<IEnumerable<string>>> GetPackageImages(int id)
        {
            var images = await _context.Package_Images
                .Where(img => img.PackageId == id)
                .Select(img => $"http://safarny.runasp.net/Packages/{Uri.EscapeDataString(img.PictureUrl)}")
                .ToListAsync();

            if (!images.Any())
            {
                return NotFound($"No images found for package ID {id}.");
            }

            return Ok(images);
        }

        // POST: api/packages/{id}/addImages
        [HttpPost("{id:int}/addImages")]
        public async Task<IActionResult> AddPackageImages(int id, [FromBody] List<string> imageUrls)
        {
            if (imageUrls == null || !imageUrls.Any())
            {
                return BadRequest("Image list is required.");
            }

            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound($"Package with ID {id} not found.");
            }

            var images = imageUrls.Select(url => new Package_Images
            {
                PackageId = id,
                PictureUrl = url
            }).ToList();

            _context.Package_Images.AddRange(images);
            await _context.SaveChangesAsync();

            return Ok($"Images added to package ID {id}.");
        }
    }
}
