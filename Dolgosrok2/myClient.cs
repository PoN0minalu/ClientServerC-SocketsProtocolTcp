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
                //==========================================================
                TMPD1Packet sendPacket = new TMPD1Packet(3);     
                sendPacket.SetFileBytes("/home/svyatoslaw/tests/testProgram/bin/Debug/net5.0/testProgram");
                sendPacket.SetPathToFile("/home/svyatoslaw/tests/subtests");           
                socket.Send(sendPacket.ToPack());
                //==========================================================
                // получаем ответ
                byte[] data = new byte[150000000]; // буфер для ответа
                
                int bytes = 0; // количество полученных байт
 
                do
                {
                    bytes = socket.Receive(data);
                }
                while (socket.Available > 0);

                TMPD1Packet getPacket = new TMPD1Packet(0);
                getPacket = TMPD1Packet.ToParse(data);
                ManagerOfPackets boss = new ManagerOfPackets(getPacket);
                boss.DirtyWork();

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
