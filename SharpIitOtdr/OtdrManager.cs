using System;
using System.IO;
using System.Runtime.InteropServices;
using Iit.Fibertest.Utils;

namespace IitOtdrLibrary
{
    public partial class OtdrManager
    {
        private readonly string _iitotdrFolder;
        private readonly Logger _rtuLogger;

        public IitOtdrWrapper IitOtdr { get; set; }
        public bool IsInitializedSuccessfully;

        public OtdrManager(string iitotdrFolder, Logger rtuLogger)
        {
            _iitotdrFolder = iitotdrFolder;
            _rtuLogger = rtuLogger;
            _rtuLogger.EmptyLine();
            _rtuLogger.EmptyLine('-');
            _rtuLogger.AppendLine("OtdrManager initialized");
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
                _rtuLogger.AppendLine(message);
                return message;
            }

            message = $"Library {dllPath} loaded successfully";
            _rtuLogger.AppendLine(message);
            return "";
        }
        public void InitializeLibrary(string ipAddress)
        {
            IitOtdr = new IitOtdrWrapper(_rtuLogger);

            var message = "Initializing iit_otdr (loading sub libraries?) ...";
            _rtuLogger.AppendLine(message);
            IitOtdr.InitDll(_iitotdrFolder);

            _rtuLogger.AppendLine($"Connecting to OTDR {ipAddress}...");
            if (!IitOtdr.InitOtdr(ConnectionTypes.Tcp, ipAddress, 1500))
                return;

            IsInitializedSuccessfully = true;
            _rtuLogger.AppendLine("OTDR initialized successfully!");
        }

    }
}