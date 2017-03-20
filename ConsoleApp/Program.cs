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
        private static OtdrManager _otdrManager;

        static void Main()
        {
            _otdrManager = new OtdrManager();
            if (_otdrManager.LoadDll())
            {
                //_otdrManager.InitializeLibrary("172.16.4.10");
                //_otdrManager.InitializeLibrary("192.168.88.101");
                _otdrManager.InitializeLibrary("192.168.96.52");
                if (_otdrManager.IsInitializedSuccessfully)
                {
                    var paramGetter = new OtdrParamsGetter(_otdrManager.IitOtdr);
                    var paramSetForOtdr = paramGetter.GetParamCollectionForOtdr();
                    File.WriteAllLines(@"c:\temp\paramOtdr.txt", OtdrParamSetToFileContent(paramSetForOtdr).ToArray());
                }
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
