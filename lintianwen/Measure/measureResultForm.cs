using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lintianwen.Measure
{
    public partial class measureResultForm : Form
    {
        //init the event when form closed
        public delegate void FormClosedEventHandler();
        public event FormClosedEventHandler frmClosed = null;

        public measureResultForm()
        {
            InitializeComponent();
        }

        //del happened when form closed
        private void measureResultForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (frmClosed != null)
                frmClosed();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
