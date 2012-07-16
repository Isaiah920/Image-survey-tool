using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SurveyTool
{
    interface IQuestions
    {
        bool Display(Grid questionPane); //System.Windows.UIElement
        bool Cleanup();
        bool Serialize();


    }
}
