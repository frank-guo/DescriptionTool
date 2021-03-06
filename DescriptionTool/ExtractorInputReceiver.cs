﻿/* Copyright (c) 1994-2015 Sage Software, Inc.  All rights reserved. */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    /// <summary>
    /// Class ExtractorInputReceiver
    /// </summary>
    class ExtractorInputReceiver : InputReceiverBase
    {
        /// <summary>
        /// Receive the input path from keyboard
        /// </summary>
        public override void receiveInputPath()
        {
            bool isInputFolderValid = false;

            Console.Write("Input folder(Righ click on the top and choose Edit to do paste): \n");

            while (!isInputFolderValid)
            {
                inputPath = Console.ReadLine();

                if (Directory.Exists(inputPath))
                {
                    isInputFolderValid = true;
                    if (inputPath.EndsWith("\\"))
                    {
                        inputPath = inputPath.Substring(0, outputPath.Length - 1);       //trim off the tail of "\"
                    }
                }
                else
                {
                    Console.Write("The folder is not found. Please re-enter it: \n");
                }
            }
        }

        /// <summary>
        /// Receive the output path from keyboard
        /// </summary>
        public override void receiveOutputPath()
        {
            bool isOutputCreated = false;
            while (!isOutputCreated)
            {
                Console.Write("Output file: (Up arrow gets the input above) \n");

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
