namespace ConsoleAppOtau
{
    public class NetAddress
    {
        public string IpAddress { get; set; }
        public int TcpPort { get; set; }

        public NetAddress()
        {
        }

        public NetAddress(string ipAddress, int tcpPort)
        {
            IpAddress = ipAddress;
            TcpPort = tcpPort;
        }

        public override string ToString()
        {
            return $"{IpAddress}:{TcpPort}";
        }
    }
}