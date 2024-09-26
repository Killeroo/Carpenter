using CefSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageDesigner.Forms
{
    public partial class LivePreviewForm : Form
    {
        private string _currentLoadedUrl = string.Empty;

        public LivePreviewForm()
        {
            InitializeComponent();
        }

        public void LoadUrl(string address)
        {
            if (chromiumWebBrowser == null)
            {
                return;
            }

            if (address == _currentLoadedUrl)
            {
                chromiumWebBrowser.Reload();
            }
            else
            {
                chromiumWebBrowser.LoadUrl(address);
                _currentLoadedUrl = address;
            }
        }
    }
}
