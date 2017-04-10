using System;
using Optixsoft.SorExaminer.OtdrDataFormat;

namespace IitOtdrLibrary
{
    public partial class OtdrManager
    {
        private MoniLevelType GetMoniLevelType(RftsLevelType level)
        {
            switch (level)
            {
                case RftsLevelType.Minor:
                    return MoniLevelType.Minor;
                case RftsLevelType.Major:
                    return MoniLevelType.Major;
                case RftsLevelType.Critical:
                    return MoniLevelType.Critical;
                default:
                    return MoniLevelType.User;
            }
        }

        private void SetMoniResultFlags(MoniResult moniResult, ComparisonReturns returnCode)
        {
            switch (returnCode)
            {
                case ComparisonReturns.Ok:
                    break;
                case ComparisonReturns.ReturnFiberbreak:
                    moniResult.IsFiberBreak = true;
                    break;
                case ComparisonReturns.ReturnNolink:
                    moniResult.IsNoFiber = true;
                    break;
                default:
                    Console.WriteLine($"something goes wrong, code {returnCode}");
                    break;
            }
        }

        public void CompareMeasureWithBase(byte[] baseBuffer, byte[] measBuffer, bool includeBase)
        {
            MoniResult moniResult = new MoniResult();

            var baseSorData = SorData.FromBytes(baseBuffer);
            for (int i = 0; i < baseSorData.RftsParameters.LevelsCount; i++)
            {
                var rftsLevel = baseSorData.RftsParameters.Levels[i];
                if (rftsLevel.IsEnabled)
                {
                    baseSorData.RftsParameters.ActiveLevelIndex = i;
                    CompareOneLevel(baseBuffer, measBuffer, includeBase, GetMoniLevelType(rftsLevel.LevelName), moniResult);
                }
            }

        }

        private void CompareOneLevel(byte[] baseBuffer, byte[] measBuffer, bool includeBase, MoniLevelType type, MoniResult moniResult)
        {
            var moniLevel = new MoniLevel {Type = type};

            // allocate memory
            var baseIntPtr = IitOtdr.SetSorData(baseBuffer);
            var measIntPtr = IitOtdr.SetSorData(measBuffer);

            var returnCode = IitOtdr.CompareActiveLevel(includeBase, measIntPtr);

            var size = IitOtdr.GetSorDataSize(measIntPtr);
            IitOtdr.GetSordata(measIntPtr, measBuffer, size);
            var measSorData = SorData.FromBytes(measBuffer);

            moniLevel.IsLevelFailed = (measSorData.RftsEvents.Results & MonitoringResults.IsFailed) != 0;
            moniResult.Levels.Add(moniLevel);

            SetMoniResultFlags(moniResult, returnCode);

            // free memory
            IitOtdr.FreeSorDataMemory(measIntPtr);
            IitOtdr.FreeSorDataMemory(baseIntPtr);
        }
    }
}
