using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lintianwen.Bookmark
{
    public partial class AddBookmarkForm : Form
    {
        private string bookmarkName;
        private bool isAdding;

        //reading bookmark name
        public string ReadBookmarkName
        {
            get { return bookmarkName; }
        }

        //get the mark of continuing
        public bool IsAdding
        {
            get { return isAdding; }
        }

        public AddBookmarkForm()
        {
            InitializeComponent();
            btnOK.Enabled = false;
        }

        #region form events
        //juding...
        private void textNaming_TextChanged(object sender, EventArgs e)
        {
            if (textNaming.Text != "")
            {
                btnOK.Enabled = true;
                isAdding = true;
            }
            else
            {
                btnOK.Enabled = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bookmarkName = textNaming.Text;
            textNaming.Text = "";
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            textNaming.Text = "";
            isAdding = false;
            this.Close();
        }
        #endregion
    }
}
