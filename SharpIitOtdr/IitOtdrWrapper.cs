using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace IitOtdrLibrary
{
    public partial class IitOtdrWrapper
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


        public bool InitDll()
        {
            string path = "";
            IntPtr logFile = IntPtr.Zero;
            IntPtr lenUnit = IntPtr.Zero;
            try
            {
                DllInit(path, logFile, lenUnit);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
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
            int cmd = (int)ServiceFunctionCommand.GetParam;
            int prm1 = param;

            IntPtr unmanagedPointer = IntPtr.Zero;
            int res = ServiceFunction(cmd, ref prm1, ref unmanagedPointer);
            if (res != 0)
                return null;

            return Marshal.PtrToStringAnsi(unmanagedPointer);
        }

        public string[] ParseLineOfVariantsForParam(int paramCode)
        {
            string value = GetLineOfVariantsForParam(paramCode);
            if (value == null)
                return null;

            // if there is only one variant it will be returned without leading slash
            if (value[0] != '/')
                return new[] { value };

            var strs = value.Split('/');
            return strs.Skip(1).ToArray();
        }


        public void SetParam(int param, int indexInLine)
        {
            int cmd = (int)ServiceFunctionCommand.SetParam;
            int prm1 = param;
            IntPtr prm2 = new IntPtr(indexInLine);
            if (ServiceFunction(cmd, ref prm1, ref prm2) != 0)
                Console.WriteLine("Set parameter error!");
        }

        public bool SetMeasurementParametersFromSor(ref IntPtr baseSorData)
        {
            int cmd = (int)ServiceFunctionCommand.SetParamFromSor;
            int reserved = 0;

            var result = ServiceFunction(cmd, ref reserved, ref baseSorData);
            if (result != 0)
                Console.WriteLine("Set parameter error!");
            return result == 0;
        }

        public bool ForceLmaxNs(int lmaxNs)
        {
            int cmd = (int)ServiceFunctionCommand.ParamMeasLmaxSet;
            IntPtr reserved = new IntPtr();
            var result = ServiceFunction(cmd, ref lmaxNs, ref reserved);
            if (result != 0)
                Console.WriteLine("Set parameter error!");
            return result == 0;

        }

    }
}