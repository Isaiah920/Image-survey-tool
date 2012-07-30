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

namespace SurveyTool
{
    [Serializable]
    class MultipleShortAnswerQuestion:IQuestions
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
        public bool IsAnswered()
        {
            for (int i = 0; i < numImages; i++)
            {
                if (imageTextbox[i].Text == "") return false; //empty textbox, so unanswered
            }
            return true; //everything's filled in
        }
    }
}
