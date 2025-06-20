using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
        [HttpGet("normalSearch/{indexKey}")]
        public async Task<ActionResult<IEnumerable<Dictionary<string, object>>>> Search(string indexKey, [FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("The search query cannot be empty.");
            }

            var results = await _algoliaService.SearchAsync(indexKey, query);

            if (results == null || !results.Any())
            {
                return NotFound("No results found.");
            }

            return Ok(results);
        }

        [HttpPost("chatWithAI")]
        public async Task<IActionResult> GetAnswer([FromBody] string userQuery)
        {
            if (string.IsNullOrEmpty(userQuery))
            {
                return BadRequest("Query cannot be empty.");
            }

            var result = await _deepSeekService.GetAnswerFromDeepSeek(userQuery);

            if (result.Contains("Error") || result.Contains("Exception"))
            {
                return StatusCode(500, result);
            }

            return Ok(new { answer = result });
        }
    }
}
