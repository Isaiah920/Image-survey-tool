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

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace SurveyTool
{
    /// <summary>
    /// Interaction logic for AnswerLoader.xaml
    /// </summary>
    public partial class AnswerLoader : Window
    {
        List<List<IQuestions>> questions = new List<List<IQuestions>>();//List<<IQuestions>();
        List<PersonalInfo> people = new List<PersonalInfo>();

        public AnswerLoader()
        {
            InitializeComponent();
            
        }

        public void loadQuestions(string file)
        {
            //Stream st = new FileStream(file, FileMode.Open);//File.OpenWrite(@"C:\test.txt");
            //.
            //treamWriter sw = new StreamWriter(@"C:\test.txt");
            //MemoryStream ms = new MemoryStream();
            //BinaryFormatter bf = new BinaryFormatter();
            //XmlSerializer personalSer = new XmlSerializer(typeof(PersonalInfo));
            
            //XmlSerializer ser = new XmlSerializer(typeof(ImageSet));
            //TextWriter writer = new StreamWriter(@"C:\test.xml");
            //XmlReader reader = XmlReader.Create(st);//(typeof(PersonalInfo));
            //try
            //{
                using (XmlReader reader = XmlReader.Create(file))
                {
                    List<IQuestions> tempList = new List<IQuestions>();
                    reader.MoveToContent();
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "Question")
                            {
                                XElement el = XNode.ReadFrom(reader) as XElement;
                                if (el != null)
                                {
                                    IQuestions question = (IQuestions) Activator.CreateInstance(null, "SurveyTool." + el.FirstAttribute.Value.ToString()).Unwrap();//.Unwrap();//.GetType();
                                    var serializer = new XmlSerializer(question.GetType()); //IQuestions)); //TODO: is this actually right, or just IQuestions?
                                    question.ReadXml(el.CreateReader());
                                    tempList.Add(question);
                                    //tempList.Add((IQuestions)serializer.Deserialize(el.CreateReader()));//serializer.Deserialize(el);//of(type)));
                                    //yield return el;
                                }
                            }
                        }
                        if (tempList.Count > 0) questions.Add(tempList);
                    }

                    /*  //We don't really care for now... ImageSets aren't important for this, so might as well leave that info out
                    reader.MoveToContent();
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "ImageSet")
                            {
                                XElement el = XNode.ReadFrom(reader) as XElement;
                                if (el != null)
                                {
                                    //yield return el;
                                }
                            }
                        }
                    }*/
                }

                
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadQuestions(@"C:\test.xml");
        }
    }
}
