using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    class AdderInputReceiver : InputReceiverBase
    {
        public override void receiveInputPath()
        {
            bool isInputFileValid = false;

            Console.Write("Please specify the input file(Righ click on the top to do paste): \n");

            while (!isInputFileValid)
            {
                inputPath = Console.ReadLine();

                if (File.Exists(inputPath))
                {
                    isInputFileValid = true;
                }
                else
                {
                    Console.Write("The File is not found. Please re-enter it: \n");
                }
            }
        }

        public override void receiveOutputPath()
        {
            bool isOutputFolderValid = false;
            while (!isOutputFolderValid)
            {
                Console.Write("Please specify the output foler: \n");

                outputPath = Console.ReadLine();

                if (outputPath == "")
                {
                    continue;
                }

                isOutputFolderValid = true;

                string outputFolderPath = Path.GetDirectoryName(outputPath);

                if (!Directory.Exists(outputFolderPath))
                {
                    isOutputFolderValid = false;
                }
            }
        }
    }
}