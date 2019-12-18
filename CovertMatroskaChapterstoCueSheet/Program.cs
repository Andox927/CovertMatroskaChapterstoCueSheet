using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ConvertMkvChToFlacCue
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML (*.xml)|*.xml";
            ofd.ShowDialog();

            if (ofd.FileName != "" )
            {
                var ch_xml = new XmlDocument();
                ch_xml.Load(ofd.FileName);
                var TrackName = Path.GetFileNameWithoutExtension(ofd.FileName);
                var outputPath = Path.Combine(Path.GetDirectoryName(ofd.FileName), TrackName + ".cue");

                var TrackList = ConvertToCue(ch_xml);
                using(StreamWriter sw = new StreamWriter(outputPath, false))
                {
                    sw.WriteLine("PERFORMER \"\"");
                    sw.WriteLine("TITLE \"\"");
                    sw.WriteLine("FILE \"" + TrackName + ".wav\"  WAVE");
                    sw.Write(TrackList);
                }
            }
            else
            {
                Console.WriteLine("Please Chose a File.");
            }
            
        }

        static StringWriter ConvertToCue(XmlDocument xmlDoc)
        {
            StringWriter sw = new StringWriter();

            XmlNodeList nodeList = xmlDoc.SelectNodes("/Chapters/EditionEntry/ChapterAtom");

            foreach (XmlNode node in nodeList)
            {
                var strUID = node.SelectSingleNode("ChapterUID").InnerText;
                var strTITLE = node.SelectSingleNode("ChapterDisplay/ChapterString").InnerText;
                var stTime = node.SelectSingleNode("ChapterTimeStart").InnerText;
                var IndexTime = ConvertIndexTime(stTime);

                sw.WriteLine("  TRACK " + strUID.PadLeft(2,'0') + " AUDIO");
                sw.WriteLine("    TITLE \"" + strTITLE + "\"");
                sw.WriteLine("    INDEX 01 " + IndexTime);

            }

            return sw;
        }

        static string ConvertIndexTime(string stTime)
        {
            string rtnTime = "";
            var hh = int.Parse(stTime.Split(":")[0]);
            var mm = int.Parse(stTime.Split(":")[1]);
            var ss = int.Parse(stTime.Split(":")[2].Split(".")[0]);
            var ms = int.Parse(stTime.Split(".")[1].Substring(0,3));

            mm = hh * 60 + mm;
            ms = ms * 3 / 40;  //Convert to 'frame' 1/75 sec.
            rtnTime = String.Format("{0:00}", mm) + ":" + String.Format("{0:00}", ss) + ":" + String.Format("{0:00}", ms);

            return rtnTime;
        }
    }
}
