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
    /// Class AdderInputReceiver
    /// </summary>
    class AdderInputReceiver : InputReceiverBase
    {
        /// <summary>
        /// Receive the input path from keyboard
        /// </summary>
        public override void receiveInputPath()
        {
            bool isInputFileValid = false;

            Console.Write("Input file(Righ click on the top and choose Edit to do paste): \n");

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

        /// <summary>
        /// Receive the output path from keyboard
        /// </summary>
        public override void receiveOutputPath()
        {
            bool isOutputFolderValid = false;
            while (!isOutputFolderValid)
            {
                Console.Write("Output foler: (Up arrow gets the input above) \n");

                outputPath = Console.ReadLine();

                if (outputPath == "")
                {
                    continue;
                }

                isOutputFolderValid = true;

                if (!Directory.Exists(outputPath))
                {
                    isOutputFolderValid = false;
                    if (outputPath.EndsWith("\\"))
                    {
                        outputPath = outputPath.Substring(0,outputPath.Length-1);       //trim off the tail of "\"
                    }
                }
            }
        }
    }
}