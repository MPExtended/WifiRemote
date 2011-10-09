using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DemoClient
{
    public partial class PlaylistAddDialog : Form
    {
        public PlaylistAddDialog()
        {
            InitializeComponent();
        }

        public string GetAlbum()
        {
            return textBoxAlbum.Text;
        }

        public int GetLimit()
        {
            return (int)numericUpDownLimit.Value;
        }

        public bool Append()
        {
            return checkBoxAppend.Checked;
        }

        public bool Shuffle()
        {
            return checkBoxShuffle.Checked;
        }

        public bool Autoplay()
        {
            return checkBoxShuffle.Checked;
        }
    }
}
