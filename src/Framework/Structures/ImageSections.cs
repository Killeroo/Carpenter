using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Carpenter
{
    public abstract class ImageSection : IEquatable<ImageSection>
    {
        public abstract bool Equals(ImageSection? other);
    }

    public class StandaloneImageSection : ImageSection
    {
        public string PreviewImage;
        public string DetailedImage;

        public override bool Equals(ImageSection? other)
        {
            if (other == null)
                return false;

            if (other is StandaloneImageSection otherStandaloneImage)
            {
                return PreviewImage == otherStandaloneImage.PreviewImage 
                    && DetailedImage == otherStandaloneImage.DetailedImage;
            }

            return false;
        }
    }

    public class ColumnImageSection : ImageSection
    {
        public List<StandaloneImageSection> Sections = new();

        public override bool Equals(ImageSection? other)
        {
            if (other == null)
                return false;

            if (other is ColumnImageSection otherColumnImages)
            {
                return Sections == otherColumnImages.Sections;
            }

            return false;
        }
    }

    public class TitleImageSection : ImageSection
    {
        public string TitleText = "";

        public override bool Equals(ImageSection? other)
        {
            if (other == null)
                return false;

            if (other is TitleImageSection otherTitleImageSection)
            {
                return TitleText == otherTitleImageSection.TitleText;
            }

            return false;
        }
    }
}
