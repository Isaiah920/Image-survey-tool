using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace SurveyTool
{
    public interface IQuestions:IXmlSerializable
    {
        bool Display(Grid questionPane);
        bool Cleanup();
        bool Serialize();

        string GetQuestionString();
        bool SetQuestionString(string question);

        int GetNumImages();
        bool SetNumImages(int num);

        bool IsAnswered();

        //Everything also has this, but it's implied by IXmlSerializable, so no need to explicitly write it here:
        //void WriteXml(XmlWriter writer);
        //void ReadXml(XmlReader reader);
        //XmlSchema GetSchema();

        bool CondenseResultsToTable(IEnumerable<IQuestions> questions, int questionNumber, out string[,] table);
    }
}
