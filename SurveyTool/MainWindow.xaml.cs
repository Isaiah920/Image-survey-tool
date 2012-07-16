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
        ImageViewer[] imView;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            imView = new ImageViewer[currNumImages];

            int width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            this.Left = width/2 - this.Width / 2;
            this.Top = (height - 200);
            OneToNQuestion x = new OneToNQuestion(5, 4);
            x.Display(QuestionGrid);

            int numRows = currNumImages / 2; //always use 2 columns
            int currRow = 0;
            int currCol = 0;

            
            for (int i = 0; i < currNumImages; i++)
            {
                imView[i] = new ImageViewer();
                imView[i].ShowInTaskbar = false;
                //imView.
                imView[i].WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                imView[i].Top = currRow * (height - 200) / numRows;
                imView[i].Left = currCol * width / 2;
                imView[i].Width = width / 2;
                imView[i].Height = (height - 200) / numRows;
                imView[i].Show();

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
