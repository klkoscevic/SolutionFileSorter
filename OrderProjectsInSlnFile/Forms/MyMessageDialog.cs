using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderProjectsInSlnFile.Forms
{
    public partial class MyMessageDialog : Form
    {
        public MyMessageDialog()
        {
            InitializeComponent();
            pictureBox.Paint += PictureBox_Paint;
        }

        public MyMessageDialog(string solutionFilename) : this()
        {
            labelCaption.Text = string.Format(caption, solutionFilename);
        }

        const string caption = "Sort .sln file\n\nProjects in '{0}' file are now sorted alphabetically.";

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawIcon(SystemIcons.Information, 0, 0);
        }

        public bool DoNotShowMesssageAnymoreChecked
        {
            get { return DoNotShowMesssageAnymore.Checked; }
        }

        private void BtnOk_click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
