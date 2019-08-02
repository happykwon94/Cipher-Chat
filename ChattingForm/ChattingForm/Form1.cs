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

using System.Diagnostics;


namespace ChattingForm
{
    public partial class ChatForm : Form
    {

        [DllImport("server_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void key_init();

        [DllImport("server_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr encrypt_msg(byte[] plain_msg, out int size);

        [DllImport("server_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr decrypt_msg(byte[] cipher_msg, out int size);
        // dll 가져오기

        //***************************************** 전역변수 설정(시작) *************************************************

        TcpListener server = null;
        TcpClient client = null;
        static int counter = 0;
        string pwd = "";
        bool flag = true;
        bool userNameSetFlag = true;

        public Dictionary<TcpClient, string> clientList = new Dictionary<TcpClient, string>();

//***************************************** 전역변수 설정(끝) ****************************************************

//######################################### 함수 설정 (시작) #####################################################
        public void InitSocket(IPAddress InputIP, int InputPort)
        {
           
            server = new TcpListener(InputIP, InputPort);
            client = default(TcpClient);
            server.Start();

            DisplayText("[Connect Success] - Server Open\n");

            SetPassword();

            while (true)
            {
                counter++;
                client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                try
                {
                    if (flag)
                    {
                        byte[] pwd_read = new byte[1024];
                        int read_pwd_length = 0;

                        read_pwd_length = stream.Read(pwd_read, 0, pwd_read.Length);

                        if (read_pwd_length == 0)
                            continue;

                        byte[] temp_arr = new byte[read_pwd_length];
                        Array.Copy(pwd_read, temp_arr, read_pwd_length);

                        string org_pwd = DecryptMsg(temp_arr);

                        org_pwd = MakeOriginMsg(org_pwd);

                        byte[] sendBuffer = PwdCheckFlag(org_pwd);

                        stream.Write(sendBuffer, 0, sendBuffer.Length);
                        stream.Flush();

                        if (userNameSetFlag)
                        {
                            byte[] buffer = new byte[1024];

                            int bytes = stream.Read(buffer, 0, buffer.Length);

                            string user_name = Encoding.Unicode.GetString(buffer, 0, bytes);
                            user_name = MakeOriginMsg(user_name);

                            UserNameCheck(user_name);
                        }
                    }

                    DisplayText("[ Connect! client!]\n");

                    handleClient h_client = new handleClient();
                    h_client.OnReceived += new handleClient.MessageDisplayHandler(OnReceived);
                    h_client.OnDisconnected += new handleClient.DisconnectedHandler(h_client_OnDisconnected);
                    h_client.startClient(client, clientList);
                }
                catch (Exception err)
                {
                    Trace.WriteLine(string.Format("[ Error ] \n {0}", err.Message));

                    this.OutputMSG.AppendText("[Error] " + err + "\n\n");
                    break;
                }
            }
            client.Close();
            server.Stop();
        }

        void h_client_OnDisconnected(TcpClient client)
        {
            if (clientList.ContainsKey(client))
                clientList.Remove(client);
            client.Close();
        }

        private void OnReceived(byte[] message, string user_name)
        {
            string input_user_name = "[" + user_name + "] ";

            string input_user_msg = DecryptMsg(message);
            input_user_msg = MakeOriginMsg(input_user_msg);

            string msg = input_user_name + input_user_msg;

            DisplayText(msg);
            SendMessageAll(input_user_msg, input_user_name, true);
        }

        public void SendMessageAll(string message, string user_name, bool flag)
        {
            if (flag)
            {
                foreach (var pair in clientList)
                {
                    TcpClient client = pair.Key as TcpClient;
                    NetworkStream stream = client.GetStream();

                    Trace.WriteLine(string.Format("TCPclient : {0} User_ID : {1}", pair.Key, pair.Value));

                    string send_msg = "|" + user_name + message + "$";

                    byte[] send_byte = EncryptMsg(send_msg);

                    stream.Write(send_byte, 0, send_byte.Length);
                    stream.Flush();
                }
            }
            else
            {
                foreach (var pair in clientList)
                {
                    TcpClient client = pair.Key as TcpClient;
                    NetworkStream stream = client.GetStream();

                    message = "|" + message + "$";

                    byte[] normal_text = EncryptMsg(message);

                    stream.Write(normal_text, 0, normal_text.Length);
                    stream.Flush();
                }
            }
        }

        private void DisplayText(string text)
        {
            if (OutputMSG.InvokeRequired)
            {
                OutputMSG.BeginInvoke(new MethodInvoker(delegate
                {
                    this.OutputMSG.AppendText(text + Environment.NewLine);
                }));
            }
            else
                this.OutputMSG.AppendText(text + Environment.NewLine);
        }

        private byte[] EncryptMsg(string msg)
        {
            byte[] msgToUTF8Byte = Encoding.UTF8.GetBytes(msg);

            int size = 0;
            IntPtr send_enc_ptr = encrypt_msg(msgToUTF8Byte, out size);
            byte[] buffer = new byte[size];
            Marshal.Copy(send_enc_ptr, buffer, 0, buffer.Length);

            return buffer;
        }

        private string DecryptMsg(byte[] msg)
        {
            int size = 0;
            IntPtr send_enc_ptr = decrypt_msg(msg, out size);
            byte[] buffer = new byte[size];
            Marshal.Copy(send_enc_ptr, buffer, 0, buffer.Length);

            string msgToUTF8String = Encoding.UTF8.GetString(buffer);

            return msgToUTF8String;
        }

        private void UserNameCheck(string userName)
        {
            clientList.Add(client, userName);
            string temp = "[ ID : " + userName + " ] - Join!\n";

            DisplayText(temp);
            SendMessageAll(temp, "", false);

            userNameSetFlag = false;
        }

        private string MakeOriginMsg(string msg)
        {
            if (msg.Contains("$"))
            {
                msg = msg.Substring(0, msg.IndexOf("$"));
            }

            if (msg.Contains("|"))
            {
                msg = msg.Substring(1);
            }

            return msg;
        }

        private byte[] PwdCheckFlag(string inputPwd)
        {
            if (pwd == inputPwd)
            {
                string compare_correct = "|OK$";
                byte[] compare_result = Encoding.UTF8.GetBytes(compare_correct);
                flag = false;
                return compare_result;
            }
            else
            {
                string compare_correct = "|NOTOK$";
                byte[] compare_result = Encoding.UTF8.GetBytes(compare_correct);
                return compare_result;
            }
        }

        private void SetPassword()
        {
            Form2 pwd_form = new Form2();

            pwd_form.ShowDialog();
            pwd = pwd_form.PassPwd;

            string sec = "";
            for (int i = 0; i < pwd.Length - 1; i++)
                sec += "*";

            DisplayText("[PassWord Setting] - " + (pwd[0] + sec) + "\n");
        }

        //######################################### 함수 설정 (끝) #####################################################


        // ################################## 이벤트 처리(시작) ###############################################################
        public ChatForm()
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
            }
            else
            {
                try
                {

                    IPAddress addr = IPAddress.Parse(input_ip);
                    int input_port = int.Parse(input_port_temp);

                    Thread t = new Thread(() => InitSocket(addr, input_port));
                    t.IsBackground = true;
                    t.Start();

                    this.InputIp.ReadOnly = true;
                    this.InputPort.ReadOnly = true;
                    this.OpenButton.Enabled = false;
                }
                catch (Exception err)
                {
                    this.OutputMSG.AppendText("[Error] " + err + "\n");
                }
             
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string msg = this.InputMSG.Text;

            if (msg == "")
                MessageBox.Show("MSG error! ");
            else if (msg.First() == '|')
            {
                MessageBox.Show("[First Char Error] ");
                this.InputMSG.Text = "";
            }
            else if (msg.EndsWith("$"))
            {
                MessageBox.Show("[Last Char Error] ");
                this.InputMSG.Text = "";
            }
            else
            {
                string server_name = "[ Server ] ";

                DisplayText(server_name+msg);
                SendMessageAll(msg, server_name, true);

                this.InputMSG.Text = "";
            }
        }

        private void InputPort_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenButton_Click(sender, e);
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

            private void InputMsg_Preview(object sender, PreviewKeyDownEventArgs e)
            {
            if (e.Shift && e.KeyCode == Keys.Enter)
            {
                this.InputMSG.AppendText("\n");
            }
        }

        // ################################## 이벤트 처리(끝) #######f########################################################
    }
}
