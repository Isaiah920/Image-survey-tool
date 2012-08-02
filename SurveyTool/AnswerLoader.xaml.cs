using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SurveyTool
{
    /// <summary>
    /// Interaction logic for AnswerLoader.xaml
    /// </summary>
    public partial class AnswerLoader : Window
    {
        List<List<IQuestions>> questions = new List<List<IQuestions>>();//List<<IQuestions>();

        public AnswerLoader()
        {
            InitializeComponent();
        }

        public void loadQuestions(string file)
        {

        }
    }
}
