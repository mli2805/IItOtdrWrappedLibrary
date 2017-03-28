using System;
using System.Runtime.InteropServices;

namespace IitOtdrLibrary
{
    public partial class IitOtdrWrapper
    {
        // EXTERN_C __declspec(dllexport) int MeasPrepare(int mMode);
        [DllImport("iit_otdr.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MeasPrepare")]
        public static extern int MeasPrepare(int measurementMode);

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
        public static extern int GetSorData(IntPtr sorData, byte[] buffer, int bufferLength);


        public bool PrepareMeasurement(bool isAver)
        {
            var error = MeasPrepare(isAver ? 601 : 600);
            if (error != 0)
                Console.WriteLine($"Error {error} in MeasPrepare");
            return error == 0;
        }

        public bool DoMeasurementStep(ref IntPtr sorData)
        {
            var error = MeasStep(ref sorData);
            if (error != 0)
                Console.WriteLine($"Error {error} in MeasStep");
            return error == 0;
        }

        public int StopMeasurement(bool isImmediateStop)
        {
            IntPtr sorData = IntPtr.Zero;
            return MeasStop(ref sorData, isImmediateStop ? 1 : 0);
        }

        public int GetSorDataSize(IntPtr sorData)
        {
            return GetSorSize(sorData);
        }

        public int GetSordata(IntPtr sorData, byte[] buffer, int bufferLength)
        {
            return GetSorData(sorData, buffer, bufferLength);
        }
    }
}
