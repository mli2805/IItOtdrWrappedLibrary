using System;
using System.Runtime.InteropServices;

namespace IitOtdrLibrary
{
    public class IitOtdrWrap
    {
        //EXTERN_C __declspec(dllexport) void DllInit(char* pathDLL, void* logFile, TLenUnit* lenUnit);
        [DllImport("iit_otdr.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DllInit")]
        public static extern void DllInit(string path, IntPtr logFile, IntPtr lenUnit);

        // EXTERN_C __declspec(dllexport) int InitOTDR(int Type, const char* Name_IP, long Speed_Tport);
        [DllImport("iit_otdr.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InitOTDR")]
        public static extern int InitOTDR(int type, string ip, int port);

        // EXTERN_C __declspec(dllexport) int ServiceFunction(long cmd, long& prm1, void** prm2);
        [DllImport("iit_otdr.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ServiceFunction")]
        public static extern int ServiceFunction(int cmd, ref int prm1, ref IntPtr prm2);

        public void InitDll()
        {
            string path = "";
            IntPtr logFile = IntPtr.Zero;
            IntPtr lenUnit = IntPtr.Zero;
            DllInit(path, logFile, lenUnit);
        }

        public bool InitOtdr(ConnectionTypes type, string ip, int port)
        {
            var initOtdr = InitOTDR((int)type, ip, port);
            if (initOtdr != 0)
                Console.WriteLine($"Initialization error: {initOtdr}");
            return initOtdr == 0;
        }

        public string GetLineOfVariantsForParam(int param)
        {
            int cmd = 702; // SERVICE_CMD_GETPARAM
            int prm1 = param;

            IntPtr unmanagedPointer = IntPtr.Zero;
            int res = ServiceFunction(cmd, ref prm1, ref unmanagedPointer);
            if (res != 0)
                return null;

            return Marshal.PtrToStringAnsi(unmanagedPointer);
        }

        public void SetParam(int param, int indexInLine)
        {
            int cmd = 705;
            int prm1 = param;
            IntPtr prm2 = new IntPtr(indexInLine);
            if (ServiceFunction(cmd, ref prm1, ref prm2) != 0)
                Console.WriteLine("Set parameter error!");
        }

    }
}