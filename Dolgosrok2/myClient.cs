using System;
using System.Net;
using System.Net.Sockets;
using myFirstProtocol;
using System.Windows.Input;
using System.Text;

namespace Dolgosrok2
{
    class Interface 
    {
        public Interface() 
        {
            StartInterface();
        }

        public void StartInterface()
        {
            Console.WriteLine("\nChoose that you want to do:\n1 - Start program with parameters\n2 - Download file\n3 - Delete file\n4 - Upload file\nEsc - exit from program");
            this._key = Console.ReadKey();
            switch(this._key.Key)
            {
                case ConsoleKey.D1:
                    LaunchInterface();
                    break;
                case ConsoleKey.D2:
                    UploadInterface();
                    break;
                case ConsoleKey.D3:
                    DownloadInterface();
                    break;
                case ConsoleKey.D4:
                    DeleteInterface();
                    break;
                case ConsoleKey.Escape:
                    System.Environment.Exit(0);
                    break;
            }
        }
        public void LaunchInterface()
        {
            Console.WriteLine("\nEnter - If you realy want to launch program\nESC - exit\nBackspace - Cumback to the start menu\n");
            this._key =  Console.ReadKey();
            CheckBackESC();
            CreateResult();
        }
        public void UploadInterface()
        {
            Console.WriteLine("\nEnter - If you realy want to upload file\nESC - exit\nBackspace - Cumback to the start menu\n");
            this._key =  Console.ReadKey();
            CheckBackESC();
            CreateResult();
        }
        public void DownloadInterface()
        {
            Console.WriteLine("\nEnter - If you realy want to download file\nESC - exit\nBackspace - Cumback to the start menu\n");
            this._key =  Console.ReadKey();
            CheckBackESC();
            CreateResult();
        }
        public void DeleteInterface()
        {
            Console.WriteLine("\nEnter - If you realy want to delete file\nESC - exit\nBackspace - Cumback to the start menu\n");
            this._key =  Console.ReadKey();
            CheckBackESC();
            CreateResult();
        }

        public void CheckBackESC()
        {
            switch(this._key.Key)
            {
                case ConsoleKey.Backspace:
                    StartInterface();
                    break;
                case ConsoleKey.Escape:
                    System.Environment.Exit(0);
                    break;
            }
        }

        public void CreateResult()
        {
            Console.Clear();
            Console.WriteLine("\nWrite directory of file");
            this._result = Convert.ToString(Console.ReadLine());
            Console.Clear();
            Console.WriteLine("\nYou can comeback to start or exit\nBackspace- start page\nESC - exit");
            CheckBackESC();
        }

        public string GetResult()
        {
            return this._result;
        }
        private ConsoleKeyInfo _key;
        private string _result;
    }

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
                while(true)
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
