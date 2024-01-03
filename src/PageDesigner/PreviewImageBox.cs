using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageDesigner
{


    public partial class PreviewImageBox : UserControl
    {
        public event EventHandler<PreviewImageEventArgs> ControlClicked;

        private string _imageName;

        public PreviewImageBox()
        {
            InitializeComponent();
        }

        public PreviewImageBox(string name, Image previewImage)
        {
            InitializeComponent();



            PictureBox.Image = previewImage;
            Label.Text = name;

            _imageName = name;
        }

        private void Control_MouseClick(object? sender, MouseEventArgs e)
        {
            if (ControlClicked != null)
            {
                ControlClicked(this, new PreviewImageEventArgs(_imageName));
            }
        }


        private void Control_Click(object? sender, EventArgs e)
        {
            if (ControlClicked != null)
            {
                ControlClicked(this, new PreviewImageEventArgs(_imageName));
            }
        }

        private void PreviewImageBox_Load(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                control.Click += Control_Click;
            }

            PictureBox.Click += Control_Click;
            Label.Click += Control_Click;
        }

    }

    public class PreviewImageEventArgs : EventArgs
    {
        public string ImageName;

        public PreviewImageEventArgs(string Name)
        {
            ImageName = Name;
        }
    }
}
