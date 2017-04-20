using System;
using DirectCharonLibrary;
using Iit.Fibertest.Utils35;

namespace ConsoleAppOtau
{
    class Program
    {
        private static Logger _rtuLogger;
        static void Main()
        {
            _rtuLogger = new Logger("rtu.log");
//          const string serverIp = "192.168.88.101";
            const string serverIp = "192.168.96.52";
//          const string serverIp = "192.168.96.57";
//          const string serverIp = "172.16.4.10";
//          const int tcpPort = 11834;

            const int tcpPort = 23;
            _rtuLogger.AppendLine("Otau initialization started");
            var ch = new Charon(new NetAddress() { IpAddress = serverIp, TcpPort = tcpPort }, _rtuLogger);
            if (ch.Initialize())
                _rtuLogger.AppendLine($"charon {ch.Serial} has {ch.OwnPortCount} ports");

            //reinit
//            if (ch.Initialize())
//                _rtuLogger.AppendLine($"charon {ch.Serial} has {ch.OwnPortCount} ports");

            var activePort = ch.GetExtendedActivePort();
            if (activePort != -1)
                _rtuLogger.AppendLine($"{ch.NetAddress.IpAddress}:{ch.NetAddress.TcpPort} active port {activePort}");
            else
                _rtuLogger.AppendLine("some error");

            var newActivePort = ch.SetExtendedActivePort(14);
            if (newActivePort == -1)
            {
                _rtuLogger.AppendLine(ch.LastErrorMessage);
                newActivePort = ch.GetExtendedActivePort();
            }
            _rtuLogger.AppendLine($"New active port {newActivePort}");


            if (ch.DetachOtauFromPort(2))
                _rtuLogger.AppendLine($"detached successfully");
            else _rtuLogger.AppendLine($"{ch.LastErrorMessage}");

            if (ch.AttachOtauToPort(new NetAddress("192.168.96.57", 11834) , 2))
                _rtuLogger.AppendLine($"attached successfully");
            else _rtuLogger.AppendLine($"{ch.LastErrorMessage}");

            Console.ReadLine();
        }
    }
}
