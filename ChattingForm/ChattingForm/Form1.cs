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


namespace ChattingForm
{
    public partial class ChatForm : Form
    {

        [DllImport("server_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void key_init();

        [DllImport("server_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string decrypt_msg(string cipher_msg);

        [DllImport("server_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string compare_pwd(string input_pwd);


        public ChatForm()
        {
            InitializeComponent();
            key_init();
        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            string input_ip = this.InputIp.Text;
            string input_port_temp = this.InputPort.Text;
            int input_port = int.Parse(input_port_temp);

            string success_msg = "[Connect Success] - Server Open\n";

            if (input_ip == "" || input_port_temp == "")
            {
                MessageBox.Show("입력 필요");
            }

            else
            {
                try
                {
                    Socket root = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    IPAddress addr = IPAddress.Parse(input_ip);

                    IPEndPoint end_ptr = new IPEndPoint(addr, input_port);

                    this.OutputMSG.AppendText(success_msg);

                    this.InputIp.ReadOnly = true;
                    this.InputPort.ReadOnly = true;

                    root.Bind(end_ptr);

                    root.Listen(5);

                    while (true)
                    {
                        Socket client = root.Accept();

                        Pwd_check(client);

                        Thread client_th = new Thread(() => Client_Receive_Thread(client));
                        client_th.Start();

                        client_th.Join();
                    }
                }
                catch(Exception err)
                {
                    this.OutputMSG.AppendText("[Error] " + err + "\n");
                }
             
            }
        }

        string data;

        void Client_Receive_Thread(Socket target)
        {
            while (true)
            {
                
                MessageBox.Show("MSG 8! ");
                byte[] bytes = new byte[1024];
                int bytesRec = target.Receive(bytes);

                MessageBox.Show("bytesRec :  " + bytesRec);

                data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                MessageBox.Show("data : " + data);

                if (data.IndexOf("<eof>") > -1)
                    break;

                data = data.Substring(0, data.Length - 5);        

                string plain_text = decrypt_msg(data);

                MessageBox.Show("plain_text : " + plain_text);

                Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("plain_text : " + plain_text);
                    this.OutputMSG.AppendText(plain_text + "\n");
                });
            }

        }

        void Pwd_check(Socket client)
        {
            while(true){
                MessageBox.Show("MSG 8! ");
                byte[] bytes = new byte[1024];
                int bytesRec = client.Receive(bytes);

                MessageBox.Show("bytesRec :  " + bytesRec);

                data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                MessageBox.Show("data : " + data);

                if (data.IndexOf("<eof>") > -1)
                    break;

                data = data.Substring(0, data.Length - 5);

                string result = compare_pwd(data);

                if (result == "OK")
                    break;
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
    }
}
