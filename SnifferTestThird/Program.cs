using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SnifferTestThird
{
    class Program
    { 
        private static Socket mainSocket;
        private static byte[] byteData = new byte[4096];
        private static bool continuecapturing = false;
        private static FileStream fileStream;
        static void Main(string[] args)
        {
            if (!continuecapturing)
            {
                var ip = IPAddress.Parse(args[0]);
                var filePath = args[1];

                continuecapturing = true;

                fileStream = new FileStream(filePath, FileMode.OpenOrCreate);

                mainSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Raw, ProtocolType.IP);

                mainSocket.Bind(new IPEndPoint(ip, 0));
                mainSocket.SetSocketOption(SocketOptionLevel.IP,
                                           SocketOptionName.HeaderIncluded,
                                           true);

                byte[] byTrue = new byte[4] { 1, 0, 0, 0 };
                byte[] byOut = new byte[4] { 1, 0, 0, 0 };

                mainSocket.IOControl(IOControlCode.ReceiveAll,
                                     byTrue, byOut);


                mainSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                                        new AsyncCallback(OnReceive), null);

                Console.ReadLine();
            }
            else
            {
                continuecapturing = false;
                mainSocket.Close();
            }
        }

        private static void OnReceive(IAsyncResult result)
        {
            try
            {
                int received = mainSocket.EndReceive(result);

                ParseData(byteData, received);

                if(continuecapturing)
                {
                    byteData = new byte[4096];

                    mainSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                                            new AsyncCallback(OnReceive), null);
                }
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("SnifferTestThird[Ошибка в методе OnReceive[DisposedException]]: ", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SnifferTestThird[Ошибка в методе OnReceive[Exception]]: ", ex.Message);
            }
        }

        private static void ParseData(byte[] byteData, int received)
        {
            IPHeader iPHeader = new IPHeader(byteData, received);
            byte[] header;

            byte[] ipHeader = Encoding.UTF8.GetBytes(iPHeader.GetHeaderInformation());
            Console.WriteLine(iPHeader.GetHeaderInformation());
            fileStream.Write(ipHeader);
            fileStream.Flush();

            Console.WriteLine("-----------------------------------------------");
            switch (iPHeader.ProtocolType)
            {
                case Protocol.TCP:
                    TCPHeader tcpHeader = new TCPHeader(iPHeader.Data,
                                                        iPHeader.MessageLength);

                    header = Encoding.UTF8.GetBytes(tcpHeader.GetHeaderInformation());
                    Console.WriteLine(tcpHeader.GetHeaderInformation());
                    fileStream.Write(header);
                    fileStream.Flush();
                    break;

                case Protocol.UDP:
                    UDPHeader udpHeader = new UDPHeader(iPHeader.Data,
                                                        (int)iPHeader.MessageLength);

                    header = Encoding.UTF8.GetBytes(udpHeader.GetHeaderInformation());
                    Console.WriteLine(udpHeader.GetHeaderInformation());
                    fileStream.Write(header);
                    fileStream.Flush();
                    break;

                case Protocol.Unknown:
                    break;
            }
        }
    }
}
