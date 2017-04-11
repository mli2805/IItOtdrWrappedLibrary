using System;
using Optixsoft.SorExaminer.OtdrDataFormat;

namespace IitOtdrLibrary
{
    public partial class OtdrManager
    {
        public bool MeasureWithBase(byte[] buffer)
        {
            // allocate memory inside c++ library
            // put there base sor data
            // return pointer to that data, than you can say c++ code to use this data
            var baseSorData = IitOtdr.SetSorData(buffer);

            var result = false;
            if (IitOtdr.SetMeasurementParametersFromSor(ref baseSorData))
            {
                IitOtdr.ForceLmaxNs(IitOtdr.ConvertLmaxOwtToNs(buffer));
                result = Measure();
            }

            // free memory where was base sor data
            IitOtdr.FreeSorDataMemory(baseSorData);
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

        /// <summary>
        /// after Measure() use GetLastSorData() to obtain measurement result
        /// </summary>
        /// <returns></returns>
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
                _rtuLogger.AppendLine(e.Message);
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


        public byte[] GetLastSorDataBuffer()
        {
            int bufferLength = IitOtdr.GetSorDataSize(_sorData);
            if (bufferLength == -1)
            {
                _rtuLogger.AppendLine("_sorData is null");
                return null;
            }
            byte[] buffer = new byte[bufferLength];

            var size = IitOtdr.GetSordata(_sorData, buffer, bufferLength);
            if (size == -1)
            {
                _rtuLogger.AppendLine("Error in GetLastSorData");
                return null;
            }
            return buffer;
        }

        public OtdrDataKnownBlocks GetLastSorData()
        {
            var buffer = GetLastSorDataBuffer();
            if (buffer == null)
                return null;

            var sorData = SorData.FromBytes(buffer);
            sorData.IitParameters.Parameters = (IitBlockParameters)SetBitFlagInParameters((int)sorData.IitParameters.Parameters, IitBlockParameters.Filter, false);
            return sorData;
        }

        private int SetBitFlagInParameters(int parameters, IitBlockParameters parameter, bool flag)
        {
            return flag
                ? parameters | (int)parameter
                : parameters & (65535 ^ (int)parameter);
        }
    }
}
