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

                    this.InputIp.ReadOnly = true;
                    this.InputPort.ReadOnly = true;

                    Form2 pwd_form = new Form2();
                    pwd_form.ShowDialog();

                    string input_pwd = pwd_form.PassPwd;
                    string send_pwd = "|" + input_pwd + "$";

                    byte[] pwd_byte = EncryptMsg(send_pwd);

                    stream = client.GetStream();

                    stream.Write(pwd_byte, 0, pwd_byte.Length);
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

                            string pwd_result = Encoding.UTF8.GetString(result_read,0, read_pwd_length);
                            pwd_result = MakeOriginMsg(pwd_result);

                            if (pwd_result == "OK")
                            {
                                this.InputIp.ReadOnly = true;
                                this.InputPort.ReadOnly = true;
                                this.OpenButton.Enabled = false;
                                break;
                            }
                            else
                            {
                                MessageBox.Show("[ 비밀번호 불일치 ]");

                                pwd_form.ShowDialog();
                                input_pwd = pwd_form.PassPwd;

                                send_pwd = "|" + input_pwd + "$";

                                pwd_byte = EncryptMsg(send_pwd);

                                stream = client.GetStream();

                                stream.Write(pwd_byte, 0, pwd_byte.Length);
                                stream.Flush();
                                
                            }
                        }

                        this.OutputMSG.AppendText("[ 채팅 서버에 연결되었습니다. ]\n\n");

                        //닉네임 설정 <Start>
                        Form3 name_set_form = new Form3();
                        name_set_form.ShowDialog();

                        string input_chat_name = name_set_form.PassChatName;

                        byte[] buffer = Encoding.Unicode.GetBytes("|" + input_chat_name + "$");
                        if (stream.CanWrite)
                            stream.Write(buffer, 0, buffer.Length);
                        stream.Flush();
                        this.OutputMSG.AppendText("[ 이름이 설정되었습니다. ]  \"" + input_chat_name + "\"\n\n");
                        //닉네임 설정 <End>

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
        // 입력 받기
        private void RecvMsg()
        {
            while (true)
            {
                try
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

                catch (Exception err)
                {
                    this.OutputMSG.AppendText("[Error] " + err + "\n\n");
                    break;
                }

            }
        }

        // OutputMsg에 출력
        private void OutputMsgPrint(byte[] msg)
        {
            // 복호화
            string text = DecryptMsg(msg);

            // 문자열에 시작과 끝 구분하고 
            text = MakeOriginMsg(text);

            // 출력하기
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


        private string DecryptMsg(byte[] msg)
        {
            int size = 0;
            IntPtr send_enc_ptr = decrypt_msg(msg, out size);
            byte[] buffer = new byte[size];
            Marshal.Copy(send_enc_ptr, buffer, 0, buffer.Length);

            string msgToUTF8String = Encoding.UTF8.GetString(buffer);

            return msgToUTF8String;
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

        private byte[] EncryptMsg(string msg)
        {
            byte[] msgToUTF8Byte = Encoding.UTF8.GetBytes(msg);

            int size = 0;
            IntPtr send_enc_ptr = encrypt_msg(msgToUTF8Byte, out size);
            byte[] buffer = new byte[size];
            Marshal.Copy(send_enc_ptr, buffer, 0, buffer.Length);

            return buffer;
        }

        //**********************************************************************************************************************


        private void SendButton_Click(object sender, EventArgs e)
        {
            
            string input_msg = this.InputMSG.Text;

            // 입력 에러 체크
            if(String.IsNullOrWhiteSpace(input_msg))
            {
                MessageBox.Show("[Input Error]");
            }
            else if (input_msg.First() == '|')
            {
                MessageBox.Show("[First Char Error] ");
                this.InputMSG.Text = "";
            }
            else if (input_msg.EndsWith("$"))
            {
                MessageBox.Show("[Last Char Error] ");
                this.InputMSG.Text = "";
            }
            else
            {
                input_msg = "|" + input_msg + "$";
                // 암호화
                byte[] send_byte_msg = EncryptMsg(input_msg);

                //네트워크 스트림에 쓰기
                stream.Write(send_byte_msg, 0, send_byte_msg.Length);
                stream.Flush();
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
