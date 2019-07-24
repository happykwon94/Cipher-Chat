using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientForm
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private string Input_Pwd;
        public string PassPwd
        {
            get { return this.Input_Pwd; }
            set { this.Input_Pwd = value; }
        }

        private void InputPwdButton_Click(object sender, EventArgs e)
        {

            if (this.InputPwd.Text == "")
            {
                MessageBox.Show("[ Input Error ]");
                this.InputPwd.Clear();
            }
            else
            {
                PassPwd = this.InputPwd.Text;
                this.Hide();
            }
            
        }

        private void InputPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                InputPwdButton_Click(sender, e);
            }
        }
    }
}
