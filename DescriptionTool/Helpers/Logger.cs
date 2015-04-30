using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    class Logger
    {
        private string _logfile;
        private static string Filename = "log.txt";

        public Logger(string logFolder)
        {
            _logfile = logFolder + "\\" + Filename;

            if (File.Exists(_logfile))
            {
                File.Delete(_logfile);
            }
        }

        public void writeLog(string line)
        {
            using (StreamWriter log = File.AppendText(_logfile))
            {
                log.WriteLine(line);
            }
        }
    }
}
