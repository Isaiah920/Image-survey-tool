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

namespace SurveyTool
{
    class MultipleShortAnswerQuestion:IQuestions
    {
        private string questionString;
        private int numImages;
        TextBox[] imageTextbox;

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

            StackPanel[] questionStackPanel = new StackPanel[numImages];

            Label[] imageLabel = new Label[numImages];
            imageTextbox = new TextBox[numImages];

            for (int i = 0; i < numImages; i++)
            {
                questionStackPanel[i] = new StackPanel();
                questionStackPanel[i].Orientation = Orientation.Horizontal;
                questionStackPanel[i].HorizontalAlignment = HorizontalAlignment.Center;
                imageLabel[i] = new Label();
                imageLabel[i].FontWeight = FontWeights.Bold;
                imageLabel[i].FontSize = 12;
                imageLabel[i].Content = (char)('A' + i);
                questionStackPanel[i].Children.Add(imageLabel[i]);
                imageTextbox[i] = new TextBox();
                imageTextbox[i].Width = 320;
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
    }
}
