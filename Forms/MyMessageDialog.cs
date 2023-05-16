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
        }
        public bool DoNotShowMesssageAnymoreChecked
        {
            get { return DoNotShowMesssageAnymore.Checked; }
        }

        private void btnOk_click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
