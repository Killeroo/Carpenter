using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpenter
{
    public abstract class ImageSection
    {
        public abstract void ReplacePreviewImage(string oldImageName, string newImageName);
        public abstract void ReplaceDetailedImage(string oldImageName, string newImageName);
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
    }
}
