using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ChattingForm
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChatForm());
        }
    }

    class handleClient
    {


        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        [DllImport("server_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr decrypt_msg(string cipher_msg);

        [DllImport("server_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void key_init();

        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        TcpClient clientSocket = null;
        public Dictionary<TcpClient, string> clientList = null;

        public void startClient(TcpClient clientSocket, Dictionary<TcpClient, string> clientList)
        {
            this.clientSocket = clientSocket;
            this.clientList = clientList;

            Thread t_hanlder = new Thread(doChat);
            t_hanlder.IsBackground = true;
            t_hanlder.Start();
        }

        public delegate void MessageDisplayHandler(string message, string user_name);
        public event MessageDisplayHandler OnReceived;

        public delegate void DisconnectedHandler(TcpClient clientSocket);
        public event DisconnectedHandler OnDisconnected;

        private void doChat()
        {
            NetworkStream stream = null;
            try
            {
                byte[] buffer = new byte[1024];
                string msg = string.Empty;
                int bytes = 0;
                int MessageCount = 0;
                key_init();

                while (true)
                {
                    MessageCount++;
                    stream = clientSocket.GetStream();
                    bytes = stream.Read(buffer, 0, buffer.Length);

                    msg = Encoding.Unicode.GetString(buffer, 0, bytes);

                    msg = msg.Substring(0, msg.IndexOf("$"));

                    if (OnReceived != null)
                        OnReceived(msg, clientList[clientSocket].ToString());


                }
            }
            catch (SocketException err)
            {
                Trace.WriteLine(string.Format("doChat - SocketException : {0}", err.Message));

                if (clientSocket != null)
                {
                    if (OnDisconnected != null)
                        OnDisconnected(clientSocket);

                    clientSocket.Close();
                    stream.Close();
                }
            }
            catch (Exception err)
            {
                Trace.WriteLine(string.Format("doChat - Exception : {0}", err.Message));

                if (clientSocket != null)
                {
                    if (OnDisconnected != null)
                        OnDisconnected(clientSocket);

                    clientSocket.Close();
                    stream.Close();
                }
            }
        }

    }
}
