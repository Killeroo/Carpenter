using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PageDesigner.Controls
{
    /// <summary> 
    /// Simple wrapper around picture box, used for displaying image and storing
    /// info about image in a schema grid layout
    /// </summary>
    internal class GridPictureBox : PictureBox
    {
        /// <summary>
        /// The different types of image that can be stored in the GridPictureBox
        /// </summary>
        public enum ImageType
        {
            Standalone,
            Column
        }

        /// <summary>
        /// Image toolstrip event callbacks
        /// </summary>
        public event EventHandler<EventArgs> SwapMenuItemClicked;
        public event EventHandler<EventArgs> RemoveMenuItemClicked;
        public event EventHandler<EventArgs> StandaloneMenuItemClicked;

        /// <summary>
        /// Each image has can have 2 file names, the image that is displayed
        /// on the webpage and the detailed image that can be clicked through too
        /// both can be edited and are associated with a single image in a grid
        /// </summary>
        public string ImageFilename = string.Empty;
        public string DetailImageFilename = string.Empty;

        /// <summary>
        /// Is the image a standalone image that doesn't get put into a column
        /// </summary>
        private ImageType _imageType = ImageType.Column;

        /// <summary>
        /// Contextual tool strip ui elements
        /// </summary>
        private ToolStripItem RemoveToolStripItem;
        private ToolStripMenuItem StandaloneToolStripItem;
        private ToolStripItem SwapToolStripItem;

        public ImageType GetImageType() => _imageType;

        public GridPictureBox() : base() 
        {
            // Setup right click context menu
            ContextMenuStrip = new ContextMenuStrip();
            SwapToolStripItem = ContextMenuStrip.Items.Add("Swap...");
            RemoveToolStripItem = ContextMenuStrip.Items.Add("Remove");

            StandaloneToolStripItem = new ToolStripMenuItem("Standalone");
            StandaloneToolStripItem.Checked = _imageType == ImageType.Standalone;
            ContextMenuStrip.Items.Add(StandaloneToolStripItem);

            SwapToolStripItem.Click += SwapToolStripItem_Click;
            RemoveToolStripItem.Click += RemoveToolStripItem_Click;
            StandaloneToolStripItem.Click += StandaloneToolStripItem_Click;
        }

        // TODO: Rename
        public void SetImage(Image image, string previewName, string detailedName, ImageType type)
        {
            Image = image;
            ImageFilename = previewName;
            DetailImageFilename = detailedName;
            SetImageType(type);
        }

        /// <summary>
        /// Sets if the image should be an image displayed on it's own
        /// or an image that is displayed as a column
        /// </summary>
        public void SetImageType(ImageType type)
        {
            _imageType = type;
            StandaloneToolStripItem.Checked = type == ImageType.Standalone;
        }

        /// <summary>
        /// Internal tool strip callbacks
        /// </summary>

        private void SwapToolStripItem_Click(object? sender, EventArgs e)
        {
            if (SwapMenuItemClicked != null)
            {
                SwapMenuItemClicked(this, new EventArgs());
            }
        }

        private void StandaloneToolStripItem_Click(object? sender, EventArgs e)
        {
            if (StandaloneMenuItemClicked != null)
            {
                StandaloneMenuItemClicked(this, new EventArgs()); // TODO: PAss through 'e'
            }
        }

        private void RemoveToolStripItem_Click(object? sender, EventArgs e)
        {
            if (RemoveMenuItemClicked != null)
            {
                RemoveMenuItemClicked(this, new EventArgs());
            }
        }
    }
}
