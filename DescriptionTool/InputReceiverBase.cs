/* Copyright (c) 1994-2015 Sage Software, Inc.  All rights reserved. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionTool
{
    /// <summary>
    /// Abstract Class InputReceiverBase
    /// </summary>
    class InputReceiverBase: IInputReceiver
    {
        protected string inputPath;
        protected string outputPath;

        /// <summary>
        /// InputReceiverBase Constructor
        /// </summary>
        public InputReceiverBase()
        {
            inputPath = null;
            outputPath = null;
        }

        /// <summary>
        /// Property InputPath
        /// </summary>
        public string InputPath
        {
            get {return inputPath;}
            set {inputPath = value; }
        }

        /// <summary>
        /// Property OutputPath
        /// </summary>
        public string OutputPath
        {
            get { return outputPath; }
            set { inputPath = value; }
        }

        /// <summary>
        /// Read the input and output path from keyboard
        /// </summary>
        public void readInputOutput()
        {
            receiveInputPath();
            receiveOutputPath();
        }

        /// <summary>
        /// Receive the input path from keyboard
        /// </summary>
        public virtual void receiveInputPath()
        {

        }

        /// <summary>
        /// Receive the input path from keyboard
        /// </summary>
        public virtual void receiveOutputPath()
        {

        }
    }
}
