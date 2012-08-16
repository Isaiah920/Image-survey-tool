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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using System.IO;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace SurveyTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StartWindow startWindow;

        public MainWindow(StartWindow parent)
        {
            InitializeComponent();
            //Grid x;
            //x.Children.Add(
            startWindow = parent;

        }
        int currNumImages = 4;
        int currImageSet = -1;
        int currQuestion = -1;
        public int TotalNumQuestions { get; set; }
        public Window GreyBackgroundWindow;

        ImageViewer[] imView;
        List<ImageSet> imageSetList = new List<ImageSet>();

        public void test()
        {
        }

        public void InitializeImageList(List<ImageSet> imageSet)
        {
            this.imageSetList = imageSet;
        }
        public void StartDisplaying()
        {

            //newWindow.

            displayNextQuestion();
            QuestionProgressBar.Maximum = TotalNumQuestions;
            updateCurrQuestionLabel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void updateCurrQuestionLabel()
        {
            CurrQuestionLabel.Content = (currQuestion + 1) + " / " + TotalNumQuestions;
            QuestionProgressBar.Value = currQuestion + 1;
        }
        private void resetQuestionGrid()
        {
            QuestionGrid.Children.Clear(); //get rid of everything from previous question
            QuestionGrid.ColumnDefinitions.Clear();
            QuestionGrid.RowDefinitions.Clear();
        }

        private void displayNextQuestion()
        {
            resetQuestionGrid();
            IQuestions x;

            //if there are no more questions for our current image set, advance to the next:
            // OR if this is our very first question
            if (currQuestion == -1 || !imageSetList[currImageSet].HasNextQuestion) //(currQuestion == -1 ||
            {
                displaySet(++currImageSet);
                //if we've just come from the previous image set, display the first question in the set:
                x = imageSetList[currImageSet].GetCurrentQuestion();
            }
            else
            {
                x = imageSetList[currImageSet].GetNextQuestion();
            }

            //enable/disable previous and next buttons, depending on our position:
            currQuestion++;
            //NextButton.IsEnabled = currQuestion < TotalNumQuestions - 1;
            PreviousButton.IsEnabled = currQuestion > 0; //false for first question

            x.Display(QuestionGrid);
            updateCurrQuestionLabel();
        }
        private void displayPreviousQuestion()
        {
            resetQuestionGrid();
            IQuestions x;
            //if there are no more questions for our current image set, advance to the next:
            if (!imageSetList[currImageSet].HasPrevQuestion)
            {
                displaySet(--currImageSet);
                //if we've just come from the next image set, display the last question we left off at:
                x = imageSetList[currImageSet].GetCurrentQuestion();
            }
            else
            {
                x = imageSetList[currImageSet].GetPreviousQuestion();
            }

            //enable/disable previous and next buttons, depending on our position:
            currQuestion--;
            NextButton.IsEnabled = true;
            PreviousButton.IsEnabled = currQuestion > 0;

            x.Display(QuestionGrid);
            updateCurrQuestionLabel();
        }

        private void displaySet(int index)
        {
            if (imView != null)
            {
                foreach (ImageViewer im in imView)
                {
                    //TODO: would be better to just change image, position later
                    im.Close();
                }
            }
            currImageSet = index; //remember this, since we'll need to get questions from here later
            currNumImages = imageSetList[index].NumImages;

            //TODO: would be nice to have images maintain their position if they are moved around, 
            //rather than having them "snap back" upon changing image sets...
            imView = new ImageViewer[currNumImages];

            int width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            this.Left = width / 2 - this.Width / 2;
            this.Top = (height - 200);

            //TODO: load in question instead of this
            //OneToNQuestion x = new OneToNQuestion(5, 4);
            //x.Display(QuestionGrid);

            int numRows = (int)Math.Ceiling(currNumImages / 2.0); //always use 2 columns
            int currRow = 0;
            int currCol = 0;


            for (int i = 0; i < currNumImages; i++)
            {
                imView[i] = new ImageViewer();
                imView[i].loadImage(imageSetList[index].GetImageAt(i));
                imView[i].ShowInTaskbar = false;
                //imView.
                imView[i].WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                imView[i].Top = currRow * (height - 200) / numRows;
                imView[i].Left = currCol * width / 2;
                if (currNumImages > 1)
                {
                    imView[i].Width = width / 2;
                }
                else
                {
                    //if we only have 1 image, we don't need 2 columns to display things, so use entire width:
                    imView[i].Width = width;
                }
                imView[i].Height = (height - 200) / numRows;

                imView[i].Show();
                imView[i].Topmost = true;
                //imView[i].Owner = this;
                //imView[i].Activate();

                if (currCol == 1)
                {
                    currCol = 0;
                    currRow++;
                }
                else
                {
                    currCol++;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (ImageViewer iv in imView)
            {
                iv.Close();
            }
            GreyBackgroundWindow.Close();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currQuestion == TotalNumQuestions - 1)
            {
                //we've reached the end, but we might have missed answering a question -- check:
                List<int> unansweredList = new List<int>();
                List<int> currUnansweredList = new List<int>();
                int currListFirstQuestionNumber = 1; //as far as the user is concerned, the first question is #1
                foreach (ImageSet imSet in imageSetList)
                {
                    currUnansweredList = imSet.UnansweredQuestionsList;
                    for (int i = 0; i < currUnansweredList.Count; i++)//j in currUnansweredList)
                    {
                        currUnansweredList[i] += currListFirstQuestionNumber; //translate from relative question number (for that set) to overall number
                    }
                    unansweredList.AddRange(currUnansweredList);
                    currListFirstQuestionNumber += imSet.NumQuestions;
                }

                if (unansweredList.Count != 0)
                {
                    //we've reached the end, but we missed a question! Prompt to fix:

                    //first niceify the message:
                    List<string> unansweredMessageList = unansweredList.ConvertAll<string>(x => x.ToString());
                    for (int i = 0; i < unansweredList.Count - 1; i++)
                    {
                        unansweredMessageList[i] += ", "; //in a list of 2+, add commas after all but the last one
                    }
                    //check if we have >1 item in our list, and tack the appropriate plural form of question(s) to the front of the first string:
                    unansweredMessageList[0] = (unansweredMessageList.Count > 1) ? "questions " + unansweredMessageList[0] : "question " + unansweredMessageList[0];

                    MessageBoxResult savePrompt = MessageBox.Show("You've reached the end of the survey!\nPlease go back and complete " + string.Concat(unansweredMessageList.ToArray()) + " before submitting.  Thanks!", "Unanswered Questions", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                else
                {
                    //this is our very last one! Prompt to submit survey:
                    MessageBoxResult savePrompt = MessageBox.Show("Thank you for completing this suvey!\nAre you sure you want to submit your results now?", "Submit survey?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (savePrompt == MessageBoxResult.OK)
                    {
                        serializeData();
                        this.Close();
                    }
                }
            }
            else
            {
                displayNextQuestion();
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            displayPreviousQuestion();
        }

        private void serializeData()
        {
            Stream st = new FileStream(@"C:\Survey_" + System.IO.Path.GetRandomFileName()+ ".xml", FileMode.Append);

            /*
            XmlSerializer personalSer = new XmlSerializer(typeof(PersonalInfo));
            XmlSerializer ser = new XmlSerializer(typeof(ImageSet));
            TextWriter writer = new StreamWriter(@"C:\test.xml");

            writer.WriteStartElement("Question");
            personalSer.Serialize(writer, startWindow.Resources["data"]);
            foreach (ImageSet ims in imageSetList)
            {
                ser.Serialize(writer, ims);
            }
            st.Close();
             */

            XmlSerializer ser = new XmlSerializer(typeof(SurveyInfoWrapper));
            //TextWriter writer = new StreamWriter(@"C:\test.xml");

            SurveyInfoWrapper wrapper = new SurveyInfoWrapper((PersonalInfo)startWindow.Resources["data"], imageSetList);
            ser.Serialize(st, wrapper); 

            st.Close();

        }


    }
    public class SurveyInfoWrapper:IXmlSerializable
    {
        private PersonalInfo person;
        private List<ImageSet> imageSets;

        public SurveyInfoWrapper()
        {
        }

        public SurveyInfoWrapper(PersonalInfo personInfo, List<ImageSet> imageSetList)
        {
            person = personInfo;
            imageSets = imageSetList;
        }
        public void WriteXml(XmlWriter writer)
        {
            person.WriteXml(writer);
            foreach (ImageSet ims in imageSets)
            {
                ims.WriteXml(writer);
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
    }
 }