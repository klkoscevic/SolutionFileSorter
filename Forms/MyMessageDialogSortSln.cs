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
    public partial class MyMessageDialogSortSln : Form
    {
        public MyMessageDialogSortSln()
        {
            InitializeComponent();
            pictureBox.Paint += PictureBox_Paint;
        }

        public MyMessageDialogSortSln(string solutionFilename) : this()
        {
            labelCaption.Text = string.Format(caption, solutionFilename);
        }

        const string caption = " Are you sure you want to sort projects in '{0}' solution file?";

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawIcon(SystemIcons.Information, 0, 0);
        }

        public bool SortAlwaysWithoutAskingChecked
        {
            get { return SortAlwaysWithoutAsking.Checked; }
        }

        private void BtnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void BtnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
