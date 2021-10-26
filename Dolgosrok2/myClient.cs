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
                    int toDo = face.GetDo();
                    TMPD1Packet sendPacket = new TMPD1Packet(toDo);
                    switch(toDo)
                    {
                        case 1:
                            sendPacket.SetPathToFile(face.GetResult());
                            sendPacket.SetPathToFile(face.GetParam());
                            break;
                        case 2:
                            sendPacket.SetPathToFile(face.GetResult());
                            break;
                    }
                    if (toDo == 3 || toDo == 4)
                    {
                        sendPacket.SetPathToFile(face.GetResult());
                        sendPacket.SetPathToFile(face.GetFinal());
                    }
                    sendPacket.SetPathToFile(face.GetResult());
                    socket.Send(sendPacket.ToPack());
                    // получаем ответ
                    byte[] data = new byte[256]; // буфер для ответа
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
                    Console.WriteLine("\nBackspace - menu\nESC - exit");
                    face.CheckBackESC();
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
