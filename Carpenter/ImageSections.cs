using System;
using System.Collections.Generic;

namespace Carpenter
{
    /// <summary>
    /// Abstract representation of an section of content (can be image, text or title) that is used in a 
    /// SCHEMA page layout.
    /// </summary>
    public abstract class Section : IEquatable<Section>
    {
        public abstract bool Equals(Section? other);
    }

    /// <summary>
    /// Image section that contains the definition of a single image
    /// </summary>
    public class ImageSection : Section
    {
        public string ImageUrl = string.Empty;
        public string AltImageUrl = string.Empty;

        public override bool Equals(Section? other)
        {
            if (other == null)
                return false;

            if (other is ImageSection otherStandaloneImage)
            {
                return ImageUrl == otherStandaloneImage.ImageUrl 
                    && AltImageUrl == otherStandaloneImage.AltImageUrl;
            }

            return false;
        }
    }

    /// <summary>
    /// A column of multiple images, normally used in conjunction with another ImageColumnSection to produce side by side
    /// column of images on a webpage
    /// </summary>
    public class ImageColumnSection : Section
    {
        public List<ImageSection> Sections = new();

        public override bool Equals(Section? other)
        {
            if (other == null)
                return false;

            if (other is ImageColumnSection otherColumnImages)
            {
                return Sections == otherColumnImages.Sections;
            }

            return false;
        }
    }

    /// <summary>
    /// A section that has a single piece of title text 
    /// </summary>
    public class TitleSection : Section
    {
        public string TitleText = string.Empty;

        public override bool Equals(Section? other)
        {
            if (other == null)
                return false;

            if (other is TitleSection otherTitleImageSection)
            {
                return TitleText == otherTitleImageSection.TitleText;
            }

            return false;
        }
    }
}
