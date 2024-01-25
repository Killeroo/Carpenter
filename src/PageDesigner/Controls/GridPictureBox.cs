using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageDesigner.Controls
{
    // Simple wrapper around picture box, holding some extra info used to store data for
    // images that Carpenter needs
    internal class GridPictureBox : PictureBox
    {

        public bool Standalone = false;
        public string PreviewImageName = string.Empty;
        public string DetailedImageName = string.Empty;

        // TODO: Not needed
        //public event EventHandler<EventArgs> OnClick;
        
        //public GridPictureBox() : base()
        //{
        //    Click += GridPictureBox_Click;
        //}

        //private void GridPictureBox_Click(object? sender, EventArgs e)
        //{
        //    OnClick?.Invoke(this, new EventArgs());
        //}
    }
}
