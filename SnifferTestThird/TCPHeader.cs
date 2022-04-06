using System;
using System.IO;
using System.Net;
using System.Text;

namespace SnifferTestThird
{
    public class TCPHeader
    {
        private ushort usSourcePort;
        private ushort usDestinationPort;
        private uint uiSequenceNumber;
        private uint uiAcknowledgementNumber;
        private ushort usDataOffsetAndFlags;
        private ushort usWindow;
        private short sChecksum;

        private ushort usUrgentPointer;

        private byte byHeaderLength;
        private ushort usMessageLength;
        private byte[] byTCPData = new byte[4096];

        public TCPHeader(byte[] buffer, int received)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream(buffer, 0, received);
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                usSourcePort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usDestinationPort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                uiSequenceNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                uiAcknowledgementNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());

                usDataOffsetAndFlags = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usWindow = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                sChecksum = (short)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                usUrgentPointer = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                byHeaderLength = (byte)(usDataOffsetAndFlags >> 12);
                byHeaderLength *= 4;

                usMessageLength = (ushort)(received - byHeaderLength);

                Array.Copy(buffer, byHeaderLength, byTCPData, 0, received - byHeaderLength);
            }
            catch(Exception ex)
            {
                Console.WriteLine("SnifferTestThird: ", ex.Message);
            }
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

        public string SequenceNumber
        {
            get
            {
                return uiSequenceNumber.ToString();
            }
        }

        public string AcknowledgementNumber
        {
            get
            {
                if ((usDataOffsetAndFlags & 0x10) != 0)
                {
                    return uiAcknowledgementNumber.ToString();
                }
                else
                    return "";
            }
        }

        public string HeaderLength
        {
            get
            {
                return byHeaderLength.ToString();
            }
        }

        public string WindowSize
        {
            get
            {
                return usWindow.ToString();
            }
        }

        public string UrgentPointer
        {
            get
            {
                if ((usDataOffsetAndFlags & 0x20) != 0)
                {
                    return usUrgentPointer.ToString();
                }
                else
                    return "";
            }
        }

        public string Flags
        {
            get
            {
                int nFlags = usDataOffsetAndFlags & 0x3F;

                string strFlags = string.Format("0x{0:x2} (", nFlags);

                if((nFlags & 0x01) != 0)
                {
                    strFlags += "FIN, ";
                }
                if((nFlags & 0x02) != 0)
                {
                    strFlags += "SYN, ";
                }
                if((nFlags & 0x04) != 0)
                {
                    strFlags += "RST, ";
                }
                if((nFlags & 0x08) != 0)
                {
                    strFlags += "PSH, ";
                }
                if((nFlags & 0x10) != 0)
                {
                    strFlags += "ACK, ";
                }
                if((nFlags & 0x20) != 0)
                {
                    strFlags += "URG";
                }
                strFlags += ")";

                if (strFlags.Contains("()"))
                {
                    strFlags = strFlags.Remove(strFlags.Length - 3);
                }
                else if (strFlags.Contains(", )"))
                {
                    strFlags = strFlags.Remove(strFlags.Length - 3, 2);
                }

                return strFlags;
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
                return byTCPData;
            }
        }

        public ushort MessageLength
        {
            get
            {
                return usMessageLength;
            }
        }

        public string GetHeaderInformation()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"Source Port: {SourcePort}, Destination Port: {DestinationPort}\n");
            stringBuilder.Append($"Sequence Number: {SequenceNumber}, " +
                                 $"Acknowledgement Number: {AcknowledgementNumber}\n");
            stringBuilder.Append($"Header Length: {HeaderLength}, Window Size: {WindowSize}, Urgent Pointer: {UrgentPointer}\n");
            stringBuilder.Append($"Flags: {Flags}, Checksum: {Checksum}, Message Length {MessageLength}\n\n");

            return stringBuilder.ToString();
        }
    }
}
