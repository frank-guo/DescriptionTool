using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    class InputReceiverBase: IInputReceiver
    {
        protected string inputPath;
        protected string outputPath;

        public InputReceiverBase()
        {
            inputPath = null;
            outputPath = null;
        }

        public string InputPath
        {
            get {return inputPath;}
            set {inputPath = value; }
        }

        public string OutputPath
        {
            get { return outputPath; }
            set { inputPath = value; }
        }

        public void readInputOutput()
        {
            receiveInputPath();
            receiveOutputPath();
        }

        public virtual void receiveInputPath()
        {

        }

        public virtual void receiveOutputPath()
        {

        }
    }
}
