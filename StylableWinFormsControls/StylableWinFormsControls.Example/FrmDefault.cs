using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace StylableWinFormsControls.Example
{
    public partial class FrmDefault : Form
    {
        public FrmDefault()
        {
            InitializeComponent();
        }

        private void stylableButton1_Click(object sender, EventArgs e)
        {
            new FrmMdi(true).Show();
        }
    }
}