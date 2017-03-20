using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ConsoleAppOtau
{
    class Program
    {
        static void Main()
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


            if (EraseAdditionalOtauFromIni(serverIp, tcpPort, 1))
                Console.WriteLine($"detached successfully");

            if (InsertAdditionalOtauToIni(serverIp,tcpPort,1,"192.168.96.57",11834))
                Console.WriteLine($"detached successfully");

            var extendedPorts = GetExtentedPorts(serverIp, tcpPort);
            foreach (var pair in extendedPorts)
                Console.WriteLine($"otau {pair.Value} is attached to port {pair.Key}");

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

        private static bool InsertAdditionalOtauToIni(string serverIp, int tcpPort, int fromOpticalPort, string addOtauIp, int addOtauTcpPort)
        {
            var extPorts = GetExtentedPorts(serverIp, tcpPort);
            if (extPorts.ContainsKey(fromOpticalPort))
            {
                Console.WriteLine("This is extended port already. Denied.");
                return true;
            }
            extPorts.Add(fromOpticalPort, $"{addOtauIp}:{addOtauTcpPort}");
            var content = DictionaryToContent(extPorts);
            string answer;
            return SendWriteIniCommand(serverIp, tcpPort, content, out answer);
        }
        private static bool EraseAdditionalOtauFromIni(string serverIp, int tcpPort, int fromOpticalPort)
        {
            var extPorts = GetExtentedPorts(serverIp, tcpPort);
            if (!extPorts.ContainsKey(fromOpticalPort))
            {
                Console.WriteLine("There is no such extended port. Nothing to do.");
                return true;
            }

            extPorts.Remove(fromOpticalPort);
            var content = DictionaryToContent(extPorts);
            string answer;
            return SendWriteIniCommand(serverIp, tcpPort, content, out answer);
        }

        private static string DictionaryToContent(Dictionary<int, string> extPorts)
        {
            if (extPorts.Count == 0)
                return "\r\n";
            var result = "[OpticalPortExtension]\r\n";
            foreach (var extPort in extPorts)
                result += $"{extPort.Key}={extPort.Value}\r\n";
            return result;
        }

        private static Dictionary<int, string> GetExtentedPorts(string serverIp, int tcpPort)
        {
            string iniContent;
            if (!ReadIniFile(serverIp, tcpPort, out iniContent))
                return null; // read iniFile error

            if (iniContent.Substring(0, 15) == "ERROR_COMMAND\r\n")
                return new Dictionary<int, string>(); // charon too old, know nothing about extensions

            if (iniContent.Substring(0, 22) == "[OpticalPortExtension]")
                return ParseIniContent(iniContent);

            return new Dictionary<int, string>(); 
        }

        private static Dictionary<int, string> ParseIniContent(string content)
        {
            var result = new Dictionary<int, string>();
            string[] separator = new[] { "\r\n" };
            var lines = content.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length-1; i++)
            {
                var parts = lines[i].Split('=');
                result.Add(int.Parse(parts[0]), parts[1]);
            }
            return result;
        }
        private static bool ReadIniFile(string serverIp, int tcpPort, out string content)
        {
            string cmd = "ini_read\r\n";
            return SendCommand(serverIp, tcpPort, cmd, out content);
        }

        private static bool SendWriteIniCommand(string serverIp, int tcpPort, string content, out string answer)
        {
            string cmd = "ini_write\r\n";
            try
            {
                //---create a TCPClient object at the IP and port no.---
                TcpClient client = new TcpClient(serverIp, tcpPort);
                NetworkStream nwStream = client.GetStream();

                //---send the command---
                byte[] bytesToSend = Encoding.ASCII.GetBytes(cmd);
                Console.WriteLine("Sending : \n" + cmd);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                //---read back the answer---
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                Console.WriteLine("Received : \n" + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));

                //---send the content---
                byte[] contentBytes = new byte[480];
                var contentB = Encoding.ASCII.GetBytes(content);
                Array.Copy(contentB, contentBytes, contentB.Length);
                byte[] bytes256 = new byte[256];
                Array.Copy(contentBytes, bytes256, 256);

                Console.WriteLine("Sending : \n" + bytes256);
                nwStream.Write(bytes256, 0, bytes256.Length);

                byte[] bytes224 = new byte[224];
                Array.Copy(contentBytes, 256, bytes224, 0, 224);

                Console.WriteLine("Sending : \n" + bytes224);
                nwStream.Write(bytes224, 0, bytes224.Length);

                Thread.Sleep(1000);

                //---read back the answer---
                bytesToRead = new byte[client.ReceiveBufferSize];
                bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                Console.WriteLine("Received : \n" + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));

                client.Close();
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
