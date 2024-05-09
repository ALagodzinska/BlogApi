namespace BlogApi.Models
{
    public class PostImage
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public string Format { get; set; }
        public string Type { get; set; }
    }
}
