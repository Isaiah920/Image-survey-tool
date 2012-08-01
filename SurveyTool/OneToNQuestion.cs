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

using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace SurveyTool
{
    [Serializable]
    public class OneToNQuestion:IQuestions, IXmlSerializable
    {

        #region Properties

            public int NumChoices { get; set; }
            private int numImages;
            private string questionString;
            private RadioButton[,] radioChoices;
            private StackPanel[] questionStackPanel;
            private List<string> choiceLabels = new List<string>();
            public IList<string> ChoiceLabels
            {
                get { return choiceLabels.AsReadOnly(); }
            }

            //this is what we save when we save this type of question's results:
            //[Serializable]
            private int[] answer { get { return getRadioButtonAnswers(); } }

        #endregion

        #region Constructors

        
            public OneToNQuestion()
            {
            //    initialize(5, 4);
                //numImages = 4; //TODO not necessarily...
            }
            public OneToNQuestion(int numChoices, int numImages)
            {
                this.numImages = numImages;
                initialize(numChoices);
            }
            private void initialize(int numChoices)
            {
                questionString = "What is the answer to this question?";
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

            //for when we need to find the selected results:
            public int[] getRadioButtonAnswers()
            {
                int[] results = new int[numImages];
                for (int i=0; i<numImages; i++)
                {
                    for (int j=0; j<NumChoices; j++)
                    {
                        if (radioChoices[i,j].IsChecked == true) results[i] = j;
                    }
                }
                return results;
            }
            public bool SetChoiceLabel(int index, string label)
            {
                if (index >= 0 && index < NumChoices)
                {
                    choiceLabels[index] = label;
                    NumChoices = choiceLabels.Count;
                    return true;
                }
                return false; //invalid index
            }
            public bool SetChoiceLabels(IEnumerable<string> labels)
            {
                choiceLabels = labels.ToList<string>();
                NumChoices = choiceLabels.Count;
                return true;
            }

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
                        questionStackPanel[i].Children.Clear(); //TODO: probably not the best; save all instead?
                    }
                    imageLabel[i] = new Label();
                    imageLabel[i].FontWeight = FontWeights.Bold;
                    imageLabel[i].FontSize = 12;
                    imageLabel[i].Content = (char)('A' + i);
                    questionStackPanel[i].Children.Add(imageLabel[i]);

                    if (radioChoices == null)
                    {
                        //we only want to remake these the first time, so data persists as we flip between questions
                        radioChoices = new RadioButton[numImages, NumChoices];
                    }
                    for (int j = 0; j < NumChoices; j++)
                    {
                        if (radioChoices[i, j] == null)
                        {
                            //again, we only want to make these once, so their data persists
                            radioChoices[i, j] = new RadioButton();
                            radioChoices[i, j].Content = choiceLabels[j];
                            radioChoices[i, j].Margin = new Thickness(5);
                        }
                        questionStackPanel[i].Children.Add(radioChoices[i,j]);
                    }
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
            
        /*public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
            {
                for (int i = 0; i < numImages; i++)
                {
                    for (int j = 0; j < NumChoices; j++)
                    {
                        if (radioChoices[i, j].IsChecked == true)
                        {
                            info.AddValue("Answer" + i, j);
                        }
                    }
                } 
            }*/

            public void WriteXml(XmlWriter writer)
            {
                
                writer.WriteAttributeString("Type", "OneToNQuestion");
                writer.WriteAttributeString("NumImages", ""+numImages);
                writer.WriteAttributeString("QuestionString", questionString);
                for (int i = 0; i < numImages; i++)
                {
                    for (int j = 0; j < NumChoices; j++)
                    {
                        if (radioChoices[i, j].IsChecked == true)
                        {
                            writer.WriteElementString("Answer" + i, ""+j);
                        }
                    }
                } 
            }

            public void ReadXml(XmlReader reader)
            {
                //personName = reader.ReadString();
            }

            public XmlSchema GetSchema()
            {
                return (null);
            }


            public bool IsAnswered()
            {
                for (int i = 0; i < numImages; i++)
                {
                    if (getAnswer(i) == null) return false;
                }
                return true;
            }
            private int? getAnswer(int imageNum)
            {
                for (int j = 0; j < NumChoices; j++)
                {
                    if (radioChoices[imageNum, j].IsChecked == true)
                    {
                        return j; //our answer
                    }
                }
                return null; //couldn't find anything checked, so it hasn't been answered
            }

        #endregion

    }
}
