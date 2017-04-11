using System;
using System.Linq;
using Optixsoft.SorExaminer.OtdrDataFormat;
using Optixsoft.SorExaminer.OtdrDataFormat.Structures;

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

        private EmbeddedDataBlock bufferToEmbeddedDataBlock(byte[] buffer)
        {
            var embededData = new EmbeddedData
            {
                Description = "SOR",
                DataSize = buffer.Length,
                Data = buffer.ToArray()
            };

            var result = new EmbeddedDataBlock
            {
                EmbeddedDataBlocks = new[] {embededData}
            };
            return result;
        }

        public MoniResult CompareMeasureWithBase(byte[] baseBuffer, byte[] measBuffer, bool includeBase)
        {
            MoniResult moniResult = new MoniResult();

            var baseSorData = SorData.FromBytes(baseBuffer);
            var measSorData = SorData.FromBytes(measBuffer);
            if (includeBase)
                measSorData.EmbeddedData = bufferToEmbeddedDataBlock(baseBuffer);

            var levelCount = baseSorData.RftsParameters.LevelsCount;
            Console.WriteLine($"Comparison begin. Level count = {levelCount}");
            for (int i = 0; i < levelCount; i++)
            {
                var rftsLevel = baseSorData.RftsParameters.Levels[i];
                if (rftsLevel.IsEnabled)
                {
                    baseSorData.RftsParameters.ActiveLevelIndex = i;
                    CompareOneLevel(baseSorData, ref measSorData, false, GetMoniLevelType(rftsLevel.LevelName), moniResult);
                }
            }

            measSorData.Save(@"c:\temp\MeasWithBase.sor");
            return moniResult;
        }

        private void CompareOneLevel(OtdrDataKnownBlocks baseSorData, ref OtdrDataKnownBlocks measSorData,  bool includeBase, MoniLevelType type, MoniResult moniResult)
        {
            var moniLevel = new MoniLevel {Type = type};

            // allocate memory
            var baseIntPtr = IitOtdr.SetSorData(SorData.ToBytes(baseSorData));
            IitOtdr.SetBaseForComparison(baseIntPtr);

            var measIntPtr = IitOtdr.SetSorData(SorData.ToBytes(measSorData));
            var returnCode = IitOtdr.CompareActiveLevel(includeBase, measIntPtr);

            Console.WriteLine($"Level {type} comparison result = {returnCode}!");

            var size = IitOtdr.GetSorDataSize(measIntPtr);
            byte[] buffer = new byte[size];
            IitOtdr.GetSordata(measIntPtr, buffer, size);
            measSorData = SorData.FromBytes(buffer);

            moniLevel.IsLevelFailed = (measSorData.RftsEvents.Results & MonitoringResults.IsFailed) != 0;
            moniResult.Levels.Add(moniLevel);

            SetMoniResultFlags(moniResult, returnCode);

            // free memory
            IitOtdr.FreeSorDataMemory(measIntPtr);
            IitOtdr.FreeSorDataMemory(baseIntPtr);
        }
    }
}
