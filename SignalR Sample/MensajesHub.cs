using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SignalR.Hubs;

namespace SignalR_Sample
{
    public class MensajesHub : Hub
    {
        private static Socket _receiverSocket;
        

        public void OpenSocket()
        {
            StartListening();
        }

        public void EnviarMensaje(string mensaje)
        {
            Clients.enviar(mensaje);
        }

        public void RecibirMensaje(string mensaje)
        {
            if(Clients != null)
                if(!String.IsNullOrEmpty(mensaje))
                    Clients.recibir(mensaje);   
        }


    

        public void StartListening()
        {

            _receiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var receiverSocketAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27878);
            _receiverSocket.Bind(receiverSocketAddress);
            
            var newThread = new Thread(new ThreadStart(SocketListenerThreadStart));
            newThread.Start();
        }

        private void SocketListenerThreadStart()
        {
            int size = 0;
            var byteBuffer = new byte[255];
            _receiverSocket.Listen(1);
            Socket listenerSocket = _receiverSocket.Accept();

            var stopClient = false;

            while (!stopClient)
            {
                try
                {
                    byteBuffer = new byte[255];

                    size = listenerSocket.Receive(byteBuffer, 0, byteBuffer.Length, 0);
                    Array.Resize(ref byteBuffer, size);
                    var messageString = Encoding.UTF8.GetString(byteBuffer);
                    RecibirMensaje(messageString);

                }
                catch (SocketException se)
                {
                    stopClient = true;
                }
            }
        }
    }
}