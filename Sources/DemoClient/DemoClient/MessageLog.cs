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
    public partial class MessageLog : Form
    {
        private String log = String.Empty;
        public String Log
        {
            set {
                log = log + value + "\r\n\r\n";
                richTextBox1.Text = log;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;

            }
        }

        public MessageLog()
        {
            InitializeComponent();

            richTextBox1.Text = log;
        }
    }
}
