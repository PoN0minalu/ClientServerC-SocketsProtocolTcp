using Microsoft.VisualStudio.TestTools.UnitTesting;
using myInterface;
using myClient;
using myFirstProtocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Input;
using System.Text;


namespace ClientTests
{
    [TestClass]
    public class TestsLaunchAndDelete
    {
        [TestMethod]
        public void LaunchTestWithoutParam()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("192.168.1.7"), 1924);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            TMPD1Packet sendPacket = new TMPD1Packet(1);
            sendPacket.SetPathToFile("/home/svyatoslaw/tests/testProgram/bin/Debug/net5.0/testProgram");
            socket.Send(sendPacket.ToPack());
            byte[] data = new byte[15000000];
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data);
            }
            while (socket.Available > 0);
            TMPD1Packet getPacket = new TMPD1Packet(0);
            getPacket = TMPD1Packet.ToParse(data);
            ManagerOfPackets boss = new ManagerOfPackets(getPacket);
            Assert.AreEqual("Hello World!", boss.DirtyWork().GetReply());
        }

        [TestMethod]
        public void LaunchTestWithParam()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("192.168.1.7"), 1924);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            TMPD1Packet sendPacket = new TMPD1Packet(1);
            sendPacket.SetPathToFile("/home/svyatoslaw/tests/testProgram/bin/Debug/net5.0/testProgram");
            sendPacket.SetParamsOfExe("-h");
            socket.Send(sendPacket.ToPack());
            byte[] data = new byte[15000000];
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data);
            }
            while (socket.Available > 0);
            TMPD1Packet getPacket = new TMPD1Packet(0);
            getPacket = TMPD1Packet.ToParse(data);
            ManagerOfPackets boss = new ManagerOfPackets(getPacket);
            Assert.AreEqual("Данная программа выводит фразу: Hello World!", boss.DirtyWork().GetReply());
        }

        [TestMethod]
        public void DeleteTest()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("192.168.1.7"), 1924);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            TMPD1Packet sendPacket = new TMPD1Packet(2);
            sendPacket.SetPathToFile("/home/svyatoslaw/tests/check.txt");
            socket.Send(sendPacket.ToPack());
            byte[] data = new byte[15000000];
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data);
            }
            while (socket.Available > 0);
            TMPD1Packet getPacket = new TMPD1Packet(0);
            getPacket = TMPD1Packet.ToParse(data);
            ManagerOfPackets boss = new ManagerOfPackets(getPacket);
            Assert.AreEqual("файл успешно удалён", boss.DirtyWork().GetReply());
        }
        [TestMethod]
        public void UploadTest()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("192.168.1.7"), 1924);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            TMPD1Packet sendPacket = new TMPD1Packet(3);
            sendPacket.SetFileBytes("D://checkMAIN.txt");
            sendPacket.SetPathToFile("/home/svyatoslaw/tests/subtests");
            socket.Send(sendPacket.ToPack());
            byte[] data = new byte[15000000];
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data);
            }
            while (socket.Available > 0);
            TMPD1Packet getPacket = new TMPD1Packet(0);
            getPacket = TMPD1Packet.ToParse(data);
            ManagerOfPackets boss = new ManagerOfPackets(getPacket);
            Assert.AreEqual("файл успешно загрузился", boss.DirtyWork().GetReply());
        }
        [TestMethod]
        public void DownloadTest()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("192.168.1.7"), 1924);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            TMPD1Packet sendPacket = new TMPD1Packet(4);
            sendPacket.SetPathToFile("/home/svyatoslaw/tests/subtests/Sum.cpp");
            sendPacket.SetPathToGetFile("D://");
            socket.Send(sendPacket.ToPack());
            byte[] data = new byte[15000000];
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data);
            }
            while (socket.Available > 0);
            TMPD1Packet getPacket = new TMPD1Packet(0);
            getPacket = TMPD1Packet.ToParse(data);
            ManagerOfPackets boss = new ManagerOfPackets(getPacket);
            Assert.AreEqual("файл успешно загрузился", boss.DirtyWork().GetReply());
        }
    }
}
