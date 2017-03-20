using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleAppOtau
{
    class Program
    {
        static void Main(string[] args)
        {
            const string serverIp = "192.168.96.52";
//            const string serverIp = "192.168.96.57";
//            const string serverIp = "172.16.4.10";
//            const int tcpPort = 11834;
            const int tcpPort = 23;

            string serial;
            if (!GetSerial(serverIp, tcpPort, out serial))
            {
                Console.WriteLine($"error {serial}");
                Console.ReadLine();
                return;
            }
            Console.WriteLine($"serial: {serial}");
            int portCount;
            if (GetOwnPortCount(serverIp, tcpPort, out portCount))
                Console.WriteLine($"own port count {portCount}");
            else
                Console.WriteLine($"some error");

            string content;
            GetIniFile(serverIp, tcpPort, out content);

            var ep = GetExtentedPorts(serverIp, tcpPort);

            Console.ReadLine();
        }

        private static bool GetSerial(string serverIp, int tcpPort, out string serial)
        {
            string cmd = "get_rtu_number\r\n";
            return SendCommand(serverIp, tcpPort, cmd, out serial);
        }

        private static bool GetOwnPortCount(string serverIp, int tcpPort, out int ownPortCount)
        {
            string cmd = "otau_get_count_channels\r\n";
            string answer;
            ownPortCount = -1;
            if (!SendCommand(serverIp, tcpPort, cmd, out answer))
                return false;
            if (int.TryParse(answer, out ownPortCount))
               return true;
            ownPortCount = -1;
            return false;
        }

        private static Dictionary<int, string> GetExtentedPorts(string serverIp, int tcpPort)
        {
            string iniContent;
            if (!GetIniFile(serverIp, tcpPort, out iniContent))
                return null; // read iniFile error

            if (iniContent.Substring(0, 15) == "ERROR_COMMAND\r\n")
                return new Dictionary<int, string>(); // charon too old, know nothing about extentions

            if (iniContent.Substring(0, 15) == new string((char)255, 15))
                return new Dictionary<int, string>(); // charon ahs never had ini file, so it hasn't got extentions so far


            if (iniContent.Substring(0, 22) == "[OpticalPortExtension]")
                ParseIniContent(iniContent);

            return null; // some unknown answer
        }

        private static Dictionary<int, string> ParseIniContent(string content)
        {
            return new Dictionary<int, string>();
        }
        private static bool GetIniFile(string serverIp, int tcpPort, out string content)
        {
            string cmd = "ini_read\r\n";
            return SendCommand(serverIp, tcpPort, cmd, out content);
        }

        private static bool SendCommand(string serverIp, int tcpPort, string cmd, out string answer)
        {
            try
            {
                //---create a TCPClient object at the IP and port no.---
                TcpClient client = new TcpClient(serverIp, tcpPort);
                NetworkStream nwStream = client.GetStream();
                byte[] bytesToSend = Encoding.ASCII.GetBytes(cmd);

                //---send the text---
                Console.WriteLine("Sending : \n" + cmd);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                // for bulk command could be needed
                // Thread.Sleep(200);

                //---read back the text---
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                client.Close();
                Console.WriteLine("Received : \n" + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                answer = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                answer = e.Message;
                return false;
            }
        }

    }
}
