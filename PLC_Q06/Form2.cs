using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.Framework.Lib;

namespace PLC_Q06
{
    public partial class Success_noti : Form
    {
        public Success_noti()
        {
            InitializeComponent();
            
            

        }

        private void btn_Ok_SuccessForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void change_label_content(string newtext)
        {
            label_Content_Success_Form.Text = newtext;
            label_Content_Success_Form.Location = new Point((label_Content_Success_Form.Parent.Width - label_Content_Success_Form.Width) / 2, label_Content_Success_Form.Location.Y);
            //int length_lable = label_Content_Success_Form.Size.Width;
            //int postion_lable_X = (390 - length_lable) / 2;
            //label_Content_Success_Form.Location = new Point(postion_lable_X, 140);
        }
    }
}
