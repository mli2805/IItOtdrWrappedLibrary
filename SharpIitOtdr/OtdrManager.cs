using System;
using System.Runtime.InteropServices;
using Optixsoft.SorExaminer.OtdrDataFormat;

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

        public bool MeasureWithBase(byte[] buffer)
        {
            // allocate memory inside c++ library
            // put there base sor data
            // return pointer to that data, than you can say c++ code to use this data
            var baseSorData = IitOtdr.SetBaseSorData(buffer);

            var result = false;
            if (IitOtdr.SetMeasurementParametersFromSor(ref baseSorData))
            {
                IitOtdr.ForceLmaxNs(IitOtdr.ConvertLmaxOwtToNs(buffer));
                result = Measure();
            }

            // free memory where was base sor data
            IitOtdr.FreeBaseSorDataMemory(baseSorData);
            return result;
        }

        public bool DoManualMeasurement(bool shouldForceLmax)
        {
            if (shouldForceLmax)
                IitOtdr.ForceLmaxNs(IitOtdr.ConvertLmaxKmToNs());

            return Measure();
        }

        private readonly object _lockObj = new object();
        private bool _isMeasurementCanceled;
        private IntPtr _sorData = IntPtr.Zero;

        private bool Measure()
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

            return true;
        }

        public void InterruptMeasurement()
        {
            lock (_lockObj)
            {
                _isMeasurementCanceled = true;
            }
        }

        public OtdrDataKnownBlocks GetLastSorData()
        {
            int bufferLength = IitOtdr.GetSorDataSize(_sorData);
            if (bufferLength == -1)
            {
                Console.WriteLine("_sorData is null");
                return null;
            }
            byte[] buffer = new byte[bufferLength];

            var size = IitOtdr.GetSordata(_sorData, buffer, bufferLength);
            if (size == -1)
            {
                Console.WriteLine("Error in GetLastSorData");
                return null;
            }

            var sorData = SorData.FromBytes(buffer);
//            sorData.IitParameters.SetFlagFilter(false); ???

            return sorData;
        }
    }
}