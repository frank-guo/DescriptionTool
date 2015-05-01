/* Copyright (c) 1994-2015 Sage Software, Inc.  All rights reserved. */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DescriptionTool
{
    /// <summary>
    /// Class FileWriter
    /// </summary>
    class FileWriter
    {
        private string _outputFile;

        /// <summary>
        /// FileWriter Constructor
        /// </summary>
        /// <param name="outputFile">Output File Full Path</param>
        public FileWriter(string outputFile)
        {
            _outputFile = outputFile;

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
        }

        /// <summary>
        /// Write content to the ouputFile
        /// </summary>
        /// <param name="contentToWrite">Content To write</param>
        public void write(string contentToWrite)
        {
        save:
            try
            {
                File.AppendAllText(_outputFile, contentToWrite);
            }
            catch (Exception e)
            {
                DialogResult result = DialogResult.Abort;
                try
                {
                    result = MessageBox.Show("Please contact the developers with the"
                      + " following information:\n\n" + e.Message + e.StackTrace,
                      "Application Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                    if (result == DialogResult.Retry)
                    {
                        goto save;
                    }
                }
                finally
                {
                    if (result == DialogResult.Abort)
                    {
                        Application.Exit();
                    }
                }
            }
        }
    }
}
