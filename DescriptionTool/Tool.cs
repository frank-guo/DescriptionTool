/* Copyright (c) 1994-2015 Sage Software, Inc.  All rights reserved. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    /// <summary>
    /// Class Tool
    /// </summary>
    class Tool
    {
        private ToolType toolType;
        private InputReceiverBase inputReceiver;
        private IDataProcessor dataProcessor;

        /// <summary>
        /// Property ToolType
        /// </summary>
        public ToolType ToolType
        {
            get
            {
                return toolType;
            }

            set
            {
                toolType = value;
            }
        }

        /// <summary>
        /// Tool Constructor
        /// </summary>
        /// <param name="toolType">ToolType</param>
        public Tool(ToolType toolType)
        {
            this.toolType = toolType;
            switch (toolType)
            {
                case ToolType.Extractor:
                    inputReceiver = new ExtractorInputReceiver();
                    inputReceiver.readInputOutput();
                    dataProcessor = new DescpExtractor(inputReceiver);
                    break;
                case ToolType.Adder:
                    inputReceiver = new AdderInputReceiver();
                    inputReceiver.readInputOutput();
                    dataProcessor = new DescpAdder(inputReceiver);
                    break;
            }
        }

        /// <summary>
        /// Run the data processor to process the input and generate the output
        /// </summary>
        public void run()
        {
            Console.WriteLine("Totally {0} .htm files to be processed.", dataProcessor.TotalNumOfFiles.ToString());
            Console.WriteLine("Type in any key to continue..");
            Console.ReadKey();

            dataProcessor.process();

            Console.WriteLine("Type in any key to exit..");
            Console.ReadKey();
                 
        }
    }
}
