using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly AlgoliaService _algoliaService;
        private readonly DeepSeekService _deepSeekService;

        public SearchController(AlgoliaService algoliaService, DeepSeekService deepSeekService)
        {
            _algoliaService = algoliaService;
            _deepSeekService = deepSeekService;
        }

        // نقطة النهاية للبحث في Algolia
        [HttpGet("NormalSearch/{indexKey}")]
        public async Task<ActionResult<IEnumerable<Dictionary<string, object>>>> Search(string indexKey, [FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("The search query cannot be empty.");
            }

            // استدعاء الخدمة لتنفيذ البحث في Algolia
            var results = await _algoliaService.SearchAsync(indexKey, query);

            // التأكد من وجود نتائج
            if (results == null || !results.Any())
            {
                return NotFound("No results found.");
            }

            return Ok(results); // إرجاع النتائج
        }

        [HttpPost("ChatWithAI")]
        public async Task<IActionResult> GetAnswer([FromBody] string userQuery)
        {
            if (string.IsNullOrEmpty(userQuery))
            {
                return BadRequest("Query cannot be empty.");
            }

            var result = await _deepSeekService.GetAnswerFromDeepSeek(userQuery);

            if (result.Contains("Error") || result.Contains("Exception"))
            {
                return StatusCode(500, result); // Internal server error
            }

            return Ok(new { answer = result });
        }
    }
}