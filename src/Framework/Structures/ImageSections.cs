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
        public abstract void ReplacePreviewImage(string oldImageName, string newImageName);
        public abstract void ReplaceDetailedImage(string oldImageName, string newImageName);
        public abstract bool Equals(ImageSection? other);
    }

    public class StandaloneImageSection : ImageSection
    {
        public string PreviewImage;
        public string DetailedImage;

        public override void ReplacePreviewImage(string oldImageName, string newImageName)
        {
            if (PreviewImage.ToLower() == oldImageName.ToLower())
            {
                PreviewImage = newImageName;
            }
        }

        public override void ReplaceDetailedImage(string oldImageName, string newImageName)
        {
            if (DetailedImage.ToLower() == oldImageName.ToLower())
            {
                DetailedImage = newImageName;
            }
        }

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

        public override void ReplacePreviewImage(string oldImageName, string newImageName)
        {
            foreach (var section in Sections)
            {
                if (section.PreviewImage.ToLower() == oldImageName.ToLower())
                {
                    section.ReplacePreviewImage(oldImageName, newImageName);
                    break;
                }
            }
        }

        public override void ReplaceDetailedImage(string oldImageName, string newImageName)
        {
            foreach (var section in Sections)
            {
                if (section.DetailedImage.ToLower() == oldImageName.ToLower())
                {
                    section.ReplaceDetailedImage(oldImageName, newImageName);
                    break;
                }
            }
        }

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

        public override void ReplaceDetailedImage(string oldImageName, string newImageName)
        {
            throw new NotImplementedException();
        }

        public override void ReplacePreviewImage(string oldImageName, string newImageName)
        {
            throw new NotImplementedException();
        }

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
