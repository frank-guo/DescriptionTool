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
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                string inputFolder = null;
                string outputFile = null;
                ExtractInputProcessor inputProcessor = new ExtractInputProcessor();

                inputProcessor.receiveInputPath();
                inputFolder = inputProcessor.InputPath;
                inputProcessor.receiveOutputPath();
                outputFile = inputProcessor.OutputPath;

                var files = Directory.EnumerateFiles(inputFolder, "*.htm", SearchOption.AllDirectories);

                Console.WriteLine("Totally {0} .htm files found.", files.Count().ToString());
                Console.ReadKey();

                DescpExtractor extractor = new DescpExtractor(files);
                extractor.Extract();

                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }


                //ToDo:Pop up a dialog to retry
                save:
                try{
                    File.WriteAllText(outputFile, extractor.result.ToString());
                }
                catch(Exception e)
                {
                    DialogResult result = DialogResult.Abort;
                    try
                    {
                        result = MessageBox.Show("Please contact the developers with the"
                          + " following information:\n\n" + e.Message + e.StackTrace,
                          "Application Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                        if (result == DialogResult.Retry)
                        {
                            goto save;
                        }
                    }
                    finally
                    {
                        if (result == DialogResult.Abort)
                        {
                            Application.Exit();
                        }
                    }
                }


                /*
                XmlDocument xmlDoc = new XmlDocument();

                //Prepare the output file
                var result = @"C:\Users\LGuo\Desktop\TestFolderForDescTool\result.csv";

                if (File.Exists(result))
                {
                    File.Delete(result);
                }

                using (StreamWriter file = new StreamWriter(result))
                {
                    file.WriteLine("Source File Path,Output File Path,File Name,Topic Class,Meta Description");
                }


                foreach (var f in files)
                {
                    Console.WriteLine("{0}", f);

                    xmlDoc.Load(f);

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
                        continue;
                    }

                    //Extract the required information
                    var topic = xmlDoc.DocumentElement.GetAttribute("class");
                    string fileFullName = f;
                    string filePath = "";
                    string fileName = "";

                    getFilePathAndName(fileFullName, ref filePath, ref fileName);

                    string output = transform(filePath);

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

                    if (metaDescp == "\"\"")
                    {
                        continue;
                    }

                    var line = filePath + "," + output + "," + fileName + "," + topic + "," + metaDescp;


                    using (StreamWriter file = new StreamWriter(result, true))
                    {
                        file.WriteLine(line);
                    }

                }

                Console.ReadKey();
                 */
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
                     
        }



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

        static string extractDescp(XmlNode firstPargrah)
        {
            if (firstPargrah == null)
            {
                return null;
            }

            string metaDescp = "";

            metaDescp += '"';

            if (firstPargrah.InnerText != "")
            {
                metaDescp += firstPargrah.InnerText;
            }

            XmlNode immediateList = firstPargrah.NextSibling;

            if (immediateList != null && (immediateList.Name == "ul" || immediateList.Name == "ol" || immediateList.Name == "dl"))
            {
                metaDescp += getTextInLines(immediateList);
            }

            metaDescp += '"';

            return metaDescp;

        }

        static string transform(string filePath)
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


        static string getTextInLines(XmlNode list)
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

        static XmlNode getFirstParagraphAfterH1(XmlDocument xmlDoc)
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

        static XmlNode getFirstParagAfterOverViewH2(XmlDocument xmlDoc)
        {
            XmlNode h2 = xmlDoc.GetElementsByTagName("h2")[0];
            if ( h2 == null )
            {
                return null;
            }

            var firstChild = h2.FirstChild;
            if( firstChild != null && firstChild.Name !="span" ||
                firstChild.Attributes["class"] == null || firstChild.Attributes["class"].Value != "UIOverview")
            {
                return null;
            }

            XmlNode nextSibling = h2.NextSibling;
            while (nextSibling != null && nextSibling.Name != "p")
            {
                nextSibling = nextSibling.NextSibling;
            }

            return nextSibling;
        }

        static void getFilePathAndName(string fileFullName, ref string filePath, ref string fileName)
        {
            if (fileFullName == null)
            {
                return;
            }

            int idxOfLastSlash = fileFullName.LastIndexOf(@"\");

            if ( idxOfLastSlash != -1)
            {
                fileName = fileFullName.Substring(idxOfLastSlash + 1);
                filePath = fileFullName.Substring(0, idxOfLastSlash);
                return;
            }

            return;
        }
    }
}
