using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClientForm
{
    public partial class Form1 : Form
    {
        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void key_init();

        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr encrypt_msg(byte[] plain_msg, out int size);

        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr decrypt_msg(byte[] cipher_msg, out int size);
        //**********************************************************************************************************************

        TcpClient client = new TcpClient();
        NetworkStream stream = default(NetworkStream);

        //**********************************************************************************************************************
        public Form1()
        {
            InitializeComponent();
            key_init();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            string input_ip = this.InputIp.Text;
            string input_port_temp = this.InputPort.Text;

            if (String.IsNullOrWhiteSpace(input_ip) || String.IsNullOrWhiteSpace(input_port_temp))
            {
                MessageBox.Show("[ Input Error ]");

                this.InputIp.Clear();
                this.InputPort.Clear();
            }

            else
            {
                try
                {
                    int input_port = int.Parse(input_port_temp);
                    client.Connect(IPAddress.Parse(input_ip), input_port);

                    Form2 pwd_form = new Form2();

                    this.InputIp.ReadOnly = true;
                    this.InputPort.ReadOnly = true;

                    pwd_form.ShowDialog();
                    string input_pwd = pwd_form.PassPwd;

                    byte[] pwd_write = Encoding.UTF8.GetBytes("|"+input_pwd + "$");


                    int pwd_size = 0;

                    IntPtr pwd_ptr = encrypt_msg(pwd_write, out pwd_size);

                    byte[] pwd_byte_array = new byte[pwd_size];

                    Marshal.Copy(pwd_ptr, pwd_byte_array, 0, pwd_byte_array.Length);

                    stream = client.GetStream();

                    stream.Write(pwd_byte_array, 0, pwd_byte_array.Length);
                    stream.Flush();

                    // pwd 체크
                    try
                    {
                        while (true)
                        {

                            byte[] result_read = new byte[1024];

                            int read_pwd_length = 0;
                            //stream.ReadTimeout = 10000;
                            if (stream.DataAvailable)
                            {
                                read_pwd_length = stream.Read(result_read, 0, result_read.Length);
                            }

                            if (read_pwd_length == 0)
                                continue;

                            string org_pwd = Encoding.UTF8.GetString(result_read,0, read_pwd_length);

                            org_pwd = org_pwd.Substring(0, org_pwd.IndexOf("$"));

                            this.InputIp.ReadOnly = false;
                            this.InputPort.ReadOnly = false;

                            if (org_pwd == "|OK")
                            {
                                this.InputIp.ReadOnly = true;
                                this.InputPort.ReadOnly = true;
                                this.OpenButton.Enabled = false;
                                break;
                            }
                            else
                            {
                                pwd_form = new Form2();

                                this.InputIp.ReadOnly = true;
                                this.InputPort.ReadOnly = true;

                                pwd_form.ShowDialog();
                                input_pwd = pwd_form.PassPwd;

                                pwd_write = Encoding.UTF8.GetBytes("|" + input_pwd + "$");


                                pwd_size = 0;

                                pwd_ptr = encrypt_msg(pwd_write, out pwd_size);

                                pwd_byte_array = new byte[pwd_size];

                                Marshal.Copy(pwd_ptr, pwd_byte_array, 0, pwd_byte_array.Length);

                                stream = client.GetStream();

                                stream.Write(pwd_byte_array, 0, pwd_byte_array.Length);
                                stream.Flush();
                            }
                        }

                        this.OutputMSG.AppendText("[ 채팅 서버에 연결되었습니다. ]\n\n");

                        Form3 name_set_form = new Form3();
                        name_set_form.ShowDialog();

                        string input_chat_name = name_set_form.PassChatName;

                        //닉네임 설정
                        byte[] buffer = Encoding.Unicode.GetBytes("|" + input_chat_name + "$");
                        if (stream.CanWrite)
                            stream.Write(buffer, 0, buffer.Length);
                        stream.Flush();
                        this.OutputMSG.AppendText("[ 이름이 설정되었습니다. ]  \"" + input_chat_name + "\"\n\n");
                        //닉네임 설정

                        Thread t_handler = new Thread(RecvMsg);
                        t_handler.IsBackground = true;
                        t_handler.Start();

                    }
                    catch (Exception err)
                    {
                        this.OutputMSG.AppendText("[Error] " + err + "\n\n");
                    }

                }
                catch (Exception err)
                {
                    this.OutputMSG.AppendText("[Error] " + err + "\n\n");
                }
        }

        }
        //**********************************************************************************************************************
        private void RecvMsg()
        {
            try
            {
                while (true)
                {
                    stream = client.GetStream();
                    //int BUFFERSIZE = client.ReceiveBufferSize;
                    //byte[] buffer = new byte[BUFFERSIZE];
                    byte[] buffer = new byte[1024];

                    int bytes = 0;

                    if (stream.CanRead)
                        bytes = stream.Read(buffer, 0, buffer.Length);

                    if (bytes == 0)
                    {
                        continue;
                    }

                    OutputMsgPrint(buffer);
                }
             }

            catch (Exception err)
            {
                this.OutputMSG.AppendText("[Error] " + err + "\n\n");
            }
        }

        private void OutputMsgPrint(byte[] msg)
        {
            int size_temp = 0;
            IntPtr hope = decrypt_msg(msg, out size_temp);
            byte[] hope_big = new byte[size_temp];
            Marshal.Copy(hope, hope_big, 0, hope_big.Length);

            string text = Encoding.UTF8.GetString(hope_big);

            if (text.Contains("$"))
            {
                text = text.Substring(0, text.IndexOf("$"));
            }

            if (text.Contains("|"))
            {
                text = text.Substring(1);
            }

            if (text.Contains("]"))
            {
                char sp = ']';
                string[] user_data = text.Split(sp);
                user_data[0] = user_data[0].Trim();
                user_data[1] = user_data[1].Trim();
                text = (user_data[0]+"] "+ user_data[1]);
            }

            if (OutputMSG.InvokeRequired)
            {
                OutputMSG.BeginInvoke(new MethodInvoker(delegate
                {
                    OutputMSG.AppendText(text + "\n");
                }));
            }
            else
                OutputMSG.AppendText(text + "\n");
        }

        //**********************************************************************************************************************


        private void SendButton_Click(object sender, EventArgs e)
        {
            
            string org_msg = "|" + this.InputMSG.Text;

            if(String.IsNullOrWhiteSpace(this.InputMSG.Text))
            {
                MessageBox.Show("[Input Error]");
            }
            else if (this.InputMSG.Text.First() == '|')
            {
                MessageBox.Show("[First Char Error] ");
                this.InputMSG.Text = "";
            }
            else if (this.InputMSG.Text.EndsWith("$"))
            {
                MessageBox.Show("[Last Char Error] ");
                this.InputMSG.Text = "";
            }
            else
            {
                byte[] enc_msg = Encoding.UTF8.GetBytes(org_msg);
                int size = 0;
                IntPtr ptr = encrypt_msg(enc_msg, out size);
                byte[] please = new byte[size];
                Marshal.Copy(ptr, please, 0, size);
               
                while (true)
                {
                    if (size != 0)
                    {
                        stream.Write(please, 0, please.Length);
                        stream.Flush();
                        this.InputMSG.Text = "";
                        break;
                    }
                }
            }
        }

        private void InputPort_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenButton_Click(sender, e);
            }
        }

        private void InputMsg_KeyPreview(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Enter)
            {
                this.InputMSG.AppendText("\n");
            }
        }

        private void InputMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Shift == false)
            {
                SendButton_Click(sender, e);
                this.InputMSG.Focus();
                SendKeys.Send("{backspace}");
            }
        }
    }
}
