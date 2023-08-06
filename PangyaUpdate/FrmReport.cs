using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PangyaUpdate
{
    public partial class FrmReport : Form
    {
        public FrmReport()
        {
            InitializeComponent();
        }

        private void FrmReport_Load(object sender, EventArgs e)
        {

        }

        private void FrmReport_SizeChanged(object sender, EventArgs e)
        {
            var s = new Size(474, 408);
            if (Size != s)
            {
                Size = s;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
         if(   MessageBox.Show("Cancelou o envio de bugs report !", "NOTICE", MessageBoxButtons.OK, MessageBoxIcon.None) == DialogResult.OK)
            {
                Close();
            }
        }
    }
}
