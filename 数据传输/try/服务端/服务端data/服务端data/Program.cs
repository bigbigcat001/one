using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Data;
using CSharpConnectMySQL;

namespace Console_try
{

    internal class Program
    {
        public struct ClientData
        {
            public Socket socket;
            public byte[] readBuffer;
        }
        static private byte[] readBuffer = new byte[1024];
        static private Dictionary<Socket, ClientData> clientDic = new Dictionary<Socket, ClientData>();
        static void Main(string[] args)
        {
            Socket tempserve = Create();
            BindAndListen(tempserve);
            tempserve.BeginAccept(AcceptCallback, tempserve);
            System.Threading.Thread.Sleep(99999999);


        }

        static Socket Create()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        static void BindAndListen(Socket psocket)
        {

            IPEndPoint temEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            psocket.Bind(temEndPoint);
            psocket.Listen(3);
            Console.WriteLine("服务端启动成功");
        }
        static void send(Socket psocket)
        {
            string tempSendInfo = Console.ReadLine();

            byte[] temSendDate = Encoding.UTF8.GetBytes(tempSendInfo);
            psocket.BeginSend(temSendDate, 0, temSendDate.Length, SocketFlags.None, SendCallback, psocket);
        }
        static void Receive(Socket psocket)
        {
            Byte[] temReadBuffer = new Byte[1024];
            psocket.Receive(temReadBuffer);
            string temReadString = Encoding.UTF8.GetString(temReadBuffer);
            Console.WriteLine("客户端: " + temReadString);
        }



        static void Close(Socket psocket)
        {
            psocket.Close();
        }
        /*static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket tempSocket = (Socket)ar.AsyncState;
                tempSocket.EndConnect(ar);
                Console.WriteLine("连接客户端成功");
                send(tempSocket);
                tempSocket.BeginReceive(readBuffer, 0, 1024, SocketFlags.None, ReceiveCallback, tempSocket);

                send(tempSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("连接异常，请检查网络");
            }
        }*/

        static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {

                Socket tempSocket = (Socket)ar.AsyncState;
                int tempCount = tempSocket.EndReceive(ar);
                if (tempCount == 0)
                {
                    Console.WriteLine(((IPEndPoint)tempSocket.RemoteEndPoint) + "已断开");
                    tempSocket.Close();
                    clientDic.Remove(tempSocket);
                }
                else
                {
                    string tempReadString = Encoding.UTF8.GetString(clientDic[tempSocket].readBuffer);
                    tempReadString = tempReadString.TrimEnd('\0');//去除字符\0
                    string temSendInfo1 = sql_Data(tempReadString);

                    byte[] temSenddata1 = Encoding.UTF8.GetBytes(temSendInfo1);
                    tempSocket.Send(temSenddata1);



                    /* if (tempReadString.Contains("再见") || tempReadString.Contains("断开"))
                     {
                         String temSendInfo1 = "再见";
                         byte[] temSenddata1 = Encoding.UTF8.GetBytes(temSendInfo1);
                         tempSocket.Send(temSenddata1);
                     }

                     else
                     {
                         readBuffer = new Byte[readBuffer.Length];
                         tempSocket.BeginReceive(clientDic[tempSocket].readBuffer, 0, 1024, SocketFlags.None, ReceiveCallback, tempSocket);
                     }*/



                }
                readBuffer = new Byte[readBuffer.Length];
                tempSocket.BeginReceive(clientDic[tempSocket].readBuffer, 0, 1024, SocketFlags.None, ReceiveCallback, tempSocket);

            }
            catch (Exception ex)
            {
                Console.WriteLine("接收失败"+ex.Message);
            }

        }

        static string sql_Data(string tempReadString)
        {
            string[] strings = tempReadString.Split(',');
            if (strings[0] == "3")
            {
                return MySqlHelper.sql_xyzDatainsert(strings[1], strings[2], strings[3], strings[4]);
            }
            else if (strings[0] == "1")
            {
                return MySqlHelper.xyz_enter(strings[1], strings[2], strings[3], strings[4]);
            }
            else if (strings[0] == "2")
            {
                string[] xyz_return = MySqlHelper.sql_XYZDataQuery(strings[1]).Split(',');
                return strings[1]+":"+ xyz_return[0]+"-"+xyz_return[1]+"-"+xyz_return[2];

            }
            return "操作失败";

        }
        /// <summary>
        /// 去除byte[]数组缓冲区内的尾部空白区;从末尾向前判断;
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] bytesTrimEnd(byte[] bytes)
        {
            List<byte> list = bytes.ToList();
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i] == 0x00)
                {
                    list.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
            return list.ToArray();
        }
        static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket tempServeSocket = (Socket)ar.AsyncState;
                Socket temClientSocket = tempServeSocket.EndAccept(ar);

                ClientData tempClientData = new ClientData();
                tempClientData.socket = temClientSocket;
                tempClientData.readBuffer = new Byte[1024];

                clientDic.Add(temClientSocket, tempClientData);//加入链表，方便管理
                Console.WriteLine(((IPEndPoint)temClientSocket.RemoteEndPoint) + "已接入");
                tempServeSocket.BeginAccept(AcceptCallback, tempServeSocket);//不断接收
                temClientSocket.BeginReceive(tempClientData.readBuffer, 0, 1024, SocketFlags.None, ReceiveCallback, temClientSocket);

            }
            catch (Exception ex)
            {
                Console.WriteLine("接入失败"+ex.Message);
            }
        }
        static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket tempSocket = (Socket)ar.AsyncState;
                tempSocket.EndSend(ar);
                Console.WriteLine("发送成功");
                send(tempSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送失败" + ex.Message);
            }
        }
    }
}


