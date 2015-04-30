using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    public interface IDataProcessor
    {
        int TotalNumOfFiles { get; }
        void process();
    }
}
