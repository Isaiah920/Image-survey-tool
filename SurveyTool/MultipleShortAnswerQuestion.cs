using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using System.IO;

namespace SurveyTool
{
    [Serializable]
    public class MultipleShortAnswerQuestion:IQuestions, IXmlSerializable
    {
        private string questionString;
        private int numImages;
        TextBox[] imageTextbox;
        StackPanel[] questionStackPanel;

        //this is what we save when we save this type of question's results:
        //[Serializable]
        private string[] answer { get { return imageTextbox.Where(t => !string.IsNullOrEmpty(t.Text)).Select(t => t.Text).ToArray(); } }

        public bool Display(Grid questionPane)
        {
            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();

            questionPane.ColumnDefinitions.Add(col1);
            questionPane.ColumnDefinitions.Add(col2);

            for (int i = 0; i <= Math.Ceiling(numImages / 2.0); i++)
            {
                RowDefinition row = new RowDefinition();
                questionPane.RowDefinitions.Add(row);
            }
            if (questionStackPanel == null)
            {
                questionStackPanel = new StackPanel[numImages];
            }

            Label[] imageLabel = new Label[numImages];
            if (imageTextbox == null)
            {
                //we only want to make these the first time, so data persists as we flip between questions
                imageTextbox = new TextBox[numImages];
            }

            for (int i = 0; i < numImages; i++)
            {
                if (questionStackPanel[i] == null)
                {
                    questionStackPanel[i] = new StackPanel();
                    questionStackPanel[i].Orientation = Orientation.Horizontal;
                    questionStackPanel[i].HorizontalAlignment = HorizontalAlignment.Center;
                }
                else
                {
                    questionStackPanel[i].Children.Clear();  //TODO: probably not the best; save all instead?
                }
                imageLabel[i] = new Label();
                imageLabel[i].FontWeight = FontWeights.Bold;
                imageLabel[i].FontSize = 12;
                imageLabel[i].Content = (char)('A' + i);
                questionStackPanel[i].Children.Add(imageLabel[i]);
                if (imageTextbox[i] == null)
                {
                    //again, we only want to make these once, so their data persists
                    imageTextbox[i] = new TextBox();
                    imageTextbox[i].Width = 320;
                }
                questionStackPanel[i].Children.Add(imageTextbox[i]);
            }

            Label questionLabel = new Label();
            questionLabel.FontSize = 14;
            questionLabel.FontWeight = FontWeights.Bold;
            questionLabel.Content = questionString;
            questionPane.Children.Add(questionLabel);
            questionLabel.SetValue(Grid.ColumnSpanProperty, 2);

            int currCol = 0;
            int currRow = 1;
            for (int i = 0; i < numImages; i++)
            {
                questionPane.Children.Add(questionStackPanel[i]);
                questionStackPanel[i].SetValue(Grid.ColumnProperty, currCol);
                questionStackPanel[i].SetValue(Grid.RowProperty, currRow);

                //increment position
                if (currCol == 1)
                {
                    currRow++;
                    currCol = 0;
                }
                else
                {
                    currCol++;
                }
            }

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
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            for (int i = 0; i < numImages; i++)
            {
                info.AddValue("Answer"+i, imageTextbox[i].Text);
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Type", "MultipleShortAnswerQuestion");
            writer.WriteAttributeString("NumImages", ""+numImages);
            writer.WriteAttributeString("QuestionString", questionString);
            for (int i = 0; i < numImages; i++)
            {
                writer.WriteElementString("Answer" + i , imageTextbox[i].Text);
            }
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            numImages = int.Parse(reader.GetAttribute("NumImages")); //TODO: error checking!
            questionString = reader.GetAttribute("QuestionString");

            if (imageTextbox == null)
            {
                //since we're probably reading into a brand new question, this is almost certainly true:
                imageTextbox = new TextBox[numImages];
                for (int i = 0; i < numImages; i++)
                {
                     imageTextbox[i] = new TextBox();
                }
            }

            XmlReader inner = reader.ReadSubtree();
            inner.MoveToContent();

            int curr = 0;
            while (inner.Read())
            {
                if (inner.NodeType == XmlNodeType.Element && inner.Name == "Answer" + curr)
                {
                    XElement el = XNode.ReadFrom(inner) as XElement;
                    if (el != null)
                    {
                        imageTextbox[curr].Text = el.Value.ToString();
                        curr++;
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
            for (int i = 0; i < numImages; i++)
            {
                if (imageTextbox[i].Text == "") return false; //empty textbox, so unanswered
            }
            return true; //everything's filled in
        }

        public bool CondenseResultsToTable(IEnumerable<IQuestions> questionsList, int questionNumber, out string[,] table)
        {
            //the table will look like this:
            // QuestionNum  QuestionString
            //   (empty)    A                       B           ...
            //   (empty)    answerforfirstperson    anotherans
            //   (empty)    answerfor2ndperson      yetanotherans
            //   (empty)    ...

            //make sure these are all the right type, to avoid unexpected behaviour
            MultipleShortAnswerQuestion[] questions = new MultipleShortAnswerQuestion[questionsList.Count()];
            for (int i = 0; i < questions.Length; i++)
            {
                questions[i] = questionsList.ElementAt(i) as MultipleShortAnswerQuestion;
            }
            //MultipleShortAnswerQuestion[] questions = (MultipleShortAnswerQuestion[])questionsList.Where(x => x is MultipleShortAnswerQuestion);

            int numQuestions = questions.Count();

            table = new string[numQuestions + 2, 2*questions[0].numImages]; //extra row for heading, 
            //table[0, 0] = "" + questionNumber;
            table[0, 1] = questions[0].questionString; //they all have the same questionString, just pick one

            for (int i = 0; i < numImages; i++)
            {
                table[1, 2 * i+1] = "" + (char)('A' + i);
            }

            int currRow = 2;
            foreach (MultipleShortAnswerQuestion question in questions)
            {
                for (int i = 0; i < numImages; i++)
                {
                    table[currRow, 2*i+1] = question.imageTextbox[i].Text; //skip a column after each
                }
                currRow++;
            }

            return true;
        }
    }
}
