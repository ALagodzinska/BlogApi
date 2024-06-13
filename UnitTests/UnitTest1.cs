using BlogApi;

namespace UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(150,50,3312,1104)]
        [TestCase(1000, 2015, 547, 1104)]
        [TestCase(1000, 15000, 250, 3750)]
        public void Calculate_BackgroundImageResize_ReturnExpectedResult(int height, int width, int expectedHeight, int expectedWidth)
        {
            (int actualHeight, int actualWidth ) = ImageConversion.CalculateBackgroundImageSize(height, width);

            Assert.That(actualHeight, Is.EqualTo(expectedHeight), "wRONG hEIGHT");
            Assert.That(actualWidth, Is.EqualTo(expectedWidth), "wRONG WIDTH");
        }

        [Test]
        [TestCase(200, 600, 300, 900)]        
        [TestCase(50, 50, 300, 300)]
        [TestCase(700, 600, 300, 257)]
        [TestCase(700, 100, 1750, 250)]
        [TestCase(958, 639, 374, 250)]
        public void Calculate_PreviewImageResize_ReturnExpectedResult(int height, int width, int expectedHeight, int expectedWidth)
        {
            (int actualHeight, int actualWidth) = ImageConversion.CalculatePreviewImageSize(height, width);

            Assert.That(actualHeight, Is.EqualTo(expectedHeight), "wRONG hEIGHT");
            Assert.That(actualWidth, Is.EqualTo(expectedWidth), "wRONG WIDTH");
        }
    }
}