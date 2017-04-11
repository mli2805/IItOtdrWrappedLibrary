using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Iit.Fibertest.Utils
{
    public class Logger
    {
        private readonly StreamWriter _logFile;

        public Logger(string filename)
        {
            var logFullFileName = LogFullFileName(filename);
            _logFile = File.AppendText(logFullFileName);
            _logFile.AutoFlush = true;
        }

        private string LogFullFileName(string filename)
        {
            try
            {
                string logFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\log\"));
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                var logFileName = Path.GetFullPath(Path.Combine(logFolder, filename));
                if (!File.Exists(logFileName))
                    using (FileStream fs = File.Create(logFileName)) { } 
                return logFileName;
            }
            catch (COMException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void EmptyLine(char ch = ' ')
        {
            string message = new string(ch, 78);
            _logFile.WriteLine(message);
        }
        public void AppendLine(string message)
        {
            _logFile.WriteLine(DateTime.Now + "  " + message);
        }
    }
}
