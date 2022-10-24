//https://stackoverflow.com/questions/24643408/how-to-do-on-the-fly-image-compression-in-c
//https://developer.mozilla.org/en-US/docs/Web/CSS/object-fit
//https://stackoverflow.com/questions/19414856/how-can-i-make-all-images-of-different-height-and-width-the-same-via-css

using System.Drawing.Imaging;
using System.Drawing;

string directory = Directory.GetCurrentDirectory();
string[] images = Directory.GetFiles(directory, "*.jpg");

foreach (string jpeg in images)
{
    using (Image originalImage = Image.FromFile(jpeg))
    {
        
    }
        Console.WriteLine(jpeg);
}