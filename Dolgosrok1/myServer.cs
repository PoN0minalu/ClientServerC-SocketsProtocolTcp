using System;
using myFirstProtocol;
using System.Net;
using System.Net.Sockets;

namespace Dolgosrok1
{
    class Program
    {
        static void Main(string[] args)
        {   
           // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005);
             
            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);
 
                // начинаем прослушивание
                listenSocket.Listen(100);
 
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
 
                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[150000000]; // буфер для получаемых данных
 
                    do
                    {
                        bytes = handler.Receive(data);
                    }
                    while (handler.Available>0);

                    TMPD1Packet getPacket = new TMPD1Packet(0);
                    getPacket = TMPD1Packet.ToParse(data);

                    ManagerOfPackets boss = new ManagerOfPackets(getPacket);
                    handler.Send(boss.DirtyWork().ToPack());
        
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
