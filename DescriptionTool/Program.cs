﻿/* Copyright (c) 1994-2015 Sage Software, Inc.  All rights reserved. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace DescriptionTool
{
    /// <summary>
    /// Class Main
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                ChooseTool:
                Console.WriteLine("Please choose 1/2:\n(1)Extract description       (2)Add description");
                string input =  Console.ReadLine();
                if (input != "1" && input != "2")
                {
                    Console.WriteLine("Wrong Choice, Retry");
                    goto ChooseTool;
                }
                
                ToolType toolType = (ToolType)(Convert.ToInt32(input));
                Tool tool = new Tool(toolType);
                tool.run();
            }
            catch (Exception e)
            {
                Console.Write(e.Message + "\n");
                Console.WriteLine("Type in any key to exit..");
                Console.ReadKey();
            }

        }

    }
}
