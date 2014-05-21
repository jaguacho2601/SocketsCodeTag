using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace middlewareSockects
{
    public class SocketWorker
    {

        #region atributes
        private Socket _socketC = null;
        private Socket _socketOut = null;
        private bool stop = false;
        private Thread mainThread = null;
        private List<int> _numbersList;
        #endregion

        public SocketWorker(Socket clientSocket)
        {
            _socketC = clientSocket.Accept();
        }

  
        
        public void StartSocketListener()
        {

            _socketOut = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var sumSocketAddress = new IPEndPoint(IPAddress.Parse(ConfigurationSettings.AppSettings["ip"]), int.Parse(ConfigurationSettings.AppSettings["portSum"]));
            _socketOut.Connect(sumSocketAddress);

            if (_socketC != null)
            {
                mainThread =
                    new Thread(new ThreadStart(SocketListenerThreadStart));

                mainThread.Start();
            }
        }
        
        private void SocketListenerThreadStart()
        {
            _numbersList = new List<int>();
               
            new Timer(AddCurrentValues, null, 1000, 1000);

            while (!stop)
            {
                try
                {
                    var byteBuffer = new byte[255];
                    var size = _socketC.Receive(byteBuffer, 0, byteBuffer.Length, 0);
                    ValidateAndSaveInput(ref stop, byteBuffer, size);

                }
                catch (SocketException se)
                {
                    stop = true;
                    Dispose();
                }
            }
        }

        private void AddCurrentValues(object state)
        {
            var resultSum = _numbersList.Sum();
            _numbersList = new List<int>();
            byte[] sendingString = Encoding.UTF8.GetBytes(resultSum.ToString());
            _socketOut.Send(sendingString, 0, sendingString.Length, 0);
            Console.WriteLine("Number Sum:" + resultSum);
        }


        private void ValidateAndSaveInput(ref bool mStopClient, byte[] byteBuffer, int size)
        {
            Array.Resize(ref byteBuffer, size);
            var messageString = Encoding.UTF8.GetString(byteBuffer);
            var valueNum = 0;
            if (int.TryParse(messageString, out valueNum))
            {
                _numbersList.Add(valueNum);
            }
            else
            {
                mStopClient = false;
            }

            Console.WriteLine("New Number Sent: " + Encoding.UTF8.GetString(byteBuffer));
        }


        public void Dispose()
        {
            if (_socketC == null) return;
            stop = true;
            _socketC.Close();
            mainThread.Join(1000);
            if (mainThread.IsAlive)
            {
                mainThread.Abort();
            }
            mainThread = null;
            _socketC = null;
        }
        
    }
}
