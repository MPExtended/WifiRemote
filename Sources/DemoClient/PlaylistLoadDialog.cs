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
    public partial class PlaylistLoadDialog : Form
    {
        public PlaylistLoadDialog()
        {
            InitializeComponent();
        }

        internal string GetPlaylistName()
        {
            return textBoxPlaylist.Text;
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
