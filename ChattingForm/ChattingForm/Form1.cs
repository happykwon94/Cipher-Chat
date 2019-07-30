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
            OutputMSG.AppendText("[Connect Success] - Server Open\n");

            while (true)
            {
                try
                {
                    counter++;
                    client = server.AcceptTcpClient();
                    OutputMSG.AppendText("[ Connecting... ]\n");

                    NetworkStream stream = client.GetStream();

                    // pwd 체크
                    while (true)
                    {
                        byte[] pwd_read = new byte[1024];
                        int read_pwd_length = stream.Read(pwd_read, 0, pwd_read.Length);

                        int input_pwd_size = 0;
                        IntPtr pwd_ptr = decrypt_msg(pwd_read, out input_pwd_size);
                        byte[] pwd_array = new byte[input_pwd_size];
                        Marshal.Copy(pwd_ptr, pwd_array, 0, pwd_array.Length);

                        string org_pwd = Encoding.UTF8.GetString(pwd_array);
                        org_pwd = org_pwd.Substring(0, org_pwd.IndexOf("$"));

                        if (pwd == org_pwd)
                        {
                            string compare_correct = "OK$";
                            byte[] compare_result = Encoding.UTF8.GetBytes(compare_correct);
                            stream.Write(compare_result, 0, compare_result.Length);
                            stream.Flush();
                            break;
                        }
                        else
                        {
                            string compare_correct = "NOTOK$";
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

                    clientList.Add(client, user_name);
                    OutputMSG.AppendText("[ ID :"+ user_name+" ] - Join\n");

                    string temp = "Welcome! \"" + user_name + "\", Hello! \n";
                    // send message all user
                    SendMessageAll(Encoding.UTF8.GetBytes(temp), "", false);

                    handleClient h_client = new handleClient();
                    h_client.OnReceived += new handleClient.MessageDisplayHandler(OnReceived);
                    h_client.OnDisconnected += new handleClient.DisconnectedHandler(h_client_OnDisconnected);
                    h_client.startClient(client, clientList);
                }
                catch (SocketException err)
                {
                    Trace.WriteLine(string.Format("[ Socket Exception ] \n {0}", err.Message));
                    break;
                }
                catch (Exception err)
                {
                    Trace.WriteLine(string.Format("[ Error ] \n {0}", err.Message));
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
        }

        //private void OnReceived(string message, string user_name)
        //{
        //    string input_user_msg = "[" + user_name + "] " + message;
        //    // 복호화
        //    MessageBox.Show("recv_server : "+ message);

        //    DisplayText(input_user_msg);
        //    SendMessageAll(message, user_name, true);
        //}

        private void OnReceived(byte[] message, string user_name)
        {
            string input_user_name = "[" + user_name + "]";

            int size = 0;
            IntPtr dec_ptr = decrypt_msg(message, out size);
            byte[] dec_msg = new byte[size];
            Marshal.Copy(dec_ptr, dec_msg, 0, dec_msg.Length);

            DisplayText(dec_msg, input_user_name);
            SendMessageAll(dec_msg, input_user_name, true);
        }

        //public void SendMessageAll(string message, string user_name, bool flag)
        //{
        //    foreach (var pair in clientList)
        //    {
        //        Trace.WriteLine(string.Format("TCPclient : {0} User_ID : {1}", pair.Key, pair.Value));

        //        TcpClient client = pair.Key as TcpClient;
        //        NetworkStream stream = client.GetStream();
        //        byte[] buffer = null;

        //        if (flag)
        //        {
        //            buffer = Encoding.UTF8.GetBytes("[ " + user_name + " ]" + message);

        //            //buffer = Encoding.Unicode.GetBytes("[ " + user_name + " ]" + message);
        //        }
        //        else
        //        {
        //            buffer = Encoding.UTF8.GetBytes(message);

        //            // buffer = Encoding.Unicode.GetBytes(message);
        //        }

        //        stream.Write(buffer, 0, buffer.Length);
        //        stream.Flush();
        //    }
        //}


        public void SendMessageAll(byte[] message, string user_name, bool flag)
        {
            foreach (var pair in clientList)
            {
                Trace.WriteLine(string.Format("TCPclient : {0} User_ID : {1}", pair.Key, pair.Value));

                TcpClient client = pair.Key as TcpClient;
                NetworkStream stream = client.GetStream();
                //byte[] buffer = null;

                //
                byte[] temp = Encoding.UTF8.GetBytes(user_name);
                byte[] send_byte_array = new byte[temp.Length+message.Length];

                Array.Copy(temp, send_byte_array, temp.Length);
                Array.Copy(message, 0, send_byte_array, temp.Length, message.Length);

                string tester = Encoding.UTF8.GetString(send_byte_array);

                int size = 0;
                IntPtr send_enc_ptr = encrypt_msg(send_byte_array, out size);
                byte[] buffer = new byte[size];
                Marshal.Copy(send_enc_ptr, buffer, 0, buffer.Length);

                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }

        //private void DisplayText(string text)
        //{

        //    string user_text = text.Substring(text.IndexOf("]") + 1, text.Length - (text.IndexOf("]") + 1));
        //    //user_name = text.Substring(0, text.IndexOf("]"));

        //    byte[] org_msg = Encoding.UTF8.GetBytes(user_text);

        //    MessageBox.Show("text : " + user_text);

        //    for(int i = 0; i< org_msg.Length; i++)
        //    {
        //        MessageBox.Show("서버 디스플레이 : "+i+" : "+org_msg[i]);
        //    }

        //    int size = 0;
        //    IntPtr dec_ptr = decrypt_msg(org_msg, out size);
        //    byte[] dec_msg = new byte[size];
        //    Marshal.Copy(dec_ptr, dec_msg, 0, dec_msg.Length);

        //    string msg = Encoding.UTF8.GetString(dec_msg); 

        //    MessageBox.Show("msg : " + msg);

        //    if (OutputMSG.InvokeRequired)
        //    {
        //        OutputMSG.BeginInvoke(new MethodInvoker(delegate
        //        {
        //            OutputMSG.AppendText(text + Environment.NewLine);
        //        }));
        //    }
        //    else
        //        OutputMSG.AppendText(text + Environment.NewLine);
        //}

        private void DisplayText(byte[] text, string user_name)
        {
            string msg = Encoding.UTF8.GetString(text);

            string print_msg = user_name + " " + msg;

            if (OutputMSG.InvokeRequired)
            { 
                OutputMSG.BeginInvoke(new MethodInvoker(delegate
                {
                    OutputMSG.AppendText(print_msg + Environment.NewLine);
                }));
            }
            else
                OutputMSG.AppendText(print_msg + Environment.NewLine);
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

                    Thread t = new Thread( () => InitSocket(addr, input_port) );
                    t.IsBackground = true;
                    t.Start();

                    this.InputIp.ReadOnly = true;
                    this.InputPort.ReadOnly = true;
                    this.OpenButton.Enabled = false;

                    Form2 pwd_form = new Form2();

                    pwd_form.ShowDialog();
                    pwd = pwd_form.PassPwd;

                    string sec = "";
                    for (int i = 0; i < pwd.Length - 1; i++)
                        sec += "*";

                    this.OutputMSG.AppendText("[PassWord Setting] - " + (pwd[0]+ sec)+"\n");
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
            if (e.KeyCode == Keys.Enter && e.Shift == true)
            {
                this.InputMSG.AppendText("\n");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendButton_Click(sender, e);
            }
        }

        // ################################## 이벤트 처리(끝) ###############################################################
    }
}
