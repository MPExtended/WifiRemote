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
    public partial class PasscodeDialog : Form
    {
        public PasscodeDialog()
        {
            InitializeComponent();
        }

        internal string GetPasscode()
        {
            return textBoxPasscode.Text;
        }
    }
}
