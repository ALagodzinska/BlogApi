using BlogApi.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlogApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WritingAssistantController : ControllerBase
    {
        private readonly string _apiKey;

        public WritingAssistantController(IOptions<AiSettings> aiSettings)
        {
            _apiKey = aiSettings.Value.OpenAiKey;
        }

        [HttpGet]
        public IActionResult TestApiKey()
        {
            // Do NOT return the real key in production.
            if (string.IsNullOrEmpty(_apiKey))
            {
                return BadRequest("Gemini API key is missing.");
            }

            return Ok("Gemini API key is loaded.");
        }
    }
}
