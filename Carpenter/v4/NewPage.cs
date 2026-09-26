using System.Collections.Generic;

namespace Carpenter
{
    public class PageImage
    {
        public string Filename;
        public int Width;
        public int Height;
    }

    public class NewPage
    {
        public string Title;
        public string Url;
        public string ThumbnailFilename;
        public string Location;
        public string Month;
        public string Year;
        public string Description;
        public PageImage HeaderImage;
        public List<PageImage> Images = new();

        public string InsertImageElement(PageImage image)
        {
            return $"<a id=\"grid-image\" class=\"glightbox\" href=\"{Url}/{image.Filename}\">\r\n    <img class=\"base-image\" src=\"{Url}/{image.Filename}\" height=\"{image.Height}\" width=\"{image.Width}\" loading=\"lazy\"> \r\n</a>"
        }
    }
}
