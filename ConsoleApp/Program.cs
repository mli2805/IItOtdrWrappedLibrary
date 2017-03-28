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
//                    otdrAddress = "172.16.4.10";
//                    otdrAddress = "192.168.88.101";
                    otdrAddress = "192.168.96.52";
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
            content.Add($"Unit = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.Unit)}");
            content.Add($"ActiveUnit = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.ActiveUnit)}");

            content.Add($"ActiveRi = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.ActiveRi)}");

            content.Add($"ActiveBc = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.ActiveBc)}");

            content.Add($"Lmax = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.Lmax)}");
            content.Add($"ActiveLmax = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.ActiveLmax)}");

            content.Add($"Res = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.Res)}");
            content.Add($"ActiveRes = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.ActiveRes)}");

            content.Add($"Pulse = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.Pulse)}");
            content.Add($"ActivePulse = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.ActivePulse)}");

            var isTime = _otdrManager.IitOtdr.GetLineOfVariantsForParam((int) ServiceCmdParam.ActiveIsTime);
            content.Add($"ActiveIsTime = {isTime}");
            if (1 == int.Parse(isTime))
            {
                content.Add($"Time = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int)ServiceCmdParam.Time)}");
                content.Add($"ActiveTime = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int)ServiceCmdParam.ActiveTime)}");
            }
            else
            {
                content.Add($"Navr = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int)ServiceCmdParam.Navr)}");
                content.Add($"ActiveNavr = {_otdrManager.IitOtdr.GetLineOfVariantsForParam((int)ServiceCmdParam.ActiveNavr)}");
            }

            // and so on...
            return content;
        }
    }
}