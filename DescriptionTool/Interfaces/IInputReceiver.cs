using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    public interface IInputReceiver
    {
        string InputPath {get;set;}
        string OutputPath {get;set;}
        void receiveInputPath();
        void receiveOutputPath();

        void readInputOutput();
    }
}
