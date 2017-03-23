﻿using System;
using System.Collections.Generic;

namespace DirectCharonLibrary
{
    public partial class Charon
    {
        private string GetSerial()
        {
            SendCommand("get_rtu_number\r\n");
            return LastAnswer;
        }

        private int GetOwnPortCount()
        {
            SendCommand("otau_get_count_channels\r\n");
            if (!IsLastCommandSuccessful)
                return -1;

            int ownPortCount;
            if (int.TryParse(LastAnswer, out ownPortCount))
                return ownPortCount;

            LastErrorMessage = "Invalid port count";
            IsLastCommandSuccessful = false;
            return -1;
        }

        private Dictionary<int, NetAddress> GetExtentedPorts()
        {
            ReadIniFile();
            if (!IsLastCommandSuccessful)
                return null; // read iniFile error

            if (LastAnswer.Substring(0, 15) == "ERROR_COMMAND\r\n")
                return new Dictionary<int, NetAddress>(); // charon too old, know nothing about extensions

            if (LastAnswer.Substring(0, 22) == "[OpticalPortExtension]")
                return ParseIniContent(LastAnswer);

            return new Dictionary<int, NetAddress>();
        }

        private Dictionary<int, NetAddress> ParseIniContent(string content)
        {
            var result = new Dictionary<int, NetAddress>();
            string[] separator = new[] { "\r\n" };
            var lines = content.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length - 1; i++)
            {
                var parts = lines[i].Split('=');
                var addressParts = parts[1].Split(':');
                int port;
                int.TryParse(addressParts[1], out port);
                result.Add(int.Parse(parts[0]), new NetAddress(addressParts[0], port));
            }
            return result;
        }
        private void ReadIniFile() { SendCommand("ini_read\r\n"); }

        private int SetActivePort(int port)
        {
            SendCommand($"otau_set_channel {port} d\r\n");
            if (!IsLastCommandSuccessful)
                return -1;
            var resultingPort = GetActivePort();
            if (!IsLastCommandSuccessful)
                return -1;
            if (resultingPort != port)
                LastErrorMessage = "Set active port number error";
            return resultingPort;
        }

        private int GetActivePort()
        {
            SendCommand("otau_get_channel\r\n");
            if (!IsLastCommandSuccessful)
                return -1;

            int activePort;
            if (int.TryParse(LastAnswer, out activePort))
                return activePort;

            IsLastCommandSuccessful = false;
            return -1;
        }
        private string DictionaryToContent(Dictionary<int, NetAddress> extPorts)
        {
            if (extPorts.Count == 0)
                return "\r\n";
            var result = "[OpticalPortExtension]\r\n";
            foreach (var extPort in extPorts)
                result += $"{extPort.Key}={extPort.Value}\r\n";
            return result;
        }
    }
}
