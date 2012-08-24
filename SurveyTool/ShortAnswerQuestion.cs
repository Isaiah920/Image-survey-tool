using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace SurveyTool
{
    [Serializable]
    public class ShortAnswerQuestion:IQuestions, IXmlSerializable
    {
        private int numImages;
        private TextBox imageTextbox;
        private string questionString;

        //this is what we save when we save this type of question's results:
        //[Serializable]
        private string answer { get { return imageTextbox.Text; } }


        public bool Display(Grid questionPane)
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(30);
            questionPane.RowDefinitions.Add(row);
            RowDefinition row2 = new RowDefinition();
            questionPane.RowDefinitions.Add(row2);

            Label questionLabel = new Label();
            questionLabel.FontSize = 14;
            questionLabel.FontWeight = FontWeights.Bold;
            questionLabel.Content = questionString;
            questionPane.Children.Add(questionLabel);

            if (imageTextbox == null)
            {
                imageTextbox = new TextBox();
                imageTextbox.Width = questionPane.ActualWidth - 40;
                questionPane.Children.Add(imageTextbox);
                imageTextbox.Height = 45;

                //make multiline:
                imageTextbox.TextWrapping = TextWrapping.Wrap;
                imageTextbox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                imageTextbox.AcceptsReturn = true;
            }
            else
            {
                questionPane.Children.Add(imageTextbox);
            }
            imageTextbox.SetValue(Grid.RowProperty, 1);

            return true;
        }
        public bool Cleanup()
        {
            return true;
        }
        public bool Serialize()
        {
            return true;
        }
        public string GetQuestionString()
        {
            return questionString;
        }
        public bool SetQuestionString(string question)
        {
            questionString = question;
            return true;
        }

        public int GetNumImages()
        {
            return numImages;
        }
        public bool SetNumImages(int num)
        {
            numImages = num;
            return true;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Type", "ShortAnswerQuestion");
            writer.WriteAttributeString("NumImages", ""+numImages);
            writer.WriteAttributeString("QuestionString", questionString);
            writer.WriteElementString("Answer", answer);                    
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            numImages = int.Parse(reader.GetAttribute("NumImages")); //TODO: error checking!
            questionString = reader.GetAttribute("QuestionString");

            if (imageTextbox == null)
            {
                //since we're probably reading into a brand new question, this is almost certainly true:
                imageTextbox = new TextBox();
            }

            XmlReader inner = reader.ReadSubtree();
            inner.MoveToContent();

            while (inner.Read())
            {
                if (inner.NodeType == XmlNodeType.Element && inner.Name == "Answer")
                {
                    XElement el = XNode.ReadFrom(inner) as XElement;
                    if (el != null)
                    {
                        imageTextbox.Text = el.Value.ToString();
                        break;
                    }
                }
            }
            inner.Close();
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        public bool IsAnswered()
        {
            if (imageTextbox.Text == "") return false; //empty textbox, so unanswered
            return true; //filled in
        }

        public bool CondenseResultsToTable(IEnumerable<IQuestions> questionsList, int questionNumber, out string[,] table)
        {
            //the table will look like this:
            // QuestionNum  QuestionString
            //   (empty)    answerforfirstqn
            //   (empty)    answerfor2ndqn
            //   (empty)    ...

            //the table will look like this:
            // QuestionNum  QuestionString
            //   (empty)    option1         totalforoption1
            //   (empty)    option2         totalforoption2
            //   (empty)    ...

             //make sure these are all the right type, to avoid unexpected behaviour
            ShortAnswerQuestion[] questions = new ShortAnswerQuestion[questionsList.Count()];
            for (int i = 0; i < questions.Length; i++)
            {
                questions[i] = questionsList.ElementAt(i) as ShortAnswerQuestion;
            }
            //ShortAnswerQuestion[] questions = (ShortAnswerQuestion[])questionsList.Where(x => x is ShortAnswerQuestion);

            int numQuestions = questions.Count();

            table = new string[numQuestions + 1, 2]; //extra row for heading
            //table[0, 0] = "" + (questionNumber + 1);
            table[0, 1] = questions[0].questionString; //they all have the same questionString, just pick one

            int currRow = 1;
            foreach (ShortAnswerQuestion question in questions)
            {
                table[currRow++, 1] = question.answer;
            }

            return true;
        }
    }
}
