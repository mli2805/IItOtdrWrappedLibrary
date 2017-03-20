using System;
using System.Runtime.InteropServices;

namespace IitOtdrLibrary
{
    public class OtdrManager
    {
        public IitOtdrWrapper IitOtdr { get; set; }
        public bool IsInitializedSuccessfully;

        public bool LoadDll()
        {
            var dllPath = "iit_otdr.dll";
            var handle = Native.LoadLibrary(dllPath);
            if (handle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                Console.WriteLine($"Failed to load library {dllPath} (code: {errorCode})");
                return false;
            }
            return true;
        }
        public void InitializeLibrary(string ipAddress)
        {
            IitOtdr = new IitOtdrWrapper();

            IitOtdr.InitDll();
            if (!IitOtdr.InitOtdr(ConnectionTypes.Tcp, ipAddress, 1500))
                return;

            IsInitializedSuccessfully = true;
        }

    }
}