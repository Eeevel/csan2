using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace laba2
{
    class Tracert
    {
        private const int WAITING_TIME = 3000;
        private const int JUMPS_COUNT = 30;
        private const int PACKAGE_COUNT = 3;

        private static void ViewDNS(EndPoint endPoint)
        {
            try
            {
                Console.WriteLine(GetDNS(endPoint) + " [" + endPoint + ']');
            }
            catch
            {
                Console.WriteLine(endPoint);
            }
        }

        private static string GetDNS(EndPoint endPoint)
        {
            return Dns.GetHostEntry(IPAddress.Parse(endPoint.ToString().Split(':')[0])).HostName;
        }

        public static void Perform(string host, bool viewDNS)
        {
            Icmp icmp = new Icmp();

            IPHostEntry ipHost;
            try
            {
                ipHost = Dns.GetHostEntry(host);
            }
            catch (SocketException)
            {
                Console.WriteLine("Не удается разрешить системное имя узла " + host);
                return;
            }

            IPEndPoint ip = new IPEndPoint(ipHost.AddressList[0], 0);
            EndPoint endPoint = ip;

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            socket.ReceiveTimeout = WAITING_TIME;
            
            bool isFinish = false;
            byte[] buffer;
            byte[] package = icmp.GetPackage();

            Console.WriteLine("Трассировка маршрута к " + host);
            Console.WriteLine("с максимальным количеством прыжков " + JUMPS_COUNT + ": \n");
            for (short i = 1; i <= JUMPS_COUNT; i++)
            {
                Console.Write("{0,3}", i);
                int errorCount = 0;
                socket.Ttl = i;

                for (int j = 1; j <= PACKAGE_COUNT; j++)
                {
                    DateTime start = DateTime.Now;
                    buffer = new byte[128];

                    try
                    {
                        socket.SendTo(package, ip);
                        socket.ReceiveFrom(buffer, ref endPoint);

                        TimeSpan time = DateTime.Now - start;

                        Icmp reply = new Icmp();
                        reply.GetType(buffer);

                        if (reply.type == 11)
                        {
                            Console.Write("{0,9}", time.Milliseconds + " мс ");
                        }

                        if (reply.type == 0)
                        {
                            Console.Write("{0,9}", time.Milliseconds + " мс ");
                            isFinish = true;
                        }
                    }
                    catch (SocketException)
                    {
                        Console.Write("{0,8}", "*");
                        errorCount++;
                    }
                }

                if (errorCount == PACKAGE_COUNT)
                    Console.WriteLine("  Превышен интервал ожидания для запроса.");
                else if (viewDNS)
                    ViewDNS(endPoint);
                else
                    Console.WriteLine(endPoint);

                if (isFinish)
                {
                    Console.WriteLine("\nТрассировка завершена.");
                    break;
                }
            }
            socket.Close();
        }
    }
}
