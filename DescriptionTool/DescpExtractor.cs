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
    class DescpExtractor
    {
        private IEnumerable<string> filesToExtract;
        private XmlDocument xmlDoc;
        public  StringBuilder result{get;set;}

        public DescpExtractor(IEnumerable<string> files)
        {
            result = new StringBuilder();
            filesToExtract = files;
            xmlDoc = new XmlDocument();
        }

        public void Extract()
        {
                
            try{
                result.AppendLine("Source File Path,Output File Path,File Name,Topic Class,Meta Description");

                foreach (var file in filesToExtract)
                {
                    //ToDo: launch an event to display the file number and succeed/fail and file counter, etc.
                    //ToDo: Pop up a dialog to select Continue or Abort when fail to load file

                    xmlDoc.Load(file);

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
                    string fileFullName = file;
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

                    //ToDo: Check the capacity of result is big enough
                    result.AppendLine(line);

                }

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
                     
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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

        private static string extractDescp(XmlNode firstPargrah)
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

        private static XmlNode getFirstParagAfterOverViewH2(XmlDocument xmlDoc)
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
                filePath = fileFullName.Substring(0, idxOfLastSlash);
                return;
            }

            return;
        }

    }
}
