using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SurveyTool
{
    [Serializable()]
    public class SurveySettings
    {
        public bool fileNameRelative = false;
        public string fileName = "";
        public string fullFileName = "";

        public bool folderNameRelative = false;
        public string folderName = "";
        public string fullFolderName = new Uri(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath)).ToString();
    }
}
