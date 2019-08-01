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
//using System.Runtime.InteropServices;

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

        //        public delegate void MessageDisplayHandler(string message, string user_name);
        public delegate void MessageDisplayHandler(byte[] message, string user_name);
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

                while (true)
                {
                    MessageCount++;
                    stream = clientSocket.GetStream();
                    if (stream.CanRead)
                    {
                        bytes = stream.Read(buffer, 0, buffer.Length);

                        byte[] temp = new byte[bytes];
                        Array.Copy(buffer, temp, temp.Length);
                        //msg = Encoding.UTF8.GetString(buffer, 0, bytes);

                        if (OnReceived != null)
                            OnReceived(temp, clientList[clientSocket].ToString());
                    }
                    else
                        continue;
                }
            }
            catch (SocketException err)
            {
                Trace.WriteLine(string.Format("[ Error ] - SocketException : {0}", err.Message));

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
                Trace.WriteLine(string.Format("[ Error ] - Exception : {0}", err.Message));

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
