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
using Microsoft.Win32;
using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace SurveyTool
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : System.Windows.Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void FileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".xls";
            dialog.Filter = "Excel files (.xls)|*.xls"; //TODO: xlsx?!

            string file = "";
            if (dialog.ShowDialog() == true) //this is a nullable bool, hence the ==
            {
                file = dialog.FileName;


                //long path since conflicts with some wpf Application thing
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                Workbook wb = excel.Workbooks.Open(file, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                Worksheet surveyWorksheet = (Worksheet)wb.Worksheets.get_Item(1);
                Worksheet photoWorksheet = (Worksheet)wb.Worksheets.get_Item(2);


                int numImageSets = int.Parse(surveyWorksheet.get_Range("G7", "G7").Value.ToString());
                int numQuestions = int.Parse(surveyWorksheet.get_Range("G4", "G4").Value.ToString());
                //int numQuestions = (int)surveyWorksheet.get_Range("G4", "G4").Value;

                List<ImageSet> imageSets = new List<ImageSet>(numImageSets);
                for (int i=0; i<numImageSets; i++)
                {
                    imageSets.Add(new ImageSet());
                    string rootPath = photoWorksheet.get_Range("B" + (4 + i), "B" + (4 + i)).Value.ToString();

                    char currCol = 'C';
                    string currPhoto = "";
                    int j=0;
                    try
                    {
                        //get each photo -- there will be between 1 and 4, in excel columns C-F.  An empty string means there are no more
                        //photos in this set
                        currPhoto = photoWorksheet.get_Range(((char)(currCol + j)) + "" + (4 + i), ((char)(currCol + j)) + "" + (4 + i)).Value.ToString();
                        if (currPhoto != "")
                        {
                            imageSets[i].AddPicture(rootPath + currPhoto);
                        }
                        j++;
                    }
                    catch (NullReferenceException ex)
                    {
                        //this happens when we read ""

                    }

                    string imageName = photoWorksheet.get_Range("B" + (4 + i), "B" + (4 + i)).Value.ToString();
                }

                for (int i = 0; i < numQuestions; i++)
                {
                    int imageGroupNum = int.Parse(surveyWorksheet.get_Range("A" + (4 + i), "A" + (4 + i)).Value.ToString());
                    string qTypeString = surveyWorksheet.get_Range("D" + (4 + i), "D" + (4 + i)).Value.ToString();

                    try
                    {
                        IQuestions currQuestion = (IQuestions)Activator.CreateInstance(null, "SurveyTool." + qTypeString).Unwrap();
                        if (currQuestion is OneToNQuestion)
                        {
                            OneToNQuestion tempQuestion = (OneToNQuestion)currQuestion;
                            string[] labels = surveyWorksheet.get_Range("D" + (4 + i), "D" + (4 + i)).Value.ToString().Split(';');
                            tempQuestion.SetChoiceLabels(labels);

                        }
                    }
                    catch (TypeLoadException ex)
                    {
                    }
                }

                SurveyTextBox.Text = file;
                SurveySelectTextBlock.Text = "The survey has been loaded successfully.";

            }

            
        }
    }
}
