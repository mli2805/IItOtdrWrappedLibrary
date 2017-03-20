using System;
using System.Runtime.InteropServices;

namespace IitOtdrLibrary
{
    public class OtdrLogic
    {
        public IitOtdrWrap IitOtdr { get; set; }
        public readonly bool IsInitializedSuccessfully;

        public OtdrLogic(string ipAddress)
        {
            IitOtdr = new IitOtdrWrap();

            if (!LoadDll("iit_otdr.dll"))
                return;

            IitOtdr.InitDll();
            if (!IitOtdr.InitOtdr(ConnectionTypes.Tcp, ipAddress, 1500))
                return;

            IsInitializedSuccessfully = true;
        }

        private bool LoadDll(string dllPath)
        {
            var handle = Native.LoadLibrary(dllPath);
            if (handle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                Console.WriteLine($"Failed to load library {dllPath} (code: {errorCode})");
                return false;
            }
            return true;
        }
    }
}