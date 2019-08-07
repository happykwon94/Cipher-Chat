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

        //******************************************** DLL 가져오기 *********************************************
        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void key_init();

        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr encrypt_msg(byte[] plain_msg, out int size);

        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr decrypt_msg(byte[] cipher_msg, out int size);
        //********************************************************************************************************


        //***************************************** 전역변수 설정 ************************************************
        TcpClient client = new TcpClient();
        NetworkStream stream = default(NetworkStream);

        //********************************************************************************************************
        

        //*************************************** 함수 세팅 ******************************************************
       
        // 메세지가 들어올 때 처리
        private void RecvMsg()
        {
            while (true)
            {
                stream = client.GetStream();

                int BUFFERSIZE = client.ReceiveBufferSize;

                byte[] buffer = new byte[BUFFERSIZE];

                int bytes = stream.Read(buffer, 0, buffer.Length);

                if (bytes != 0)
                    OutputMsgPrint(buffer);
                else
                    continue;
            }
        }

        // 클라이언트 출력창에 출력
        private void OutputMsgPrint(byte[] msg)
        {
            string text = DecryptMsg(msg);

            text = MakeOriginMsg(text);

            if(text == "<EndMsg>")
            {
                text = "---------- [ Server Close ] ----------";
                stream.Close();
                client.Close();
            }

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


        // 암호화
        private string DecryptMsg(byte[] msg)
        {
            int size = 0;
            IntPtr send_enc_ptr = decrypt_msg(msg, out size);
            byte[] buffer = new byte[size];
            Marshal.Copy(send_enc_ptr, buffer, 0, buffer.Length);

            //string msgToUTF8String = Encoding.UTF8.GetString(buffer);
            string msgToAnsiString = Encoding.Default.GetString(buffer);


            return msgToAnsiString;
        }

        // 메세지의 인덱스를 제거하여 원래의 메세지로 바꿔주는 함수
        private string MakeOriginMsg(string msg)
        {
            if (msg.Contains("&")) //msg.Contains("$")
            {
                msg = msg.Substring(0, msg.IndexOf("&"));
            }

            if (msg.Contains("<SOT>")) //msg.Contains("|")
            {
                msg = msg.Substring(5);
            }

            return msg;
        }

        // 오류를 제거한 암호문을 만드는 함수
        private byte[] MakeEncryptMsg(string msg)
        {
            msg = "<SOT>" + msg + "&";

            byte[] send_byte_msg = EncryptMsg(msg);

            while (send_byte_msg.Length < 16)
            {
                msg = msg + "&";
                // 암호화
                send_byte_msg = EncryptMsg(msg);
            }

            return send_byte_msg;
        }

        // 암호화
        private byte[] EncryptMsg(string msg)
        {
            //byte[] msgToUTF8Byte = Encoding.UTF8.GetBytes(msg);
            byte[] msgToAnsiByte = Encoding.Default.GetBytes(msg);

            int size = 0;
            IntPtr send_enc_ptr = encrypt_msg(msgToAnsiByte, out size);
            byte[] buffer = new byte[size];
            Marshal.Copy(send_enc_ptr, buffer, 0, buffer.Length);

            return buffer;
        }

        // 비밀번호를 입력받는 함수
        private byte[] InputPwd()
        {
            Form2 pwd_form = new Form2();
            pwd_form.ShowDialog();

            string input_pwd = pwd_form.PassPwd;

            byte[] pwd_byte = MakeEncryptMsg(input_pwd);

            return pwd_byte;
        }

        // 닉네임을 입력받는 함수
        private byte[] InputChatName()
        {
            Form3 name_set_form = new Form3();
            name_set_form.ShowDialog();

            string input_chat_name = name_set_form.PassChatName;

            byte[] buffer = MakeEncryptMsg(input_chat_name);

            this.OutputMSG.AppendText("[ 이름이 설정되었습니다. ]  \"" + input_chat_name + "\"\n\n");

            return buffer;
        }

        // 비밀번호 입력 결과를 확인하는 함수
        private bool PwdCheckResult(byte[] result, int result_length)
        {
            bool return_result = false;

            //string pwd_result = Encoding.UTF8.GetString(result, 0, result_length);
            string pwd_result = Encoding.Default.GetString(result, 0, result_length);

            pwd_result = MakeOriginMsg(pwd_result);

            if (pwd_result == "OK")
            {
                this.InputIp.ReadOnly = true;
                this.InputPort.ReadOnly = true;
                this.OpenButton.Enabled = false;
                return_result = true;
            }
            else
            {
                MessageBox.Show("[ 비밀번호 불일치 ]");
            }

            return return_result;
        }

        //********************************************************************************************************



        //*************************************** 이벤트 처리 ****************************************************

        // 폼이 로드될 때 이벤트
        public Form1()
        {
            InitializeComponent();
            key_init();
        }

        // 서버에 연결될 때 이벤트
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

                    this.InputIp.ReadOnly = true;
                    this.InputIp.TabStop = false;
                    this.InputPort.ReadOnly = true;
                    this.InputPort.TabStop = false;
                    this.OpenButton.Enabled = false;
                    this.OpenButton.TabStop = false;

                    byte[] pwd = InputPwd();

                    stream = client.GetStream();

                    stream.Write(pwd, 0, pwd.Length);
                    stream.Flush();

                    // pwd 체크
                    try
                    {
                        while (true)
                        {
                            byte[] result_read = new byte[1024];

                            int read_pwd_length = 0;
                            if (stream.CanRead)
                            {
                                read_pwd_length = stream.Read(result_read, 0, result_read.Length);
                            }

                            if (read_pwd_length == 0)
                                continue;

                            if (PwdCheckResult(result_read, read_pwd_length))
                            {
                                break;
                            }
                            else
                            {
                                pwd = InputPwd();

                                stream = client.GetStream();

                                stream.Write(pwd, 0, pwd.Length);
                                stream.Flush();
                            }

                        }
                        this.OutputMSG.Clear();
                        this.OutputMSG.AppendText("[ 채팅 서버에 연결되었습니다. ]\n\n");

                        //닉네임 설정 <Start>
                        byte[] chatName = InputChatName();

                        if (stream.CanWrite)
                            stream.Write(chatName, 0, chatName.Length);
                        stream.Flush();
                        //닉네임 설정 <End>

                        Thread t_handler = new Thread(RecvMsg);
                        t_handler.IsBackground = true;
                        t_handler.Start();

                    }
                    catch (Exception err)
                    {
                        this.OutputMSG.AppendText("[Check Error] " + err + "\n\n");

                        stream.Close();
                        client.Close();
                    }

                }
                catch (Exception err)
                {
                    this.OutputMSG.AppendText("[Connect Error] " + err + "\n\n");
                    stream.Close();
                    client.Close();
                }
            }
        }

        // 메세지 전송 시 이벤트
        private void SendButton_Click(object sender, EventArgs e)
        {
            
            string input_msg = this.InputMSG.Text;

            // 입력 에러 체크
            if(String.IsNullOrWhiteSpace(input_msg))
            {
                MessageBox.Show("[Input Error]");
            }
            else
            {
                byte[] send_byte_msg = MakeEncryptMsg(input_msg);

                stream.Write(send_byte_msg, 0, send_byte_msg.Length);
                stream.Flush();
                this.InputMSG.Text = "";
            }
        }

        // 엔터키로 서버 접속
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

        private void InputIp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Shift == false)
            {
                SendButton_Click(sender, e);
                this.InputIp.Focus();
                SendKeys.Send("{backspace}");
            }
        }

        private void FormClosing_Event(object sender, FormClosingEventArgs e)
        {
            byte[] send_byte_msg = MakeEncryptMsg("<End Msg>");

            stream.Write(send_byte_msg, 0, send_byte_msg.Length);
            stream.Flush();

            if (client != null)
                client.Close();
        }
        //********************************************************************************************************

    }
}
