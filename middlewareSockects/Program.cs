using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace middlewareSockects
{
    class Program
    {
        static void Main(string[] args)
        {
            StartNumbersOutput();
            StartListener();
        }

        private static void StartNumbersOutput()
        {
            var threadNumbers = new Thread(RandomNumbers.CreateAndSendNumbers);
            threadNumbers.Start();
        }
        private static void StartListener()
        {
            var receiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Port 27877
            var receiverSocketAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27877);
            receiverSocket.Bind(receiverSocketAddress);
            receiverSocket.Listen(1);
            var mySocket = new SocketWorker(receiverSocket);
            mySocket.StartSocketListener();
            receiverSocket.Close();
        }
    }
}
