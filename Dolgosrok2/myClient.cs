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
        static int port = 1924;
        static string address = "192.168.1.7";
        static void Main(string[] args)
        {
            try
            {
                //IPAddress ip = Dns.GetHostEntry("192.168.42.129").AddressList[0]; 
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);//IPAddress.Parse(address), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                Console.WriteLine("Connection succesed");
                while (true)
                {

                    Interface face = new Interface();
                    int toDo = face.GetDo();
                    TMPD1Packet sendPacket = new TMPD1Packet(toDo);
                    switch(toDo)
                    {
                        case 1:
                            sendPacket.SetPathToFile(face.GetResult());
                            if (face.GetParam() == null)
                            {
                                sendPacket.SetParamsOfExe(face.GetParam());
                            }
                            break;
                        case 2:
                            sendPacket.SetPathToFile(face.GetResult());
                            break;
                        case 3:
                            sendPacket.SetFileBytes(face.GetResult());
                            sendPacket.SetPathToFile(face.GetFinal());
                            break;
                        case 4:
                            sendPacket.SetPathToFile(face.GetFinal());
                            sendPacket.SetPathToGetFile(face.GetFinal());
                            break;
                    }
                    socket.Send(sendPacket.ToPack());
                    // получаем ответ
                    byte[] data = new byte[15000000]; // буфер для ответа
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
