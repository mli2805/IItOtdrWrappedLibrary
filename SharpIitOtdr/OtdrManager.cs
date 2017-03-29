using System;
using System.IO;
using System.Runtime.InteropServices;

namespace IitOtdrLibrary
{
    public class OtdrManager
    {
        public IitOtdrWrapper IitOtdr { get; set; }
        public bool IsInitializedSuccessfully;

        public string LoadDll()
        {
            var dllPath = "iit_otdr.dll";
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
            IitOtdr.InitDll();

            Console.WriteLine($"Connecting to OTDR {ipAddress}...");
            if (!IitOtdr.InitOtdr(ConnectionTypes.Tcp, ipAddress, 1500))
                return;

            IsInitializedSuccessfully = true;
        }

        private readonly object _lockObj = new object();
        private bool _isMeasurementCanceled;
        private IntPtr _sorData = IntPtr.Zero;
        public bool Measure()
        {
            lock (_lockObj)
            {
                _isMeasurementCanceled = false;
            }

            if (!IitOtdr.PrepareMeasurement(true))
                return false;

            try
            {
                bool hasMoreSteps;
                do
                {
                    lock (_lockObj)
                    {
                        if (_isMeasurementCanceled)
                        {
                            IitOtdr.StopMeasurement(true);
                            break;
                        }
                    }

                    hasMoreSteps = IitOtdr.DoMeasurementStep(ref _sorData);
                }
                while (hasMoreSteps);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            GetLastSorData();
            return true;
        }

        public void InterruptMeasurement()
        {
            lock (_lockObj)
            {
                _isMeasurementCanceled = true;
            }
        }

        public void GetLastSorData()
        {
            int bufferLength = IitOtdr.GetSorDataSize(_sorData);
            if (bufferLength == -1)
            {
                Console.WriteLine("_sorData is null");
                return;
            }
            byte[] buffer = new byte[bufferLength];

            var size = IitOtdr.GetSordata(_sorData, buffer, bufferLength);
            if (size == -1)
            {
                Console.WriteLine("Error in GetLastSorData");
                return;
            }
            File.WriteAllBytes(@"c:\temp\123.sor", buffer);
        }
    }
}