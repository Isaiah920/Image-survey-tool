using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace SurveyTool
{
    [Serializable]
    public class ImageSet : IXmlSerializable
    {
        private List<IQuestions> questionList = new List<IQuestions>();
        private List<String> imagePath = new List<string>();
        private int currQuestion = 0;

        #region Properties

            public int NumQuestions 
            { 
                get { return questionList.Count; } 
            }
            public bool HasPrevQuestion
            {
                get { return currQuestion > 0; } //first question is 1
            }
            public bool HasNextQuestion
            {
                get { return currQuestion < NumQuestions -1; }
            }
            public int NumImages
            {
                get { return imagePath.Count; }
            }
            public List<int> UnansweredQuestionsList
            {
                get
                {
                    List<int> unanswered = new List<int>();
                    for(int i=0; i<questionList.Count; i++)
                    {
                        if (!questionList[i].IsAnswered())
                        {
                            unanswered.Add(i); //add to unanswered list
                        }
                    }
                    return unanswered;
                }
            }

        #endregion
        
        #region Public Methods

            public ImageSet()
            {

            }

        /*
            public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
            {

            }*/

            public void WriteXml(XmlWriter writer)
            {
                //writer.WriteStartElement("ImageSet");
                /*for (int i = 0; i < NumImages; i++)
                {
                    writer.WriteAttributeString("Image", imagePath[i]);       
                }*/
                for (int i = 0; i < NumQuestions; i++)
                {
                    writer.WriteStartElement("Question");
                    questionList[i].WriteXml(writer);
                    writer.WriteEndElement();
                    //info.AddValue("Question"+i, questionList[i]);
                }
                //writer.WriteEndElement();
            }

            public void ReadXml(XmlReader reader)
            {
                //personName = reader.ReadString();
            }

            public XmlSchema GetSchema()
            {
                return (null);
            }

            public void AddQuestion(IQuestions question)
            {
                questionList.Add(question);
            }
            public void RemoveQuestion(IQuestions question)
            {
                questionList.Remove(question);
            }
            public IQuestions GetCurrentQuestion() //we've just come to this question from another image list; resume where we left off
            {
                return questionList[currQuestion]; //TODO: check this -- getprev at 1 = 0
            }
            public IQuestions GetPreviousQuestion()
            {
                return questionList[--currQuestion]; //TODO: check this -- getprev at 1 = 0
            }
            public IQuestions GetNextQuestion()
            {
                return questionList[++currQuestion]; //TODO: check this -- getnext at 0 = 1

            }
            public void AddPicture(string path)
            {
                imagePath.Add(path);
            }
            public string GetImageAt(int index)
            {
                return imagePath[index];
            }

        #endregion

    }
}
