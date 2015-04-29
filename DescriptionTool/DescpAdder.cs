using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DescriptionTool
{
    class DescpAdder: IDataProcessor
    {
        private XmlDocument xmlDoc;
        private Logger logger;
        private int totalNumOfFiles;
        private string inputFile;
        private string outputFolder;
        private List<InputRecord> inputRecords;
        private struct InputRecord
        {
            public string inputFile;
            public string description;

            public InputRecord(string filePath, string descp)
            {
                inputFile = filePath;
                description = descp;
            }
        }

        public DescpAdder(InputReceiverBase inputReceiver)
        {
            xmlDoc = new XmlDocument();
            this.logger = new Logger(Path.GetDirectoryName(inputReceiver.InputPath));
            this.inputFile = inputReceiver.InputPath;
            this.outputFolder = inputReceiver.OutputPath;
            IntializeInputRecords();
            totalNumOfFiles = inputRecords.Count;
        }

        public int TotalNumOfFiles
        {
            get
            {
                return totalNumOfFiles;
            }

        }

        public void process()
        {

            try
            {
                //Tranverse all the input files to insert the description
                int counter = 0;
                foreach (var inputRecord in inputRecords)
                {
                    counter++;
                    Console.Write("{0}/{1} Processing description in {2} ", counter, totalNumOfFiles, inputRecord.inputFile);

                    try
                    {
                        xmlDoc.Load(inputRecord.inputFile);
                    }
                    catch (Exception e)
                    {
                        writeLog(e, inputRecord.inputFile);
                        continue;
                    }

                    //Find the head tag 
                    XmlNodeList nodes = xmlDoc.DocumentElement.GetElementsByTagName("head");
                    foreach (XmlNode node in nodes)
                    {
                        var firstChild = node.FirstChild;
                        XmlElement descriptionElement = xmlDoc.CreateElement("meta");

                        descriptionElement.SetAttribute("name", "description");
                        descriptionElement.SetAttribute("content", inputRecord.description);
                        node.InsertBefore(descriptionElement, firstChild);
                    }                        

                    //Write back xmlDoc to its file
                    try
                    {
                        XmlWriter writer = XmlWriter.Create(inputRecord.inputFile);
                        xmlDoc.WriteTo(writer);
                        Console.WriteLine("succeeded. Insert the Description!");
                    }
                    catch (Exception e)
                    {
                        writeLog(e, inputRecord.inputFile);
                    }
                }

                Console.WriteLine("Type in any key to exit..");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

        }

        private void IntializeInputRecords () 
        {
            //Get the list of output file name and meta description
            GenerateInputRecords:
            try
            {
                string[] inputLines = File.ReadAllLines(inputFile);
                inputRecords = new List<InputRecord>();
                bool isDescpClose = true;
                string filePath = "";
                string descp = "";
                string[] columns = null;
                string revisedLine = "";

                //Note the fourth column 'meta description' may contain charater '\n'
                //Therefore some line may start with description
                for (int i = 1; i < inputLines.Length; i++ )
                {
                    string line = inputLines[i];
                    if (isDescpClose)
                    {
                        columns = line.Split(',');
                        filePath = columns[1] + "\\" + columns[2];
                        revisedLine = columns[4].Substring(1);        //Remove first double quote mark
                    }
                    else
                    {
                        revisedLine = line;
                    }

                    if (revisedLine.Contains('"'))
                    {
                        isDescpClose = true;
                        descp += revisedLine.Substring(0, line.Length - 1);    //Trim off the double quote mark at the tail
                        InputRecord inputRecord = new InputRecord(filePath, descp);
                        inputRecords.Add(inputRecord);
                    }
                    else
                    {
                        descp += revisedLine;
                        isDescpClose = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Write("Would you like to retry?(Y/N)");
                string retry = Console.ReadLine();
                if (retry == "Y")
                {
                    goto GenerateInputRecords;
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void writeLog(Exception e, String filePath)
        {
            try
            {
                string loginfo = string.Format("Processing {0} failed \n {1}", filePath, e.Message);
                logger.writeLog(loginfo);
                Console.WriteLine("failed");
            }
            catch (Exception logEx)
            {
                Console.WriteLine("failed");
                Console.WriteLine(e.Message + logEx.Message);
            }
        }

    }
}
