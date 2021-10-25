using System;
using System.Net;
using System.Net.Sockets;
using myFirstProtocol;
using myInterface;
using System.Windows.Input;
using System.Text;

namespace myClient
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
                while (true)
                {

                    Interface face = new Interface();
                    TMPD1Packet sendPacket = new TMPD1Packet(1);
                    sendPacket.SetPathToFile(face.GetResult());
                    socket.Send(sendPacket.ToPack());
                }
                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
