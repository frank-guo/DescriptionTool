/* Copyright (c) 1994-2015 Sage Software, Inc.  All rights reserved. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    /// <summary>
    /// Interface IDataProcessor
    /// </summary>
    public interface IDataProcessor
    {
        int TotalNumOfFiles { get; }
        void process();
    }
}
