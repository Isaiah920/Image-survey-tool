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
//using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;

namespace SurveyTool
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : System.Windows.Window
    {
        int numQuestions = 0;
        int numImageSets = 0;
        bool validSurveyLoaded = false;
        SurveySettings settings;

        public StartWindow()
        {
            InitializeComponent();

            try
            {
                XmlSerializer serializerObj = new XmlSerializer(typeof(SurveySettings));
                FileStream ReadFileStream = new FileStream(@"config.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                settings = (SurveySettings)serializerObj.Deserialize(ReadFileStream);
                ReadFileStream.Close();

                //if we're being told to use relative paths, we must fix the absolute paths first, since that's what
                //we'll be using:
                if (settings.fileNameRelative)
                {
                    Uri programUri = new Uri(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath));
                    Uri uri = new Uri(programUri, settings.fileName);
                    settings.fullFileName = uri.LocalPath.ToString();
                }
                if (settings.folderNameRelative)
                {
                    Uri programUri = new Uri(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath));
                    Uri uri = new Uri(programUri, settings.folderName);
                    settings.fullFolderName = uri.LocalPath.ToString();
                }

                //SurveySelectTextBlock.t
                //survey.Text = settings.fileName;
                loadExcel(settings.fullFileName);
            }
            catch (Exception e)
            {
                //invalid/no config file
            }

            //initialize command bindings:
            //CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed, Save_CanExecute));
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //System.Windows.
            System.Windows.Window newWindow = new System.Windows.Window();
            newWindow.WindowStyle = System.Windows.WindowStyle.None;
            newWindow.WindowState = System.Windows.WindowState.Maximized;
            newWindow.Background = Brushes.Gray;
            newWindow.Show();
            newWindow.Focusable = false;
            //newWindow.IsActive = false;
            //newWindow.c

            MainWindow win = new MainWindow(this, settings);
            //newWindow.
            win.InitializeImageList(imageSets);
            win.TotalNumQuestions = numQuestions;
            win.GreyBackgroundWindow = newWindow;
            win.StartDisplaying();
            //win.InitializeList(x);
            win.Show();
            win.Topmost = true;
            win.Activate();
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PersonalInfo.DataIsValid && validSurveyLoaded;
            e.Handled = true;
        }

        //TODO: must be public to pass it along to other stuff later, since reference object... probably not a good thing
        public List<ImageSet> imageSets;

       /* private void FileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".xls";
            dialog.Filter = "Excel files (.xls)|*.xls"; //TODO: xlsx?!

            string file = "";
            if (dialog.ShowDialog() == true) //this is a nullable bool, hence the ==
            {
        */
        void loadExcel(string file)
        {
           // file = dialog.FileName;


            //long path since conflicts with some wpf Application thing
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = excel.Workbooks.Open(file, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            Worksheet surveyWorksheet = (Worksheet)wb.Worksheets.get_Item(1);
            Worksheet photoWorksheet = (Worksheet)wb.Worksheets.get_Item(2);

            numImageSets = int.Parse(surveyWorksheet.get_Range("G7", "G7").Value.ToString());
            numQuestions = int.Parse(surveyWorksheet.get_Range("G4", "G4").Value.ToString());
            //int numQuestions = (int)surveyWorksheet.get_Range("G4", "G4").Value;

            imageSets = new List<ImageSet>(numImageSets);
            for (int i=0; i<numImageSets; i++)
            {
                imageSets.Add(new ImageSet());
                string rootPath = photoWorksheet.get_Range("B" + (4 + i), "B" + (4 + i)).Value.ToString();

                char currCol = 'C';
                string currPhoto = "";
                int j=0;
                try
                {
                    do
                    {
                        //get each photo -- there will be between 1 and 4, in excel columns C-F.  An empty string means there are no more
                        //photos in this set
                        Range curr = photoWorksheet.get_Range(((char)(currCol + j)) + "" + (4 + i), ((char)(currCol + j)) + "" + (4 + i));
                        currPhoto = (curr.Value == null)? "":curr.Value.ToString();
                        if (currPhoto != "")
                        {
                            imageSets[i].AddPicture(System.IO.Path.GetDirectoryName(settings.fullFileName) + "//" + rootPath + currPhoto);//
                        }
                        j++;
                    }
                    while (currPhoto != "");
                }
                catch (NullReferenceException ex)
                {
                    //this happens when we read "", since nothing to get the .Value of...

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
                    currQuestion.SetQuestionString(surveyWorksheet.get_Range("C" + (4 + i), "C" + (4 + i)).Value.ToString());
                    currQuestion.SetNumImages(imageSets[int.Parse(surveyWorksheet.get_Range("A" + (4 + i), "A" + (4 + i)).Value.ToString())-1].NumImages);
                    if (currQuestion is OneToNQuestion)
                    {
                        OneToNQuestion tempQuestion = (OneToNQuestion)currQuestion;
                        string[] labels = surveyWorksheet.get_Range("E" + (4 + i), "E" + (4 + i)).Value.ToString().Split(';'); //TODO: check for exception here
                        tempQuestion.SetChoiceLabels(labels);
                    }
                    imageSets[imageGroupNum-1].AddQuestion(currQuestion); //since the Excel sheet starts at 1 rather than 0...
                }
                catch (TypeLoadException ex)
                {
                }
            }

            //SurveyTextBox.Text = file;
            SurveySelectTextBlock.Text = "The survey has been loaded successfully from " + settings.fileName;
            SavedToBlock.Text = "The results from this survey will be saved to " + settings.folderName;
            validSurveyLoaded = true; //now we can click the Start Survey button
        }
            
        //}

        private void VisionProblemsNoRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            VisionProblemsTextbox.IsEnabled = false;
        }

        private void VisionProblemsYesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            VisionProblemsTextbox.IsEnabled = true;
        }

        private void FileBrowseButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
