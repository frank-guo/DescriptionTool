/* Copyright (c) 1994-2015 Sage Software, Inc.  All rights reserved. */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    /// <summary>
    /// Class Logger
    /// </summary>
    class Logger
    {
        private string _logfile;
        private static string Filename = "log.txt";
        private int failCount;

        /// <summary>
        /// Property FailCount
        /// </summary>
        public int FailCount
        {
            get { return failCount; }
        }

        /// <summary>
        /// Logger Constructor
        /// </summary>
        /// <param name="logFolder">Log Folder</param>
        public Logger(string logFolder)
        {
            _logfile = logFolder + "\\" + Filename;
            failCount = 0;

            if (File.Exists(_logfile))
            {
                File.Delete(_logfile);
            }
        }

        /// <summary>
        /// Write one line of log info
        /// </summary>
        /// <param name="line">One line of log info</param>
        public void writeLine(string line)
        {
            failCount++;
            using (StreamWriter log = File.AppendText(_logfile))
            {
                log.WriteLine(line);
            }
        }
    }
}
