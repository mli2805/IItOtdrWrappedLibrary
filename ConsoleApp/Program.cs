using System;
using System.Collections.Generic;
using System.IO;
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
//                    otdrAddress = "192.168.96.52";
                }
                _otdrManager.InitializeLibrary(otdrAddress);
                if (_otdrManager.IsInitializedSuccessfully)
                {
                    var content = ReadCurrentParameters();
                    File.WriteAllLines(@"c:\temp\paramOtdr.txt", content.ToArray());
                }
            }
            Console.Write("Done.");
            Console.Read();
        }

        private static List<string> ReadCurrentParameters()
        {
            var content = new List<string>();
            var units = _otdrManager.IitOtdr.ParseLineOfVariantsForParam((int) ServiceCmdParam.Unit);
            content.AddRange(units);
            var distances = _otdrManager.IitOtdr.ParseLineOfVariantsForParam((int) ServiceCmdParam.Lmax);
            content.AddRange(distances);
            // and so on...
            return content;
        }
    }
}