/* Copyright (c) 1994-2015 Sage Software, Inc.  All rights reserved. */

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
    /// <summary>
    /// Class DescpExtractor
    /// </summary>
    class DescpExtractor: IDataProcessor
    {
        private IEnumerable<string> files;
        private XmlDocument xmlDoc;
        private Logger logger;
        private int totalNumOfFiles;
        private FileWriter outputWriter;
        private string inputFolder;
        private string outputFile;
        public  StringBuilder result{get;set;}

        /// <summary>
        /// DescpExtractor Constructor
        /// </summary>
        /// <param name="inputReceiver"></param>
        public DescpExtractor(InputReceiverBase inputReceiver)
        {
            result = new StringBuilder();
            this.inputFolder = inputReceiver.InputPath;
            this.outputFile = inputReceiver.OutputPath;
            this.files = Directory.EnumerateFiles(inputReceiver.InputPath, "*.htm", SearchOption.AllDirectories);
            xmlDoc = new XmlDocument();
            this.logger = new Logger(Path.GetDirectoryName(inputReceiver.OutputPath));
            this.totalNumOfFiles = files.Count();
            outputWriter = new FileWriter(inputReceiver.OutputPath);
        }

        /// <summary>
        /// Property TotalNumOfFiles
        /// </summary>
        public int TotalNumOfFiles
        {
            get
            {
                return totalNumOfFiles;
            }

        }

        /// <summary>
        /// Process the input files and generate extracted description
        /// </summary>
        public void process()
        {
                
            try{
                result.AppendLine("Source File Path,Output File Path,File Name,Topic Class,Meta Description");

                int fileCount = 0;

                foreach (var file in files)
                {
                    //ToDo: Use a background worker thread to do extraction,
                    //ToDo: and fire an event to display the file number and succeed/fail and file counter, etc in console.

                    fileCount++;
                    Console.Write("{0}/{1} Processing description in {2} ", fileCount, totalNumOfFiles, file);
                    try
                    {
                        xmlDoc.Load(file);
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            string loginfo = string.Format("Processing {0} failed \n {1}", file, e.Message);
                            logger.writeLine(loginfo);
                            Console.WriteLine("failed");
                        }
                        catch(Exception logEx)
                        {
                            Console.WriteLine("failed");
                            Console.WriteLine(e.Message + logEx.Message);
                        }
                        continue;
                    }

                    //Check if it contains meta tag 
                    bool isContainDesc = false;

                    XmlNodeList nodes = xmlDoc.DocumentElement.GetElementsByTagName("meta");
                    foreach (XmlNode node in nodes)
                    {
                        if (node.Attributes["name"] != null && node.Attributes["content"] != null)
                        {

                            if (node.Attributes["name"].Value == "description")
                            {
                                isContainDesc = true;
                                break;
                            }
                        }
                    }

                    if (isContainDesc)
                    {
                        Console.WriteLine("succeeded");
                        continue;
                    }

                    //Check if html element has any of the following class values
                    bool isConcept = xmlDoc.DocumentElement.GetAttribute("class") == "concept";
                    bool isMinitoc = xmlDoc.DocumentElement.GetAttribute("class") == "minitoc";
                    bool isReference = xmlDoc.DocumentElement.GetAttribute("class") == "reference";
                    bool isScreenguide = xmlDoc.DocumentElement.GetAttribute("class") == "screenguide";
                    bool isTask = xmlDoc.DocumentElement.GetAttribute("class") == "task";
                    bool isAnyFiveValues = isConcept || isMinitoc ||
                                            isReference || isScreenguide || isTask;

                    if (!isAnyFiveValues)
                    {
                        Console.WriteLine("succeeded");
                        continue;
                    }

                    //Extract the required information
                    var topic = xmlDoc.DocumentElement.GetAttribute("class");
                    string fileFullName = file;
                    string fileFullPath = "";
                    string fileName = "";

                    getFilePathAndName(fileFullName, ref fileFullPath, ref fileName);
                    string fileRelativePath = relativePath(fileFullPath, inputFolder);

                    string outputRelativePath = transform(fileRelativePath);

                    string metaDescp = "";

                    if (topic == "concept" || topic == "minitoc" || topic == "reference" || topic == "task")
                    {
                        XmlNode firstPargrah = getFirstParagraphAfterH1(xmlDoc);
                        metaDescp = extractDescp(firstPargrah);

                    }

                    if (topic == "screenguide")
                    {
                        XmlNode firstPargrah = getFirstParagAfterOverViewH2(xmlDoc);
                        metaDescp = extractDescp(firstPargrah);
                    }

                    if (metaDescp == "" || metaDescp == null )
                    {
                        Console.WriteLine("succeeded");
                        continue;
                    }

                    //strip out all quotes in metaDescp, and then enclose it with double quotes
                    if (metaDescp.Contains('"'))
                    {
                        metaDescp = metaDescp.Replace('"', ' ');
                    }

                    if (metaDescp.Contains('\''))
                    {
                        metaDescp = metaDescp.Replace('\'', ' ');
                    }

                    metaDescp = '"' + metaDescp + '"';

                    var line = fileRelativePath + "," + outputRelativePath + "," + fileName + "," + topic + "," + metaDescp;

                    //Check if the capacity of result is big enough to hold the new line
                    if (result.Length + line.Length < result.MaxCapacity)
                    {
                        result.AppendLine(line);                        
                    }
                    else
                    {
                        outputWriter.write(result.ToString());
                        result.Clear();
                        result.AppendLine(line);
                    }

                    Console.WriteLine("succeeded. Get the Description!");

                }

                outputWriter.write(result.ToString());
                Console.WriteLine("Failed to process {0} files. Details in {1}\\log.txt. ", logger.FailCount, Path.GetDirectoryName(outputFile));
                Console.WriteLine("Finish writing description to the output file!");                
            }
            catch (Exception e)
            {
                Console.Write(e.Message + "\n");
            }
                     
        }

        /// <summary>
        /// Write log info
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="filePath">File which produces the exception</param>
        private void writeLog(Exception exception, String filePath)
        {
            try
            {
                string loginfo = string.Format("Processing {0} failed \n {1}", filePath, exception.Message);
                logger.writeLine(loginfo);
                Console.WriteLine("failed");
            }
            catch (Exception logEx)
            {
                Console.WriteLine("failed");
                Console.WriteLine(exception.Message + logEx.Message);
            }
        }

        /// <summary>
        /// Get the rest path relative to root
        /// </summary>
        /// <param name="fullPath">Full path</param>
        /// <param name="root">Root path</param>
        /// <returns>Relative path</returns>
        private string relativePath(string fullPath, string root)
        {            
            if (!fullPath.StartsWith(root) || fullPath==null || root == null)
            {
                return null;
            }

            //If fullPath equals to root, then return ""
            if (fullPath.Length > root.Length)
            {
                return fullPath.Substring(root.Length + 1);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Etract the required description
        /// </summary>
        /// <param name="firstPargrah">Xml P node</param>
        /// <returns>Descrption</returns>
        private static string extractDescp(XmlNode firstPargrah)
        {
            if (firstPargrah == null)
            {
                return null;
            }

            string metaDescp = "";

            if (firstPargrah.InnerText != "")
            {
                metaDescp += firstPargrah.InnerText;
            }

            XmlNode immediateList = firstPargrah.NextSibling;

            if (immediateList != null && (immediateList.Name == "ul" || immediateList.Name == "ol" || immediateList.Name == "dl"))
            {
                metaDescp += getTextInLines(immediateList);
            }

            return metaDescp;

        }

        /// <summary>
        /// Transfrom input file paths to output file paths
        /// </summary>
        /// <param name="filePath">Input file path</param>
        /// <returns>Output file path</returns>
        private static string transform(string filePath)
        {
            if (filePath == null)
            {
                return null;
            }

            string retStr = filePath;

            retStr = retStr.Replace(@"Portal\Content", "Content");
            retStr = retStr.Replace("Financials", "Subsystems");
            retStr = retStr.Replace("Operations", "Subsystems");

            return retStr;
        }

        /// <summary>
        /// Get the text in an Xml list
        /// </summary>
        /// <param name="list">Xml list node</param>
        /// <returns>text</returns>
        private static string getTextInLines(XmlNode list)
        {
            if ( list == null ){
                return null;
            }

            string retStr = "";
            XmlNodeList items = list.ChildNodes;

            foreach (XmlNode item in items)
            {
                if (item.InnerText != "")
                {
                    retStr += "\n" + item.InnerText;
                }
            }

            return retStr;
        }

        /// <summary>
        /// Get the first paragraph after the first heading h1
        /// </summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <returns>The first paragraph node</returns>
        private static XmlNode getFirstParagraphAfterH1(XmlDocument xmlDoc)
        {
            XmlNode h1 = xmlDoc.GetElementsByTagName("h1")[0];
            if (h1 == null)
            {
                return null;
            }

            XmlNode nextSibling = h1.NextSibling;
            while (nextSibling != null && nextSibling.Name != "p")
            {
                nextSibling = nextSibling.NextSibling;
            }

            return nextSibling;
        }

        /// <summary>
        /// Get the first paragraph after the over view heading h2
        /// </summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <returns>The first pargraph node</returns>
        private static XmlNode getFirstParagAfterOverViewH2(XmlDocument xmlDoc)
        {
            XmlNode h2 = xmlDoc.GetElementsByTagName("h2")[0];
            if ( h2 == null )
            {
                return null;
            }

            var firstChild = h2.FirstChild;

            if (firstChild.Name == "#text")
            {
                if (h2.InnerText != "Overview")
                {
                    return null;
                }
            }
            else
            {
                if (firstChild.Name != "span" || firstChild.Attributes["class"] == null
                        || firstChild.Attributes["class"].Value != "UIOverview" || h2.InnerText != "Overview")
                {
                    return null;
                }
            }

            XmlNode nextSibling = h2.NextSibling;
            while (nextSibling != null && nextSibling.Name != "p")
            {
                nextSibling = nextSibling.NextSibling;
            }

            return nextSibling;
        }

        /// <summary>
        /// Get the file path and name
        /// </summary>
        /// <param name="fileFullName">Full file name</param>
        /// <param name="filePath">Reference to file path</param>
        /// <param name="fileName">Reference to file name</param>
        private static void getFilePathAndName(string fileFullName, ref string filePath, ref string fileName)
        {
            if (fileFullName == null)
            {
                return;
            }

            int idxOfLastSlash = fileFullName.LastIndexOf(@"\");

            if ( idxOfLastSlash != -1)
            {
                fileName = fileFullName.Substring(idxOfLastSlash + 1);
                //filePath does not end with "\"
                filePath = fileFullName.Substring(0, idxOfLastSlash);
                return;
            }

            return;
        }

    }
}
