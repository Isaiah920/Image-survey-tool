using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SurveyTool
{
    /// <summary>
    /// Interaction logic for AnswerLoader.xaml
    /// </summary>
    public partial class AnswerLoader : System.Windows.Window
    {
        List<List<IQuestions>> questions = new List<List<IQuestions>>();
        List<PersonalInfo> people = new List<PersonalInfo>();

        public AnswerLoader()
        {
            InitializeComponent(); 
        }

        public void loadQuestions(string file)
        {
            List<IQuestions> tempList = new List<IQuestions>();
            PersonalInfo person = new PersonalInfo();
            try
            {
                using (XmlReader reader = XmlReader.Create(file))
                {
                    reader.MoveToContent();
                    while (reader.Read())
                    {
                        //TODO: this (2nd part) is kind of a hack to check if it's empty, or something like "\n ". Not really a good way of doing this...
                        if (reader.NodeType == XmlNodeType.Element && reader.Name.Length > 2)
                        {
                            XElement el = XNode.ReadFrom(reader) as XElement;
                            if (el != null)
                            {
                                switch (el.Name.ToString())
                                {
                                    case "Name":
                                        person.Name = el.Value.ToString();
                                        break;
                                    case "Dept":
                                        person.Dept = el.Value.ToString();
                                        break;
                                    case "OtherInfo":
                                        person.OtherInfo = el.Value.ToString();
                                        break;
                                    case "Age":
                                        person.Age = int.Parse(el.Value.ToString());
                                        break;
                                    case "Gender":
                                        person.Gender = (GenderEnum)int.Parse(el.Value.ToString());
                                        break;
                                    case "Experience":
                                        person.Experience = (ExperienceEnum)int.Parse(el.Value.ToString());
                                        break;
                                    case "VisionProblems":
                                        person.VisionProblems = (VisionProblemsEnum)int.Parse(el.Value.ToString());
                                        break;
                                    case "VisionProblemsDescription":
                                        person.VisionProblemsDescription = el.Value.ToString();
                                        break;
                                    case "Question":
                                        IQuestions question = (IQuestions)Activator.CreateInstance(null, "SurveyTool." + el.FirstAttribute.Value.ToString()).Unwrap();//.Unwrap();//.GetType();
                                        var serializer = new XmlSerializer(question.GetType());
                                        question.ReadXml(el.CreateReader());
                                        tempList.Add(question);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                if (tempList.Count > 0) questions.Add(tempList);
                tempList = new List<IQuestions>();
            }
            catch (Exception ex)
            {
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            loadQuestions(@"C:\test.xml");

           // try
            //{
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Add(Type.Missing);
                Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
            
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet) workbook.Sheets[1];
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;
                app.Visible = true;

                string[,] currTable;
                int currRow = 0;

                for (int i = 0; i < questions[0].Count; i++)
                {
                    IQuestions[] currQn = new IQuestions[questions.Count];
                    for (int j = 0; j < questions.Count; j++)
                    {
                        currQn[j] = questions[j][i]; //todo: check!
                    }
                    currQn[0].CondenseResultsToTable(currQn as IEnumerable<IQuestions>, i, out currTable);
                    writeToExcel(currTable, currRow, worksheet);
                    writeToExcel("" + (i + 1), currRow, worksheet); //question number
                    currRow += currTable.GetLength(0)+1;
                }
            //}
          ///  catch (Exception ex)
          //  {
           // }

        }
        private void writeToExcel(string curr, int currRow, Microsoft.Office.Interop.Excel.Worksheet worksheet)
        {
            worksheet.Cells[currRow + 1, 1] = curr;
        }

        private void writeToExcel(string[,] currTable, int currRow, Microsoft.Office.Interop.Excel.Worksheet worksheet)
        {
            for (int i = 1; i < currTable.GetLength(0)+1; i++)
            {
                for (int j = 1; j < currTable.GetLength(1)+1; j++) 
                {
                    worksheet.Cells[currRow + i,j] = currTable[i-1,j-1];
                }
            }
        }
    }
}
