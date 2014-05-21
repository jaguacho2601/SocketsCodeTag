using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace middlewareSockects
{
    public class RandomNumbers
    {
        public static void CreateAndSendNumbers()
        {
            bool stop = false;
            var socketOutStream = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var portClient = new IPEndPoint(IPAddress.Parse(
                ConfigurationSettings.AppSettings["ip"]),
                                            int.Parse(ConfigurationSettings.AppSettings["portCreator"]));
            try
            {
                socketOutStream.Connect(portClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exeption ==> " + ex.Message);
                Console.ReadLine();
            }
            
            while (!stop)
            {
                try
                {
                    byte[] sendingString = Encoding.UTF8.GetBytes(new Random().Next(21).ToString());
                    socketOutStream.Send(sendingString, 0, sendingString.Length, 0);
                    Console.WriteLine(String.Format("Sending Value ==> {0}", System.Text.Encoding.UTF8.GetString(sendingString)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    stop = true;
                    Console.ReadLine();
                    socketOutStream.Close();
                }
              
                Thread.Sleep(250);
            }

            socketOutStream.Close();
        }
    }
}
