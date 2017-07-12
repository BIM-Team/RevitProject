//using Infralution.Licensing.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Res = Revit.Addin.RevitTooltip.Properties.Resources;

namespace Revit.Addin.RevitTooltip
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void linkLabelCopyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(linkLabelCopyright.Tag as string);
           }
            catch (System.Exception )
            {
            	// do nothing
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(linkLabel1.Tag as string);
            }
            catch (System.Exception )
            {
                // do nothing
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            linkLabel2_LinkClicked_1(null, null);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(linkLabel2.Tag as string);
            }
            catch (System.Exception)
            {
                // do nothing
            }
        }

        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(linkLabel2.Tag as string);
            }
            catch (System.Exception)
            {
                // do nothing
            }
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox3_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("oops, do you miss Jim now?");
        }

        private void AboutForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                MessageBox.Show("Hello, You may miss Jim ? :)");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
