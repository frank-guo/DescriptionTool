/* Copyright (c) 1994-2015 Sage Software, Inc.  All rights reserved. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    /// <summary>
    /// Interface IInputReceiver
    /// </summary>
    public interface IInputReceiver
    {
        string InputPath {get;set;}
        string OutputPath {get;set;}
        void receiveInputPath();
        void receiveOutputPath();
        void readInputOutput();
    }
}
