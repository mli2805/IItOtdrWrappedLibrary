using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace IitOtdrLibrary
{
    public class IitOtdrWrapper
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



        // EXTERN_C __declspec(dllexport) int MeasPrepare(int mMode);
        [DllImport("iit_otdr.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MeasPrepare")]
        public static extern int MeasPrepare(int isFast);

        // EXTERN_C __declspec(dllexport) int MeasStep(TSorData** rezSD);
        [DllImport("iit_otdr.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MeasStep")]
        public static extern int MeasStep(ref IntPtr sorData);
        // EXTERN_C __declspec(dllexport) int MeasStop(TSorData** fullSD, int stopMode);
        [DllImport("iit_otdr.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MeasStop")]
        public static extern int MeasStop(ref IntPtr sorData, int isImmediateStop);



        // EXTERN_C __declspec(dllexport) long GetSorSize(TSorData* sorData);
        [DllImport("iit_otdr.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetSorSize")]
        public static extern int GetSorSize(IntPtr sorData);
        // EXTERN_C __declspec(dllexport) long GetSorData(TSorData* sorData, char* buffer, long bufferLength);
        [DllImport("iit_otdr.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetSorData")]
        public static extern int GetSorData(IntPtr sorData, string buffer, int bufferLength);



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
            int cmd = 702; // SERVICE_CMD_GETPARAM
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

            // если вариант только 1 он возвращается без первого слэша
            if (value[0] != '/')
                return new[] { value };

            var strs = value.Split('/');
            return strs.Skip(1).ToArray();
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