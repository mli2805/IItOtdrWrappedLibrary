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

        static void Main(string[] args)
        {
            _otdrManager = new OtdrManager();
            if (_otdrManager.LoadDll())
            {
                string otdrAddress;
                if (args.Length == 1)
                    otdrAddress = args[0];
                else
                {
                    otdrAddress = "172.16.4.10";
//                    otdrAddress = "192.168.88.101";
                    //otdrAddress = "192.168.96.52";
                }
                _otdrManager.InitializeLibrary(otdrAddress);
                if (_otdrManager.IsInitializedSuccessfully)
                {
                    var paramGetter = new OtdrParamsGetter(_otdrManager.IitOtdr);
                    var paramCollectionForOtdr = paramGetter.GetParamCollectionForOtdr();
                    File.WriteAllLines(@"c:\temp\paramOtdr.txt", OtdrParamCollectionToFileContent(paramCollectionForOtdr).ToArray());
                }
            }
            Console.Write("Done.");
            Console.Read();
        }

        private static List<string> OtdrParamCollectionToFileContent(ParamCollectionForOtdr paramCollectionForOtdr)
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
                content.AddRange(DistanceParamCollectionToFileContent(distance.Value));
            }
            return content;
        }

        private static List<string> DistanceParamCollectionToFileContent(ParamCollectionForDistance paramCollectionForDistance)
        {
            var content = new List<string>();
            foreach (var resolution in paramCollectionForDistance.Resolutions)
            {
                content.Add("");
                content.Add($"Resolution = {resolution.Key}");
                content.AddRange(ResolutionParamCollectionToFileContent(resolution.Value));
            }
            return content;
        }

        private static List<string> ResolutionParamCollectionToFileContent(ParamCollectionForResolution paramCollectionForResolution)
        {
            var content = new List<string>();
            paramCollectionForResolution.PulseDurations.ToList().ForEach(r => content.Add($"Pulse duration = {r}"));
            foreach (var pair in paramCollectionForResolution.TimeToAverage)
            {
                content.Add($"Time to average = {pair.Key}  /  measurements count to average = {pair.Value}");
            }
            foreach (var pair in paramCollectionForResolution.MeasurementCountToAverage)
            {
                content.Add($"Measurements count to average = {pair.Key}  /  Time to average = {pair.Value}");

            }
            content.Add($"BC = {paramCollectionForResolution.Bc.ToString(CultureInfo.CurrentCulture)}");
            content.Add($"OB = {paramCollectionForResolution.Ob.ToString(CultureInfo.CurrentCulture)}");
            return content;
        }
    }
}