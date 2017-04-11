using System;
using System.IO;
using System.Runtime.InteropServices;
using Iit.Fibertest.Utils;

namespace IitOtdrLibrary
{
    public partial class OtdrManager
    {
        private readonly string _iitotdrFolder;
        private Logger _logger;

        public IitOtdrWrapper IitOtdr { get; set; }
        public bool IsInitializedSuccessfully;

        public OtdrManager(string iitotdrFolder, Logger logger)
        {
            _iitotdrFolder = iitotdrFolder;
            _logger = logger;
            _logger.EmptyLine();
            _logger.EmptyLine('-');
            _logger.AppendLine("OtdrManager initialized");
        }
        
        public string LoadDll()
        {
            var dllPath = Path.Combine(_iitotdrFolder, @"iit_otdr.dll");
            var handle = Native.LoadLibrary(dllPath);
            string message;
            if (handle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                message = $"Failed to load library {dllPath} (code: {errorCode})";
                Console.WriteLine(message);
                _logger.AppendLine(message);
                return message;
            }

            message = $"Library {dllPath} loaded successfully";
            Console.WriteLine(message);
            _logger.AppendLine(message);
            return "";
        }
        public void InitializeLibrary(string ipAddress)
        {
            IitOtdr = new IitOtdrWrapper();

            var message = "Initializing iit_otdr (loading sub libraries?) ...";
            Console.WriteLine(message);
            _logger.AppendLine(message);
            IitOtdr.InitDll(_iitotdrFolder);

            Console.WriteLine($"Connecting to OTDR {ipAddress}...");
            if (!IitOtdr.InitOtdr(ConnectionTypes.Tcp, ipAddress, 1500))
                return;

            IsInitializedSuccessfully = true;
        }

    }
}