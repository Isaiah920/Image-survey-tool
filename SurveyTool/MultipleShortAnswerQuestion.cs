using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SurveyTool
{
    class MultipleShortAnswerQuestion:IQuestions
    {
        private string questionString;
        private int numImages;

        public bool Display(Grid questionPane)
        {
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
