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
            tempserve = Create();
            tempserve.BeginConnect("127.0.0.1", 12345, ConnectCallback, tempserve);
            System.Threading.Thread.Sleep(999999);

        }

        static Socket Create()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        static void send(Socket psocket)
        {
            Console.WriteLine("改写按1，查询按2，新建按3");
            int input = Convert.ToInt32(Console.ReadLine());

            if (input == 1 ||input==3)
            {
                Console.WriteLine("位置");
                string location_coordinate = Console.ReadLine();
                Console.WriteLine("x轴坐标:");
                string Xcoordinate = Console.ReadLine();
                Console.WriteLine("y轴坐标:");
                string Ycoordinate = Console.ReadLine();
                Console.WriteLine("z轴坐标:");
                string Zcoordinate = Console.ReadLine();

                string tempSendInfo = input + "," + location_coordinate + "," + Xcoordinate + "," + Ycoordinate + "," + Zcoordinate;
                byte[] temSendDate = Encoding.UTF8.GetBytes(tempSendInfo);
                psocket.BeginSend(temSendDate, 0, temSendDate.Length, SocketFlags.None, SendCallback, psocket);
            }
            else if(input==2)
            {
                Console.WriteLine("位置：");
                string location_coordinate = Console.ReadLine();
                string tempSendInfo = input + "," + location_coordinate;
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
                System.Threading.Thread.Sleep(1000);
                send(tempSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送失败"+ex.Message);
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
                Console.WriteLine("连接异常，请检查网络"+ex.Message);
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
                Console.WriteLine("接收失败" + ex.Message);
            }
        }

    }
}


