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
        public static extern IntPtr encrypt_msg(string plain_msg);

        [DllImport("server_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr decrypt_msg(string cipher_msg);

        [DllImport("server_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string compare_pwd(string input_pwd);

//***************************************** 전역변수 설정(시작) *************************************************

        TcpListener server = null;
        TcpClient client = null;
        static int counter = 0;

        public Dictionary<TcpClient, string> clientList = new Dictionary<TcpClient, string>();

//***************************************** 전역변수 설정(끝) ****************************************************

//######################################### 함수 설정 (시작) #####################################################
        private void InitSocket(IPAddress InputIP, int InputPort)
        {
            server = new TcpListener(InputIP, InputPort);
            client = default(TcpClient);
            server.Start();
            this.OutputMSG.AppendText("[Connect Success] - Server Open\n");

            while (true)
            {
                try
                {
                    counter++;
                    client = server.AcceptTcpClient();
                    this.OutputMSG.AppendText("[ Connecting... ]\n");

                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string user_name = Encoding.Unicode.GetString(buffer, 0, bytes);
                    user_name = user_name.Substring(0, user_name.IndexOf("$"));

                    clientList.Add(client, user_name);

                    // send message all user
                    SendMessageAll("Welcome! \"" + user_name + "\", Hello! \n", "", false);

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

        private void OnReceived(string message, string user_name)
        {
            //string input_user_msg = "[" + user_name + "] **" + message;
            //DisplayText(input_user_msg);
            //SendMessageAll(message, user_name, true);

            string input_user_msg = "[" + user_name + "] pr1" + message;
            DisplayText(input_user_msg);
            SendMessageAll(message, user_name, true);
        }

        public void SendMessageAll(string message, string user_name, bool flag)
        {
            foreach (var pair in clientList)
            {
                Trace.WriteLine(string.Format("tcpclient : {0} user_name : {1}", pair.Key, pair.Value));

                TcpClient client = pair.Key as TcpClient;
                NetworkStream stream = client.GetStream();
                byte[] buffer = null;

                if (flag)
                {
                    buffer = Encoding.Unicode.GetBytes("[ " + user_name + " ] pr2" + message);
                }
                else
                {
                    buffer = Encoding.Unicode.GetBytes(message);
                }

                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }

        private void DisplayText(string text)
        {
            if (OutputMSG.InvokeRequired)
            {
                OutputMSG.BeginInvoke(new MethodInvoker(delegate
                {
                    OutputMSG.AppendText(text + Environment.NewLine);
                }));
            }
            else
                OutputMSG.AppendText(text + Environment.NewLine);
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

                    Thread t = new Thread( () => InitSocket(addr, input_port) );
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
            else
            {
                //inputMSG(msg);

                string recv_msg = "[Receive MSG] : " + msg + "\n";

                this.OutputMSG.AppendText(recv_msg);

                this.InputMSG.Clear();
            }
        }

        private void InputPort_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenButton_Click(sender, e);
            }
        }

        // ################################## 이벤트 처리(끝) ###############################################################
    }
}
