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
        }

        public void writeLog(string line)
        {
            if (File.Exists(_logfile))
            {
                File.Delete(_logfile);
            }


            using (StreamWriter log = File.CreateText(_logfile))
            {
                log.WriteLine(line);
            }
        }
    }
}
