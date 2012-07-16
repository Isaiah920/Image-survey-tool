using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SurveyTool
{
    class ShortAnswerQuestion:IQuestions
    {
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
    }
}
