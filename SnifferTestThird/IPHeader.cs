using System;
using System.IO;
using System.Net;
using System.Text;

namespace SnifferTestThird
{
    public class IPHeader
    {
        private byte byVersionAndHeaderLenth; // Восемь бит для номера версии и длины заголовка(IHL, DSCP)
        private byte byDifferentiatedServices; // Восемь бит для типа сервиса(Type of Service)
        private ushort usTotalLength; // Шестнадцать бит для общей длины(Total length)
        private ushort usIdentification; // Шестнадцать бит для идентификатора пакета(Identification)
        private ushort usFlagAndOffset; // Шестнадцать бит для флагов и смещения фрагмента(Flags, Fragment offset)
        private byte byTTL; // Восемь бит для времени жизни(Time To Live)
        private byte byProtocol; // Восемь бит для протокола верхнего уровня(Protocol)
        private short sChecksum; // Шестнадцать бит для контрольной суммы(Header Checksum)

        private uint uiSourceIPAddress; // Тридцать два бит для адреса-источника(Source Address)
        private uint uiDestinationIPAddress; // Тридцать два бит для адреса-назначения(Destination Address)

        private byte byHeaderLength; // Длина заголовка
        private byte[] byIPData = new byte[4096]; // Данные содержащиеся в датаграмме

        public IPHeader(byte[] buffer, int received)
        {
            try
            {
                //Создается Memory Stream из полученных байтов
                MemoryStream memoryStream = new MemoryStream(buffer, 0, received);
                // Создаем Binary Reader из MemoryStream
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                // Первые 8 бит IP заголовка содержат версию и длину заголовка.
                // Тут мы получаем версию и длину заголовка 
                byVersionAndHeaderLenth = binaryReader.ReadByte();

                // Следующие 8 бит содержат Differentiated services
                byDifferentiatedServices = binaryReader.ReadByte();

                // Следующие 8 бит занимаю всю длину датаграммы 
                usTotalLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                // Следующие 16 бит занимает идентификатор пакета
                usIdentification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                // Следующие 16 бит занимают флаги и фрагментарное смещение 
                usFlagAndOffset = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                // Следующие 8 бит занимают Time to Live
                byTTL = binaryReader.ReadByte();

                // Следующие 8 бит занимает протокол, содержащийся в датаграмме
                byProtocol = binaryReader.ReadByte();

                //  16 бит для контрольной суммы
                sChecksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                // 32 бита для адреса-источника
                uiSourceIPAddress = (uint)(binaryReader.ReadInt32());

                // 32 бита для адреса-назначения 
                uiDestinationIPAddress = (uint)(binaryReader.ReadInt32());

                // Высчитывается длина заголовка
                byHeaderLength = byVersionAndHeaderLenth;

                // Последние 4 бита byHeaderLength содержит длину заголовка.
                // Выполняются простые арифмитические операции(хз в каком месте простые. (>>) в операторах
                // побитового смещения я не разбираюсь(пока что))
                byHeaderLength <<= 4;
                byHeaderLength >>= 4;
                byHeaderLength *= 4;

                Array.Copy(buffer,
                           byHeaderLength, // Копирование начинается с конца заголовка
                           byIPData, 0,
                           usTotalLength - byHeaderLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SnifferTestThird: ", ex.Message);
            }
        }

        public string Version
        {
            get
            {
                if((byVersionAndHeaderLenth >> 4) == 4)
                {
                    return "IP v4";
                }
                else if((byVersionAndHeaderLenth << 4) == 6)
                {
                    return "IP v6";
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public string HeaderLength
        {
            get
            {
                return byHeaderLength.ToString();
            }
        }

        public ushort MessageLength
        {
            get
            {
                return (ushort)(usTotalLength - byHeaderLength);
            }
        }

        public string DifferentialServices
        {
            get
            {
                return string.Format("0x{0:x2} ({1})", byDifferentiatedServices,
                    byDifferentiatedServices);
            }
        }

        public string Flags
        {
            get
            {
                int nFlags = usFlagAndOffset >> 13;
                if (nFlags == 2)
                {
                    return "Don't fragment";
                }
                else if(nFlags == 1)
                {
                    return "More fragments to come";
                }
                else
                {
                    return nFlags.ToString();
                }
            }
        }

        public string TTL
        {
            get
            {
                return byTTL.ToString();
            }
        }

        public Protocol ProtocolType
        {
            get
            {
                if(byProtocol == 6)
                {
                    return Protocol.TCP;
                }
                else if(byProtocol == 17)
                {
                    return Protocol.UDP;
                }
                else
                {
                    return Protocol.Unknown;
                }
            }
        }

        public string Checksum
        {
            get
            {
                return string.Format("0x{0:x2}", sChecksum);
            }
        }

        public IPAddress SourceAddress
        {
            get
            {
                return new IPAddress(uiSourceIPAddress);
            }
        }

        public IPAddress DestinationAddress
        {
            get
            {
                return new IPAddress(uiDestinationIPAddress);
            }
        }

        public string TotalLength
        {
            get
            {
                return usTotalLength.ToString();
            }
        }

        public string Identification
        {
            get
            {
                return usIdentification.ToString();
            }
        }

        public byte[] Data
        {
            get
            {
                return byIPData;
            }
        }

        public string GetHeaderInformation()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"Version: {Version}, Header Length: {HeaderLength}\n");
            stringBuilder.Append($"Message Length: {MessageLength}, Differential Services: {DifferentialServices}\n"); 
            stringBuilder.Append($"Flags: {Flags}, Time To Live: {TTL}, Protocol: {ProtocolType}, Checksum: {Checksum}\n");
            stringBuilder.Append($"Source Address: {SourceAddress}, Destination Address: {DestinationAddress}\n");
            stringBuilder.Append($"Total Length: {TotalLength}, Identification: {Identification}\n");

            return stringBuilder.ToString();
        }
    }
}
