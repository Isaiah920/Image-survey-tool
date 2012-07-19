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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Grid x;
            //x.Children.Add(


        }
        int currNumImages = 4;
        int currImageSet = 0;
        int currQuestion = 0;
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
            displaySet(0);
            displayNextQuestion();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void displayNextQuestion()
        {
            //if there are no more questions for our current image set, advance to the next:
            if (!imageSetList[currImageSet].HasNextQuestion)
            {
                displaySet(++currImageSet);
            }
            IQuestions x = imageSetList[currImageSet].GetNextQuestion();
            x.Display(QuestionGrid);
        }

        private void displaySet(int index)
        {
            currImageSet = index; //remember this, since we'll need to get questions from here later
            currNumImages = imageSetList[index].NumQuestions;

            //TODO: would be nice to have images maintain their position if they are moved around, 
            //rather than having them "snap back" upon changing image sets...
            imView = new ImageViewer[currNumImages];

            int width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            this.Left = width/2 - this.Width / 2;
            this.Top = (height - 200);
            
            //TODO: load in question instead of this
            //OneToNQuestion x = new OneToNQuestion(5, 4);
            //x.Display(QuestionGrid);

            int numRows = currNumImages / 2; //always use 2 columns
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
                imView[i].Width = width / 2;
                imView[i].Height = (height - 200) / numRows;
                imView[i].Show();
                imView[i].Activate();

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
        }

    }
}
