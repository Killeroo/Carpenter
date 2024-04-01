using PageDesigner.Controls;
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
        public event EventHandler<ImageEventArgs> ControlClicked;
        public event EventHandler<ImageEventArgs> ControlDoubleClicked;
        public event EventHandler<ImageEventArgs> AddContextItemClicked;
        public event EventHandler<ImageEventArgs> InsertContextItemClicked;
        public event EventHandler<ImageEventArgs> ReplaceContextItemClicked;

        public void SetSelected(bool isSelected) => _isPreviewSelected = isSelected;
        public bool GetSelected() => _isPreviewSelected;

        public string GetImageName => _imageName;


        private string _imageName;
        private bool _isPreviewSelected = false;

        private ImageEventArgs _eventArgs;



        public PreviewImageBox()
        {
            InitializeComponent();
        }

        public PreviewImageBox(string name, Image previewImage)
        {
            InitializeComponent();

            this.Paint += PreviewImageBox_Paint;

            PictureBox.Image = previewImage;
            Label.Text = name;
            _imageName = name;

            // Setup event args for whenever this control is interacted with
            _eventArgs = new ImageEventArgs(_imageName);
        }

        private void PreviewImageBox_Load(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                control.Click += PreviewImageBox_Click;
            }

            PictureBox.Click += PreviewImageBox_Click;
            Label.Click += PreviewImageBox_Click;

            PictureBox.DoubleClick += PictureBox_DoubleClick;
            Label.DoubleClick += Label_DoubleClick;

            this.Paint += PreviewImageBox_Paint;
        }

        private void Label_DoubleClick(object? sender, EventArgs e)
        {
            if (ControlDoubleClicked != null)
            {
                ControlDoubleClicked(this, _eventArgs);
            }
        }

        private void PictureBox_DoubleClick(object? sender, EventArgs e)
        {
            if (ControlDoubleClicked != null)
            {
                ControlDoubleClicked(this, _eventArgs);
            }
        }

        private void PreviewImageBox_Click(object? sender, EventArgs e)
        {
            if (ControlClicked != null)
            {
                ControlClicked(this, _eventArgs);
            }
        }

        private void PreviewImageBox_Paint(object? sender, PaintEventArgs e)
        {
            if (_isPreviewSelected)
            {
                using (Brush b = new SolidBrush(Color.FromArgb(100, Color.Blue)))
                {
                    e.Graphics.FillRectangle(b, this.DisplayRectangle);
                }
            }
        }

        private void AddImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AddContextItemClicked != null)
            {
                AddContextItemClicked(this, _eventArgs);
            }
        }

        private void InsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (InsertContextItemClicked != null)
            {
                InsertContextItemClicked(this, _eventArgs);
            }
        }

        private void ReplaceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ReplaceContextItemClicked != null)
            {
                ReplaceContextItemClicked(this, _eventArgs);
            }
        }


    }
}
