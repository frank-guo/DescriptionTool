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
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                ChooseTool:
                Console.WriteLine("Please choose 1/2: (1)Extract description       (2)Add description\n");
                string input =  Console.ReadLine();
                if (input != "1" && input != "2")
                {
                    Console.WriteLine("Wrong Choise, Retry");
                    goto ChooseTool;
                }

                
                ToolType toolType = (ToolType)(Convert.ToInt32(input) - 1);

                if (toolType == ToolType.Extractor)
                {

                    string inputFolder = null;
                    string outputFile = null;
                    ExtractInputProcessor inputProcessor = new ExtractInputProcessor();

                    inputProcessor.receiveInputPath();
                    inputFolder = inputProcessor.InputPath;
                    inputProcessor.receiveOutputPath();
                    outputFile = inputProcessor.OutputPath;

                    FileWriter outputWriter = new FileWriter(outputFile);
                    //Put the log file in output folder
                    Logger logger = new Logger(Path.GetDirectoryName(outputFile));
                    DescpExtractor extractor = new DescpExtractor(inputFolder, logger, outputWriter);

                    Console.WriteLine("Totally {0} .htm files found.", extractor.TotalNumOfFiles.ToString());
                    Console.ReadKey();


                    extractor.Extract();

                    outputWriter.write(extractor.result.ToString());

                    Console.WriteLine("Type any key to exit");
                    Console.ReadKey();
                }

                if (toolType == ToolType.Adder)
                {
                    string inputFile = null;
                    string outputFolder = null;
                    AddInputProcessor inputProcessor = new AddInputProcessor();

                    inputProcessor.receiveInputPath();
                    inputFile = inputProcessor.InputPath;
                    inputProcessor.receiveOutputPath();
                    outputFolder = inputProcessor.OutputPath;

                    //put the log file in the input folder
                    var inputFolder = Path.GetDirectoryName(inputFile);
                    Logger logger = new Logger(Path.GetDirectoryName(inputFile));
                    DescpAdder adder = new DescpAdder(outputFolder, inputFile, logger);

                    Console.WriteLine("Totally {0} .htm files found.", adder.TotalNumOfFiles.ToString());
                    Console.ReadKey();


                    adder.process();

                    Console.WriteLine("Type any key to exit");
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

        }


        /*
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DialogResult result = DialogResult.Abort;
            Exception ex = (Exception)e.ExceptionObject;
            if (ex is DirectoryNotFoundException)
            {
                result = MessageBox.Show("Whoops! Please contact the developers with the"
                  + " following information:\n\n" + ex.Message + ex.StackTrace,
                  "Application Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
            }
            
        }
         */
    }
}
