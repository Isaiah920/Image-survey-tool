using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace SurveyTool
{
    public interface IQuestions:IXmlSerializable
    {
        bool Display(Grid questionPane); //System.Windows.UIElement
        bool Cleanup();
        bool Serialize();

        string GetQuestionString();
        bool SetQuestionString(string question);

        int GetNumImages();
        bool SetNumImages(int num);

        bool IsAnswered();

        //void WriteXml(XmlWriter writer);
        //void ReadXml(XmlReader reader);
        //XmlSchema GetSchema();

        bool CondenseResultsToTable(IEnumerable<IQuestions> questions, int questionNumber, out string[,] table);
    }
}
