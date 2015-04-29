using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    class ExtractInputProcessor : InputProcessorBase
    {
        public override void receiveInputPath()
        {
            bool isInputFolderValid = false;

            Console.Write("Please specify the input folder(Righ click on the top to do paste): \n");

            while (!isInputFolderValid)
            {
                inputPath = Console.ReadLine();

                if (Directory.Exists(inputPath))
                {
                    isInputFolderValid = true;
                }
                else
                {
                    Console.Write("The folder is not found. Please re-enter it: \n");
                }
            }
        }

        public override void receiveOutputPath()
        {
            bool isOutputCreated = false;
            while (!isOutputCreated)
            {
                Console.Write("Please specify the output file: \n");

                outputPath = Console.ReadLine();

                if (outputPath == "")
                {
                    continue;
                }

                isOutputCreated = true;

                try
                {
                    string outputFolderPath = Path.GetDirectoryName(outputPath);
                    DirectoryInfo outputFolder = Directory.CreateDirectory(outputFolderPath);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message + "\n");
                    isOutputCreated = false;
                }
            }
        }
    }
}
