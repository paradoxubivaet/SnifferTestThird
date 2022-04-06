using System;
using System.IO;
using System.Net;
using System.Text;

namespace SnifferTestThird
{
    public class UDPHeader
    {
        private ushort usSourcePort;
        private ushort usDestinationPort;

        private ushort usHeaderLength;
        private short sChecksum;

        private byte[] byUDPata = new byte[4096];

        public UDPHeader(byte[] buffer, int received)
        {
            MemoryStream memoryStream = new MemoryStream(buffer, 0 , received);
            BinaryReader binaryReader = new BinaryReader(memoryStream);

            usSourcePort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            usDestinationPort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            usHeaderLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            sChecksum = (short)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            Array.Copy(buffer,
                       8,
                       byUDPata,
                       0,
                       received - 8);
        }

        public string SourcePort
        {
            get
            {
                return usSourcePort.ToString();
            }
        }

        public string DestinationPort
        {
            get
            {
                return usDestinationPort.ToString();
            }
        }

        public string HeaderLength
        {
            get
            {
                return usHeaderLength.ToString();
            }
        }

        public string Checksum
        {
            get
            {
                return string.Format("0x{0:x2}", sChecksum);
            }
        }

        public byte[] Data
        {
            get
            {
                return byUDPata;
            }
        }

        public string GetHeaderInformation()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"Source Port: {SourcePort}, Destination Port: {DestinationPort}\n");
            stringBuilder.Append($"Header Length: {HeaderLength}, Checksum: {Checksum}\n\n");

            return stringBuilder.ToString();
        }
    }
}
