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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace SurveyTool
{
    public class OneToNQuestion:IQuestions
    {

        #region Properties

            public int NumChoices { get; set; }
            private int numImages = 4;
            public string QuestionString { get; set; }

            private List<string> choiceLabels = new List<string>();
            public IList<string> ChoiceLabels
            {
                get { return choiceLabels.AsReadOnly(); }
            }

        #endregion

        #region Constructors

        
            public OneToNQuestion()
            {
            //    initialize(5, 4);
            }
            public OneToNQuestion(int numChoices, int numImages)
            {
                this.numImages = numImages;
                initialize(numChoices);
            }
            private void initialize(int numChoices)
            {
                QuestionString = "What is the answer to this question?";
                if (numChoices < 0)
                {
                    //just quietly pick something sensible, cause we don't want to error out of the constructor, but <1 won't do
                    numChoices = 5;             
                }
                this.NumChoices = numChoices;
                for (int i=0; i<numChoices; i++)
                {
                    //use "1", "2", etc for the labels unless the user picks something else later
                    choiceLabels.Add("");
                    SetChoiceLabel(i, "" + (i+1));
                }
            }

        #endregion

        #region Public Methods

            public bool SetChoiceLabel(int index, string label)
            {
                if (index >= 0 && index < NumChoices)
                {
                    choiceLabels[index] = label;
                    return true;
                }
                return false; //invalid index
            }
            public bool SetChoiceLabels(IEnumerable<string> labels)
            {
                choiceLabels = labels.ToList<string>();
                return true;
            }

            public bool Display(Grid questionPane)
            {

                //questionPane = new Grid(); //just to be sure it's really empty to begin with

                ColumnDefinition col1 = new ColumnDefinition();
                ColumnDefinition col2 = new ColumnDefinition();
                //ColumnDefinition col3 = new ColumnDefinition();

                questionPane.ColumnDefinitions.Add(col1);
                questionPane.ColumnDefinitions.Add(col2);
                //questionPane.ColumnDefinitions.Add(col3);

                for (int i = 0; i <= Math.Ceiling(numImages / 2.0); i++)
                {
                    RowDefinition row = new RowDefinition();
                    questionPane.RowDefinitions.Add(row);
                }

                //Button x = new Button();
                //questionPane.Children.Add(x);
                    //System.Windows.UIElement

                StackPanel[] questionStackPanel = new StackPanel[numImages];

                Label[] imageLabel = new Label[numImages];

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

                    RadioButton[,] radioChoices = new RadioButton[numImages, NumChoices];
                    for (int j = 0; j < NumChoices; j++)
                    {
                        radioChoices[i,j] = new RadioButton();
                        questionStackPanel[i].Children.Add(radioChoices[i,j]);
                        radioChoices[i,j].Content = "" + (j + 1);
                        radioChoices[i,j].Margin = new Thickness(5);
                    }
                }

                Label questionLabel = new Label();
                questionLabel.FontSize = 14;
                questionLabel.FontWeight = FontWeights.Bold;
                questionLabel.Content = QuestionString;
                questionPane.Children.Add(questionLabel);
                questionLabel.SetValue(Grid.ColumnSpanProperty, 2);

                int currCol = 0;
                int currRow = 1;
                for (int i = 0; i < numImages; i++)
                {
                    questionPane.Children.Add(questionStackPanel[i]);
                    //Grid.SetColumn(questionPane, i);

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

            //for survey-making GUI
            public string SummaryString()
            {
                return "Scale of 1 to " + NumChoices + "(Labelled " + choiceLabels[0] + "..." + choiceLabels[NumChoices - 1] + ")";
            }

            public string SummaryName() //name of question type; shown when adding a new question in survey-making GUI
            {
                return "Scale of 1 to N";
            }

            public void DisplayEditDialog(Grid editPane)
            {
            }


        #endregion

    }
}
