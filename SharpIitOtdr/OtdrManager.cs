using System;
using System.IO;
using System.Runtime.InteropServices;

namespace IitOtdrLibrary
{
    public partial class OtdrManager
    {
        private readonly string _folder;

        public IitOtdrWrapper IitOtdr { get; set; }
        public bool IsInitializedSuccessfully;

        public OtdrManager(string folder)
        {
            _folder = folder;
        }

        public string LoadDll()
        {
            var dllPath = Path.Combine(_folder, @"iit_otdr.dll");
            var handle = Native.LoadLibrary(dllPath);
            if (handle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                var result = $"Failed to load library {dllPath} (code: {errorCode})";
                Console.WriteLine(result);
                return result;
            }
            Console.WriteLine($"Library {dllPath} loaded successfully");
            return "";
        }
        public void InitializeLibrary(string ipAddress)
        {
            IitOtdr = new IitOtdrWrapper();

            Console.WriteLine($"Initializing iit_otdr (loading sub libraries?) ...");
            IitOtdr.InitDll(_folder);

            Console.WriteLine($"Connecting to OTDR {ipAddress}...");
            if (!IitOtdr.InitOtdr(ConnectionTypes.Tcp, ipAddress, 1500))
                return;

            IsInitializedSuccessfully = true;
        }

    }
}