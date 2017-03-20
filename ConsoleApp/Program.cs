using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using IitOtdrLibrary;

namespace ConsoleApp
{
    class Program
    {
        private static OtdrLogic _otdrLogic;

        static void Main()
        {
            //_otdrLogic = new OtdrLogic("172.16.4.10");
            //_otdrLogic = new OtdrLogic("192.168.88.101");
            _otdrLogic = new OtdrLogic("192.168.96.52");
            if (_otdrLogic.IsInitializedSuccessfully)
            {
                var paramGetter = new OtdrParamsGetter(_otdrLogic.IitOtdr);
                var paramSetForOtdr = paramGetter.GetParamCollectionForOtdr();
                File.WriteAllLines(@"c:\temp\paramOtdr.txt", OtdrParamSetToFileContent(paramSetForOtdr).ToArray());
            }

            Console.Write("Done.");
            Console.Read();
        }

        private static List<string> OtdrParamSetToFileContent(ParamCollectionForOtdr paramCollectionForOtdr)
        {
            var content = new List<string>();
            foreach (var unit in paramCollectionForOtdr.Units)
            {
                content.Add(unit.Key);
                content.AddRange(WaveLengthParamSetToFileContent(unit.Value));
            }
            return content;
        }

        private static List<string> WaveLengthParamSetToFileContent(ParamCollectionForWaveLength paramCollectionForWaveLength)
        {
            var content = new List<string>();
            foreach (var distance in paramCollectionForWaveLength.Distances)
            {
                content.Add("");
                content.Add($"LMax = {distance.Key}");
                content.AddRange(DistanceParamSetToFileContent(distance.Value));
            }
            return content;
        }

        private static List<string> DistanceParamSetToFileContent(ParamCollectionForDistance paramCollectionForDistance)
        {
            var content = new List<string>();
            paramCollectionForDistance.Resolutions.ToList().ForEach(r => content.Add($"Resolution = {r}"));
            paramCollectionForDistance.PulseDurations.ToList().ForEach(r => content.Add($"Pulse duration = {r}"));
            paramCollectionForDistance.AveragingTime.ToList().ForEach(r => content.Add($"Averaging time = {r}"));
            paramCollectionForDistance.AveragingNumber.ToList().ForEach(r => content.Add($"Averageing number = {r}"));
            content.Add($"BC = {paramCollectionForDistance.Bc.ToString(CultureInfo.CurrentCulture)}");
            content.Add($"OB = {paramCollectionForDistance.Ob.ToString(CultureInfo.CurrentCulture)}");
            return content;
        }

    }
}
