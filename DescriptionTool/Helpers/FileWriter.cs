using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DescriptionTool
{
    class FileWriter
    {
        private string _outputFile;

        public FileWriter(string outputFile)
        {
            _outputFile = outputFile;

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
        }

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
