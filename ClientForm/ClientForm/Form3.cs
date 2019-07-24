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
    public partial class Form3 : Form
    {

        private string Input_Chat_Name;
        public string PassChatName
        {
            get { return this.Input_Chat_Name; }
            set { this.Input_Chat_Name = value; }
        }

        public Form3()
        {
            InitializeComponent();
        }

        private void InputPwdButton_Click(object sender, EventArgs e)
        {
            if (this.InputChatName.Text == "")
            {
                MessageBox.Show("[ Input Error ]");
                this.InputChatName.Clear();
            }
            else
            {
                PassChatName = this.InputChatName.Text;
                this.Hide();
            }
        }
    }
}
