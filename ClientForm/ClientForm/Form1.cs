using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public static extern IntPtr encrypt_msg(byte[] plain_msg);

        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr decrypt_msg(byte[] cipher_msg);

        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool recv_pwd_result_decrypt(string input_pwd);


        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int decrypt_msg_int(byte[] input_pwd);

        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int encrypt_msg_int(byte[] input_pwd);
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
                    stream = client.GetStream();
                    // pwd 체크
                    //while (true)
                    //{

                    //    // pwd 체크
                    //    Form2 pwd_form = new Form2();

                    //    pwd_form.ShowDialog();
                    //    string input_pwd = pwd_form.PassPwd;

                    //    byte[] pwd_write = Encoding.Unicode.GetBytes(input_pwd + "$");
                    //    stream.Write(pwd_write, 0, pwd_write.Length);

                    //    byte[] pwd_read = new byte[1024];
                    //    if (stream.Read)
                    //    {
                    //        string pwd_result = Encoding.Unicode.GetString(pwd_read, 0, pwd_bytes);
                    //        pwd_result = pwd_result.Substring(0, pwd_result.IndexOf("$"));

                    //        if (recv_pwd_result_decrypt(pwd_result))
                    //        {
                    //            stream.Flush();
                    //            break;
                    //        }
                    //    }
                    //    client.Close();
                    //}
                    // pwd 체크

                    this.OutputMSG.AppendText("[ 채팅 서버에 연결되었습니다. ]\n\n");

                    this.InputIp.ReadOnly = true;
                    this.InputPort.ReadOnly = true;
                    this.OpenButton.Enabled = false;

                    Form3 name_set_form = new Form3();
                    name_set_form.ShowDialog();

                    string input_chat_name = name_set_form.PassChatName;
                    
                    //닉네임 설정
                    byte[] buffer = Encoding.Unicode.GetBytes(input_chat_name + "$");
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                    this.OutputMSG.AppendText("[ 이름이 설정되었습니다. ]  \""+ input_chat_name + "\"\n\n");
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

        }
        //**********************************************************************************************************************
        private void RecvMsg()
        {
            while (true)
            {
                stream = client.GetStream();
                int BUFFERSIZE = client.ReceiveBufferSize;
                byte[] buffer = new byte[BUFFERSIZE];
                int bytes = stream.Read(buffer, 0, buffer.Length);

                string message = Encoding.Unicode.GetString(buffer, 0, bytes);
                OutputMsgPrint(message);
            }
        }

        private void OutputMsgPrint(string msg)
        {
            if (OutputMSG.InvokeRequired)
            {
                OutputMSG.BeginInvoke(new MethodInvoker(delegate
                {
                    OutputMSG.AppendText(msg + Environment.NewLine);
                }));
            }
            else
                OutputMSG.AppendText(msg + Environment.NewLine);
        }
        //**********************************************************************************************************************


        private void SendButton_Click(object sender, EventArgs e)
        {
            //byte[] buffer = Encoding.Unicode.GetBytes(this.InputMSG.Text + "$");
            //stream.Write(buffer, 0, buffer.Length);
            //stream.Flush();
            //this.InputMSG.Clear();

            string org_msg = this.InputMSG.Text+"$";

            MessageBox.Show("입력값 길이(length) : " + org_msg.Length);

            byte[] please = new byte[16];

            byte[] enc_msg = Encoding.UTF8.GetBytes(org_msg);
            IntPtr ptr = encrypt_msg(enc_msg);
            Marshal.Copy(ptr, please, 0, 16);

            string str = Encoding.UTF8.GetString(please);
            MessageBox.Show("str : " + str);

            //IntPtr hope = decrypt_msg(please);
            //byte[] hope_big = new byte[16];
            //Marshal.Copy(hope, hope_big, 0, 16);

            //string believe = Encoding.UTF8.GetString(hope_big);

            //MessageBox.Show("believe : " + believe);

            byte[] buffer = Encoding.Unicode.GetBytes(str);
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            this.InputMSG.Clear();
        }

        private void InputPort_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenButton_Click(sender, e);
            }
        }
    }
}
