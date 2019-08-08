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

        string pwd = "";
        bool flag;
        bool userNameSetFlag;
        bool pwdSetFlag;
        static int count = 0;

        static int NameCount = 2;

        IntPtr send_enc_ptr;
        IntPtr send_dec_ptr;

        public Dictionary<TcpClient, string> clientList = new Dictionary<TcpClient, string>();

//***************************************** 전역변수 설정(끝) ****************************************************

//######################################### 함수 설정 (시작) #####################################################
        public void InitSocket(IPAddress InputIP, int InputPort)
        {
            server = new TcpListener(InputIP, InputPort);
            client = default(TcpClient);
            server.Start();

            this.OutputMSG.Text = "";

            DisplayText("[Connect Success] - Server Open\n");

            pwdSetFlag = SetPassword();

            while (true)
            {
                try
                {
                    if (!pwdSetFlag)
                    {
                        pwdSetFlag = SetPassword();
                        continue;
                    }

                    client = server.AcceptTcpClient();
                    count++;

                    flag = true;
                    userNameSetFlag = true;

                    NetworkStream stream = client.GetStream();

                    DisplayText("[ Connecting... ]\n");


                    // 비밀번호 입력받아서 대조
                    while (flag)
                    {
                        byte[] pwd_read = new byte[1024];
                        int read_pwd_length = 0;

                        read_pwd_length = stream.Read(pwd_read, 0, pwd_read.Length);

                        if (read_pwd_length != 0)
                        {
                            byte[] temp_arr = new byte[read_pwd_length];
                            Array.Copy(pwd_read, temp_arr, read_pwd_length);

                            string org_pwd = DecryptMsg(temp_arr);

                            org_pwd = MakeOriginMsg(org_pwd);

                            byte[] sendBuffer = PwdCheckFlag(org_pwd);

                            stream.Write(sendBuffer, 0, sendBuffer.Length);
                            stream.Flush();
                        }
                    }

                    // 닉네임 입력받아서 저장
                    while (userNameSetFlag)
                    {
                        byte[] buffer = new byte[1024];

                        int bytes = stream.Read(buffer, 0, buffer.Length);

                        //string user_name = Encoding.UTF8.GetString(buffer, 0, bytes);
                        byte[] temp = new byte[bytes];
                        Array.Copy(buffer, temp, temp.Length);

                        string user_name = DecryptMsg(temp);
                        user_name = MakeOriginMsg(user_name);

                        UserNameCheck(user_name);
                    }

                    DisplayText("[ Connect! ] - Client Member : " + count +"\n");

                    // 클라이언트 동작
                    handleClient h_client = new handleClient();
                    h_client.OnReceived += new handleClient.MessageDisplayHandler(OnReceived);
                    h_client.OnDisconnected += new handleClient.DisconnectedHandler(h_client_OnDisconnected);
                    h_client.startClient(client, clientList);
                    //if (client != null)
                    //{
                    //    Thread acceptCl = new Thread(() => AcceptClient(client));
                    //    acceptCl.IsBackground = true;
                    //    acceptCl.Start();
                    //}
                }
                catch(SocketException err)
                {
                    Trace.WriteLine(string.Format("[ Server SocketException Error ] \n {0}", err.Message));

                    this.OutputMSG.AppendText("[ Server SocketException Error] " + err + "\n\n");
                    ServerClose();
                }
                catch(Exception err)
                {
                    Trace.WriteLine(string.Format("[ Server Exception Error ] \n {0}", err.Message));

                    this.OutputMSG.AppendText("[ Server Exception Error] " + err + "\n\n");
                    ServerClose();
                }
            }
        }

        public void AcceptClient(TcpClient client)
        {

            flag = true;
            userNameSetFlag = true;

            NetworkStream stream = client.GetStream();

            DisplayText("[ Connecting... ]\n");


            // 비밀번호 입력받아서 대조
            while (flag)
            {
                byte[] pwd_read = new byte[1024];
                int read_pwd_length = 0;

                read_pwd_length = stream.Read(pwd_read, 0, pwd_read.Length);

                if (read_pwd_length != 0)
                {
                    byte[] temp_arr = new byte[read_pwd_length];
                    Array.Copy(pwd_read, temp_arr, read_pwd_length);

                    string org_pwd = DecryptMsg(temp_arr);

                    org_pwd = MakeOriginMsg(org_pwd);

                    byte[] sendBuffer = PwdCheckFlag(org_pwd);

                    stream.Write(sendBuffer, 0, sendBuffer.Length);
                    stream.Flush();
                }
            }

            // 닉네임 입력받아서 저장
            while (userNameSetFlag)
            {
                byte[] buffer = new byte[1024];

                int bytes = stream.Read(buffer, 0, buffer.Length);

                //string user_name = Encoding.UTF8.GetString(buffer, 0, bytes);
                byte[] temp = new byte[bytes];
                Array.Copy(buffer, temp, temp.Length);

                string user_name = DecryptMsg(temp);
                user_name = MakeOriginMsg(user_name);

                UserNameCheck(user_name);
            }

            // 클라이언트 동작
            handleClient h_client = new handleClient();
            h_client.OnReceived += new handleClient.MessageDisplayHandler(OnReceived);
            h_client.OnDisconnected += new handleClient.DisconnectedHandler(h_client_OnDisconnected);
            h_client.startClient(client, clientList);
        }


        // 클라이언트 삭제시 정보 삭제
        void h_client_OnDisconnected(TcpClient client, string user_name)
        {
            if (clientList.ContainsKey(client))
                clientList.Remove(client);

            string Exit_msg = "Bye Bye!";
            count--;
            DisplayText(Exit_msg + " - \"" + user_name+"\" 현재 클라이언트 수 : "+clientList.Count);
            SendMessageAll(" - \"" + user_name + "\" 현재 클라이언트 수 : " + clientList.Count, Exit_msg, true);
            client.Close();
        }

        // 메세지 입력받을 때 동작
        private void OnReceived(byte[] message, string user_name)
        {
            string input_user_name = "[" + user_name + "] ";

            string input_user_msg = DecryptMsg(message);

            input_user_msg = MakeOriginMsg(input_user_msg);

            string msg = input_user_name + input_user_msg;

            if(input_user_msg == "<End Msg>")
            {
                h_client_OnDisconnected(client, user_name);
            }
            else
            {
                DisplayText(msg);
                SendMessageAll(input_user_msg, input_user_name, true);
            }
        }

        // 연결된 클라이언트에 메세지 전체 전달
        public void SendMessageAll(string message, string user_name, bool flag)
        {
            if (flag)
            {
                foreach (var pair in clientList)
                {
                    TcpClient client = pair.Key as TcpClient;
                    NetworkStream stream = client.GetStream();

                    Trace.WriteLine(string.Format("TCPclient : {0} User_ID : {1}", pair.Key, pair.Value));

                    byte[] send_byte = MakeEncryptMsg(user_name + message);

                    string text = DecryptMsg(send_byte);

                    text = MakeOriginMsg(text);

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

                    byte[] normal_text = MakeEncryptMsg(message);

                    stream.Write(normal_text, 0, normal_text.Length);
                    stream.Flush();
                }
            }
        }

        // 서버의 출력 창에 메세지 출력
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
         
        // 암호화 함수
        private byte[] EncryptMsg(string msg)
        {
            byte[] msgToUTF8Byte = Encoding.UTF8.GetBytes(msg);
            //byte[] msgToAnsiByte = Encoding.Default.GetBytes(msg);

            int size = 0;
            send_enc_ptr = encrypt_msg(msgToUTF8Byte, out size);
            byte[] buffer = new byte[size];
            Marshal.Copy(send_enc_ptr, buffer, 0, buffer.Length);

            return buffer;
        }

        // 복호화 함수
        private string DecryptMsg(byte[] msg)
        {
            int size = 0;
            send_dec_ptr = decrypt_msg(msg, out size);
            byte[] buffer = new byte[size];
            Marshal.Copy(send_dec_ptr, buffer, 0, buffer.Length);

            string msgToUTF8String = Encoding.UTF8.GetString(buffer);
            //string msgToAnsiString = Encoding.Default.GetString(buffer);

            return msgToUTF8String;
        }

        // 닉네임 입력받는 함수
        private void UserNameCheck(string userName)
        {

            if(clientList.ContainsValue(userName) && clientList.Count != 0)
            {
                clientList.Add(client, userName + NameCount);
                string temp = "[ ID : " + userName + "_" + (NameCount++) + " ] - Join!\n";

                DisplayText(temp);
                SendMessageAll(temp, "", false);

                userNameSetFlag = false;
            }
            else
            {
                clientList.Add(client, userName);
                string temp = "[ ID : " + userName + " ] - Join!\n";

                DisplayText(temp);
                SendMessageAll(temp, "", false);

                userNameSetFlag = false;
            }
        }

        // 메세지의 인덱스를 제거하여 원래의 메세지로 바꿔주는 함수
        private string MakeOriginMsg(string msg)
        {
            if (msg.Contains("&")) //msg.Contains("$")
            {
                msg = msg.Substring(0, msg.IndexOf("&"));
            }

            if (msg.Contains(">SOT<")) //msg.Contains("|")
            {
                msg = msg.Substring(5);
            }

            return msg;
        }

        // 오류를 제거한 암호문 만드는 함수
        private byte[] MakeEncryptMsg(string msg)
        {
            msg = ">SOT<" + msg + "&";

            byte[] send_byte_msg = EncryptMsg(msg);

            while (send_byte_msg.Length < 16)
            {
                msg += "$";

                send_byte_msg = EncryptMsg(msg);
            }

            return send_byte_msg;
        }

        // 비밀번호를 대조하는 함수
        private byte[] PwdCheckFlag(string inputPwd)
        {
            if (pwd == inputPwd)
            {
                string compare_correct = ">SOT<OK&";
                byte[] compare_result = Encoding.Default.GetBytes(compare_correct);
                flag = false;
                return compare_result;
            }
            else
            {
                string compare_correct = ">SOT<NOTOK&";
                byte[] compare_result = Encoding.Default.GetBytes(compare_correct);
                return compare_result;
            }
        }

        // 비밀번호를 설정하는 함수
        private bool SetPassword()
        {
            bool result = true;

            Form2 pwd_form = new Form2();

            pwd_form.ShowDialog();
            pwd = pwd_form.PassPwd;

            if(pwd == null)
            {
                MessageBox.Show("[Program Exit]");

                this.InputIp.ReadOnly = false;
                this.InputIp.TabStop = true;

                this.InputPort.ReadOnly = false;
                this.InputPort.TabStop = true;

                this.OpenButton.Enabled = true;
                this.OpenButton.TabStop = true;

                result = false;
            }
            else
            {
                string sec = "";
                for (int i = 0; i < pwd.Length - 1; i++)
                    sec += "*";

                //this.InputIp.ReadOnly = true;
                //this.InputIp.TabStop = false;

                //this.InputPort.ReadOnly = true;
                //this.InputPort.TabStop = false;

                //this.OpenButton.Enabled = false;
                //this.OpenButton.TabStop = false;

                DisplayText("[PassWord Setting] - " + (pwd[0] + sec) + "\n");
            }

            return result;
        }

        private void ServerClose()
        {
            string end_msg = "<EndMsg>";

            SendMessageAll(end_msg, "", false);

            if (client != null)
            {
                client.Close();
                client = null;
            }

            if (server != null)
            {
                server.Stop();
                Marshal.FreeHGlobal(send_enc_ptr);
                Marshal.FreeHGlobal(send_dec_ptr);
                server = null;
            }
        }

        //######################################### 함수 설정 (끝) #####################################################


        // ################################## 이벤트 처리(시작) ###############################################################
        
        // 프로그램이 로드될때 이벤트
        public ChatForm()
        {
            InitializeComponent();
            key_init();
        }

        // 서버 오픈 버튼 클릭 시 이벤트
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

                    Thread tcp = new Thread(() => InitSocket(addr, input_port));
                    tcp.IsBackground = true;
                    tcp.Start();
                }
                catch (Exception err)
                {
                    this.OutputMSG.AppendText("[Server Error] " + err + "\n");
                }
             
            }
        }

        // 메세지 Send 버튼 클릭 시 이벤트
        private void SendButton_Click(object sender, EventArgs e)
        {
            string msg = this.InputMSG.Text;

            if (msg == "")
                MessageBox.Show("MSG error! ");
            else
            {
                string server_name = "[ Server ] ";

                DisplayText(server_name + msg);
                SendMessageAll(msg, server_name, true);

                this.InputMSG.Text = "";
            }
        }

        // 엔터키로 서버 오픈
        private void InputPort_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenButton_Click(sender, e);
                this.InputPort.Focus();
                SendKeys.Send("{backspace}");
                SendKeys.Send("{Tab}");
            }
        }

        // 엔터키로 메세지 전송
        private void InputMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Shift == false)
            {
                SendButton_Click(sender, e);
                this.InputMSG.Focus();
                SendKeys.Send("{backspace}");
                this.ActiveControl = InputMSG;
            }

            if (e.Shift && e.KeyCode == Keys.Enter)
            {
                this.InputMSG.AppendText("\n");
            }
        }

        // 서버 닫힐 때 이벤트
        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ServerClose();
        }

        private void InputIp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Shift == false)
            {
                SendButton_Click(sender, e);
                this.InputIp.Focus();
                SendKeys.Send("{backspace}");
            }
        }

        // ################################## 이벤트 처리(끝) #######f########################################################
    }
}