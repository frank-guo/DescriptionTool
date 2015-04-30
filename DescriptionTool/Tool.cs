using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    class Tool
    {
        private ToolType toolType;
        private InputReceiverBase inputReceiver;
        private IDataProcessor dataProcessor;

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
