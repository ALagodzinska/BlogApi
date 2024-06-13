namespace BlogApi
{
    public static class Constants
    {
        public static int PreviewImageWidth => 250;
        public static int PreviewImageHeight => 300;
        public static int BackgroundImageWidth => 1104;
        public static int BackgroundImageHeight => 250;

        public static string BackgroundImageType = "background";

        public static string PreviewImageType = "preview";
        public enum ImageType {
            Background,
            Preview
        }
    }
}
