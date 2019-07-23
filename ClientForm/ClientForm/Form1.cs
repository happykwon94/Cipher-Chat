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
        public static extern string encrypt_msg(string plain_msg);

        [DllImport("client_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool recv_pwd_result_decrypt(string input_pwd);


        Socket client_socket;
        byte[] bytes = new byte[1024];
        string data;

        public Form1()
        {
            InitializeComponent();
            key_init();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            string input_ip = this.InputIp.Text;

            string input_port_temp = this.InputPort.Text;
            int input_port = int.Parse(input_port_temp);

            if (input_ip == "" || input_port_temp == "")
            {
                MessageBox.Show("IP, PORT 입력 필요");

                this.InputIp.Clear();
                this.InputPort.Clear();
            }

            else
            {
                try
                {
                    client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    client_socket.Connect(new IPEndPoint(IPAddress.Parse(input_ip), input_port));
                    this.OutputMSG.AppendText(String.Format("소켓 연결이 되었습니다 {0} -> ", client_socket.RemoteEndPoint.ToString()));

                    MessageBox.Show("enc_msg : " + 1);

                    this.InputIp.ReadOnly = true;
                    this.InputPort.ReadOnly = true;

                    MessageBox.Show("enc_msg : " + 2);
                    //아래 두개는 do_receive 함수를 위한 쓰레드입니다.
                    //쓰레드가 있어야만 연결된다고 해야할까요.
                    Thread listen_thread = new Thread(do_receive);
                    listen_thread.Start();
                }
                catch(Exception err)
                {
                    this.OutputMSG.AppendText("[Error] "+ err + "\n");
                }
            }

        }

        void do_receive()
        {
            while (true)
            {
                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = client_socket.Receive(bytes);
                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<eof>") > -1)
                        break;
                }
                data = data.Substring(0, data.Length - 5);

                Invoke((MethodInvoker)delegate
                {
                    this.OutputMSG.AppendText(data+"\n");
                }
                );
                data = "";
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string enc_msg = encrypt_msg(InputMSG.Text);
            MessageBox.Show("enc_msg(send) : "+ enc_msg);

            byte[] msg = Encoding.UTF8.GetBytes(enc_msg + "<eof>");
            int bytesSent = client_socket.Send(msg);

            InputMSG.Clear();
        }
    }
}
