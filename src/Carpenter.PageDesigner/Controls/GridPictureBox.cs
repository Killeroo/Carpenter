using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PageDesigner.Controls
{
    // Simple wrapper around picture box, holding some extra info used to store data for
    // images that Carpenter needs
    internal class GridPictureBox : PictureBox
    {
        public event EventHandler<EventArgs> SwapMenuItemClicked;
        public event EventHandler<EventArgs> RemoveMenuItemClicked;
        public event EventHandler<EventArgs> StandaloneMenuItemClicked;

        public bool IsStandaloneImage() => _isStandaloneImage;

        // TODO: Make readonly
        public string PreviewImageName = string.Empty;
        public string DetailedImageName = string.Empty;

        private bool _isStandaloneImage = false;

        // UI elements
        private ToolStripItem RemoveToolStripItem;
        private ToolStripMenuItem StandaloneToolStripItem;
        private ToolStripItem SwapToolStripItem;

        public GridPictureBox() : base() 
        {
            // Setup right click context menu
            ContextMenuStrip = new ContextMenuStrip();
            SwapToolStripItem = ContextMenuStrip.Items.Add("Swap...");
            RemoveToolStripItem = ContextMenuStrip.Items.Add("Remove");

            StandaloneToolStripItem = new ToolStripMenuItem("Standalone");
            StandaloneToolStripItem.Checked = _isStandaloneImage;
            ContextMenuStrip.Items.Add(StandaloneToolStripItem);

            SwapToolStripItem.Click += SwapToolStripItem_Click;
            RemoveToolStripItem.Click += RemoveToolStripItem_Click;
            StandaloneToolStripItem.Click += StandaloneToolStripItem_Click;
        }

        // TODO: Rename
        public void SetImage(Image image, string previewName, string detailedName,  bool isStandaloneImage)
        {
            Image = image;
            PreviewImageName = previewName;
            DetailedImageName = detailedName;
            SetStandaloneImage(isStandaloneImage);
        }

        public void SetStandaloneImage(bool value)
        {
            _isStandaloneImage = value;
            StandaloneToolStripItem.Checked = value;
        }

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
