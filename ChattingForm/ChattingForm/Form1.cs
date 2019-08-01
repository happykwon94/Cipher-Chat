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

        public Dictionary<TcpClient, string> clientList = new Dictionary<TcpClient, string>();

//***************************************** 전역변수 설정(끝) ****************************************************

//######################################### 함수 설정 (시작) #####################################################
        private void InitSocket(IPAddress InputIP, int InputPort)
        {

            server = new TcpListener(InputIP, InputPort);
            client = default(TcpClient);
            server.Start();
            DisplayText(Encoding.UTF8.GetBytes("[Connect Success] - Server Open\n$"),"");

            Form2 pwd_form = new Form2();

            pwd_form.ShowDialog();
            pwd = pwd_form.PassPwd;

            string sec = "";
            for (int i = 0; i < pwd.Length - 1; i++)
                sec += "*";

            DisplayText(Encoding.UTF8.GetBytes("[PassWord Setting] - " + (pwd[0] + sec) + "\n$"), "");

            while (true)
            {
                try
                {
                    counter++;
                    client = server.AcceptTcpClient();
                    DisplayText(Encoding.UTF8.GetBytes("[ Connecting... ]\n$"), "");
                    NetworkStream stream = client.GetStream();

                    // pwd 체크
                    while (true)
                    {
                        byte[] pwd_read = new byte[1024];

                        //stream = client.GetStream();

                        int read_pwd_length = 0;
                        if(stream.CanRead)
                            read_pwd_length = stream.Read(pwd_read, 0, pwd_read.Length);

                        if (read_pwd_length == 0)
                            continue;

                        int input_pwd_size = 0;
                        IntPtr pwd_ptr = decrypt_msg(pwd_read, out input_pwd_size);
                        byte[] pwd_array = new byte[input_pwd_size];
                        Marshal.Copy(pwd_ptr, pwd_array, 0, pwd_array.Length);

                        string org_pwd = Encoding.UTF8.GetString(pwd_array);
                        org_pwd = org_pwd.Substring(0, org_pwd.IndexOf("$"));
                        org_pwd = org_pwd.Substring(1);

                        if (pwd == org_pwd)
                        {
                            string compare_correct = "|OK$";
                            byte[] compare_result = Encoding.UTF8.GetBytes(compare_correct);
                            stream.Write(compare_result, 0, compare_result.Length);
                            stream.Flush();
                            break;
                        }
                        else
                        {
                            string compare_correct = "|NOTOK$";
                            byte[] compare_result = Encoding.UTF8.GetBytes(compare_correct);
                            stream.Write(compare_result, 0, compare_result.Length);
                            stream.Flush();
                        }
                    }
                    // pwd 체크

                    byte[] buffer = new byte[1024];

                    int bytes = stream.Read(buffer, 0, buffer.Length);

                    string user_name = Encoding.Unicode.GetString(buffer, 0, bytes);
                    user_name = user_name.Substring(0, user_name.IndexOf("$"));
                    user_name = user_name.Substring(1);

                    clientList.Add(client, user_name);
                    string temp = "[ ID : " + user_name + " ] - Join!\n$";
                    SendMessageAll(Encoding.UTF8.GetBytes(temp), "", false);
                    DisplayText(Encoding.UTF8.GetBytes(temp), "");

                    handleClient h_client = new handleClient();
                    h_client.OnReceived += new handleClient.MessageDisplayHandler(OnReceived);
                    h_client.OnDisconnected += new handleClient.DisconnectedHandler(h_client_OnDisconnected);
                    h_client.startClient(client, clientList);
                }
                catch (SocketException err)
                {
                    if (client.Connected == false)
                    {
                        client.Close();
                    }
                    Trace.WriteLine(string.Format("[ Socket Exception ] \n {0}", err.Message));
                    break;
                }
                catch (Exception err)
                {
                    if (client.Connected == false)
                    {
                        client.Close();
                    }
                    Trace.WriteLine(string.Format("[ Error ] \n {0}", err.Message));
                    break;
                }
            }
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

            int size = 0;
            IntPtr dec_ptr = decrypt_msg(message, out size);
            byte[] dec_msg = new byte[size];
            Marshal.Copy(dec_ptr, dec_msg, 0, dec_msg.Length);

            string msg = Encoding.UTF8.GetString(dec_msg);

            if (msg.Contains('|'))
            {
                msg = msg.Substring(1);
            }

            byte[] msg_array = Encoding.UTF8.GetBytes(msg);

            DisplayText(msg_array, input_user_name);
            SendMessageAll(msg_array, input_user_name, true);
        }

        public void SendMessageAll(byte[] message, string user_name, bool flag)
        {
            foreach (var pair in clientList)
            {
                TcpClient client = pair.Key as TcpClient;
                NetworkStream stream = client.GetStream();

                if (user_name != "")
                {
                    Trace.WriteLine(string.Format("TCPclient : {0} User_ID : {1}", pair.Key, pair.Value));

                    byte[] temp = Encoding.UTF8.GetBytes(user_name);
                    byte[] send_byte_array = new byte[temp.Length + message.Length];

                    Array.Copy(temp, send_byte_array, temp.Length);
                    Array.Copy(message, 0, send_byte_array, temp.Length, message.Length);

                    string tester = Encoding.UTF8.GetString(send_byte_array);

                    if (tester.Length == 0)
                        continue;

                    byte[] return_byte_array = Encoding.UTF8.GetBytes("|" + tester);

                    int size = 0;
                    IntPtr send_enc_ptr = encrypt_msg(return_byte_array, out size);
                    byte[] buffer = new byte[size];
                    Marshal.Copy(send_enc_ptr, buffer, 0, buffer.Length);

                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
                else
                {
                    int size = 0;
                    IntPtr send_enc_ptr = encrypt_msg(message, out size);
                    byte[] buffer = new byte[size];
                    Marshal.Copy(send_enc_ptr, buffer, 0, buffer.Length);

                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
            }
        }

        private void DisplayText(byte[] text, string user_name)
        {
            string msg = Encoding.UTF8.GetString(text);

            string print_msg = user_name + msg;

            if (print_msg.Contains("$"))
            {
                print_msg = print_msg.Substring(0, print_msg.IndexOf("$"));
            }

            if (msg.Contains('|'))
            {
                msg = msg.Substring(1);
            }

            if (OutputMSG.InvokeRequired)
            { 
                OutputMSG.BeginInvoke(new MethodInvoker(delegate
                {
                    this.OutputMSG.AppendText(print_msg + Environment.NewLine);
                }));
            }
            else
                this.OutputMSG.AppendText(print_msg + "\n");
        }

        //######################################### 함수 설정 (끝) #####################################################


        // ################################## 이벤트 처리(시작) ###############################################################
        public ChatForm()
        {
            key_init();
            InitializeComponent();
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
                byte[] server_msg = Encoding.UTF8.GetBytes(msg);
                string server_name = "[ Server ] ";

                DisplayText(server_msg, server_name);
                SendMessageAll(server_msg, server_name, true);

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
