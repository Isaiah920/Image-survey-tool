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
    class ShortAnswerQuestion:IQuestions
    {
        private string questionString;
        private int numImages;
        TextBox imageTextbox;
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
    }
}
