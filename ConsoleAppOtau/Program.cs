using System;

namespace ConsoleAppOtau
{
    class Program
    {
        static void Main()
        {
            const string serverIp = "192.168.88.101";
//            const string serverIp = "192.168.96.52";
//            const string serverIp = "192.168.96.57";
//            const string serverIp = "172.16.4.10";
//            const int tcpPort = 11834;

            const int tcpPort = 23;

            var ch = new Charon(new NetAddress() { IpAddress = serverIp, TcpPort = tcpPort });
            if (ch.Initialize())
                Console.WriteLine($"charon {ch.Serial} has {ch.OwnPortCount} ports");

            //reinit
//            if (ch.Initialize())
//                Console.WriteLine($"charon {ch.Serial} has {ch.OwnPortCount} ports");

            var activePort = ch.GetExtendedActivePort();
            if (activePort != -1)
                Console.WriteLine($"{ch.NetAddress.IpAddress}:{ch.NetAddress.TcpPort} active port {activePort}");
            else
                Console.WriteLine("some error");

            var newActivePort = ch.SetExtendedActivePort(14);
            if (newActivePort == -1)
            {
                Console.WriteLine(ch.LastErrorMessage);
                newActivePort = ch.GetExtendedActivePort();
            }
            Console.WriteLine($"New active port {newActivePort}");


            if (ch.DetachOtauFromPort(2))
                Console.WriteLine($"detached successfully");
            else Console.WriteLine($"{ch.LastErrorMessage}");

            if (ch.AttachOtauToPort(new NetAddress("192.168.96.57", 11834) , 1))
                Console.WriteLine($"attached successfully");
            else Console.WriteLine($"{ch.LastErrorMessage}");

            Console.ReadLine();
        }
    }
}
