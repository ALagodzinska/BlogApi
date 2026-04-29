namespace BlogApi.Configuration
{
    public class AiSettings
    {
        public string ApiKey { get; set; }
        public string Model { get; set; } = "gemini-2.5-flash";
    }
}
