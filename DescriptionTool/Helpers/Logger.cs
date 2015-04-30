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
        private int failCount;

        public int FailCount
        {
            get { return failCount; }
        }

        public Logger(string logFolder)
        {
            _logfile = logFolder + "\\" + Filename;
            failCount = 0;

            if (File.Exists(_logfile))
            {
                File.Delete(_logfile);
            }
        }

        public void writeLog(string line)
        {
            failCount++;
            using (StreamWriter log = File.AppendText(_logfile))
            {
                log.WriteLine(line);
            }
        }
    }
}
