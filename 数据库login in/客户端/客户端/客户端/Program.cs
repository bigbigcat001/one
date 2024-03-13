using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Console_try
{
    internal class Program
    {
        static private byte[] tempReadBuffer = new byte[1024];
        static Socket tempserve;
        static void Main(string[] args)
        {
            Thread thread1 = new Thread(Task1);
            thread1.Start();
            System.Threading.Thread.Sleep(999999);

        }
        static void Task1()
        {
            tempserve = Create();
            tempserve.BeginConnect("127.0.0.1", 12345, ConnectCallback, tempserve);

        }


        static Socket Create()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        static void send(Socket psocket)
        {
            Console.WriteLine("登录按1，注册按2");
            int input = Convert.ToInt32(Console.ReadLine());

            if (input == 1||input==2)
            {
                Console.WriteLine("请输入账号:");
                string uersname= Console.ReadLine();
                Console.WriteLine("请输入密码:");
                string password = Console.ReadLine();

                string tempSendInfo = input.ToString()+","+uersname+","+password;
                byte[] temSendDate = Encoding.UTF8.GetBytes(tempSendInfo);
                psocket.BeginSend(temSendDate, 0, temSendDate.Length, SocketFlags.None, SendCallback, psocket);

            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 1 or 2.");
            }
            
           // string tempSendInfo = Console.ReadLine();
            
        }
        static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket tempSocket = (Socket)ar.AsyncState;
                tempSocket.EndSend(ar);
                Console.WriteLine("成功");
                tempReadBuffer = new byte[tempReadBuffer.Length];
                tempSocket.BeginReceive(tempReadBuffer, 0, 1024, SocketFlags.None, ReceiveCallback, tempSocket);
                send(tempSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送失败");
            }
        }
        static void Close(Socket psocket)
        {
            psocket.Close();
        }
        static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket tempSocket = (Socket)ar.AsyncState;
                tempSocket.EndConnect(ar);
                Console.WriteLine("连接服务器成功");

                
                send(tempSocket);

            }
            catch (Exception ex)
            {
                Console.WriteLine("连接异常，请检查网络");
            }
        }

        static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket tempSocket = (Socket)ar.AsyncState;
                tempSocket.EndReceive(ar);

                string tempReadString = Encoding.UTF8.GetString(tempReadBuffer);
                Console.WriteLine("接收到服务端信息：" + tempReadString);
                if (tempReadString.Contains("再见") || tempReadString.Contains("断开"))
                {
                    tempSocket.Shutdown(SocketShutdown.Both);
                    Console.WriteLine(((IPEndPoint)tempSocket.RemoteEndPoint) + "已从服务器断开,请关闭程序");
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("接收失败");
            }
        }

    }
}


