using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageDesigner
{
    public class ImageEventArgs : EventArgs
    {
        public string ImageName;

        public ImageEventArgs(string Name)
        {
            ImageName = Name;
        }
    }
}
