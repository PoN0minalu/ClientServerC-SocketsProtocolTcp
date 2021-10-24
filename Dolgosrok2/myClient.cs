using System;
using System.Net;
using System.Net.Sockets;
using myFirstProtocol;

namespace Dolgosrok2
{
    class Program
    {
        static int port = 8005;
        static string address = "127.0.0.1";
        static void Main(string[] args)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
 
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                TMPD1Packet sendPacket = new TMPD1Packet(1);
                sendPacket.SetPathToFile("/home/svyatoslaw/something");
                
                socket.Send(sendPacket.ToPack());
 
                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
