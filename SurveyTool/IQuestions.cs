using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SurveyTool
{
    public interface IQuestions
    {
        bool Display(Grid questionPane); //System.Windows.UIElement
        bool Cleanup();
        bool Serialize();

        string GetQuestionString();
        bool SetQuestionString(string question);

        int GetNumImages();
        bool SetNumImages(int num);
    }
}
