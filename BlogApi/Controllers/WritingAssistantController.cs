using BlogApi.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BlogApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WritingAssistantController : ControllerBase
    {
        private readonly AiSettings _settings;
        private readonly HttpClient _httpClient;

        public WritingAssistantController(HttpClient httpClient, IOptions<AiSettings> aiSettings)
        {
            _settings = aiSettings.Value;
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult TestApiKey()
        {
            // Do NOT return the real key in production.
            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                return BadRequest("Gemini API key is missing.");
            }

            return Ok("Gemini API key is loaded.");
        }

        [HttpGet]
        public async Task<IActionResult> GetFeedback(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Content is required.");

            var prompt = $$"""
            You are a writing assistant for a blog application.

            Analyze the following blog content and provide improvement-focused feedback.

            Tone and style:
            - Be friendly, supportive, and down-to-earth
            - Use simple, easy-to-understand language
            - Sound like a helpful mentor, not a strict critic
            - Be honest, but never harsh or overly formal
            - Keep the tone motivating and constructive
            - If there are formatting issues, describe them in plain language (for example: awkward spacing, abrupt line breaks, or paragraphs that feel too long or too short).
            - Do not reference or use any technical or markup terms.

            Requirements:
            1. Write a concise feedback summary, maximum 100 words.
               - The summary should explain the main improvements the writer should make.
               - Do NOT summarize what the blog post is about.
               - Focus on writing quality, clarity, structure, readability, and missing details.

            2. Provide 5 to 7 specific suggestions for improvement.

            Guidelines for suggestions:
            - Focus on clarity, structure, readability, and usefulness
            - The "issue" must be short (max 7 words)
            - Each suggestion must be no longer than two sentences
            - Use simple, direct language
            - Give concrete, actionable suggestions
            - Avoid generic advice
            - Avoid explaining obvious benefits (e.g., "this improves readability")
            - Do not repeat the same type of suggestion
            - Mention a paragraph only if clearly relevant            

            IMPORTANT:
            - Return ONLY valid JSON
            - Do NOT include extra text, explanations, or markdown formatting
            - Do NOT wrap the response in markdown
            - Do NOT use ```json
            - Do NOT include extra text
            - Follow this exact JSON structure:

            {
              "summary": "string",
              "suggestions": [
                {
                  "issue": "string",
                  "suggestion": "string"
                }
              ]
            }

            Content:
            {{content}}
            """;

            // Gemini API format
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var url =
             $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent";

            HttpResponseMessage? response = null;

            for (int i = 0; i < 3; i++)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("x-goog-api-key", _settings.ApiKey);
                request.Content = JsonContent.Create(requestBody);

                response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    break;

                if ((int)response.StatusCode == 503 || (int)response.StatusCode == 429)
                    await Task.Delay(1000 * (i + 1));
                else
                    break;
            }

            if (response == null)
            {
                return StatusCode(500, "No response from AI service.");
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, error);
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            var feedback = json
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return Ok(feedback);
        }
    }
}
