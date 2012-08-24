using System;
using System.Windows;

namespace SurveyTool
{
    public partial class App : Application
    {
        //Custom main method
        [STAThread]
        public static void Main()
        {
            App app = new App();

            //set to appropriate entry point:
            app.StartupUri = new Uri("StartWindow.xaml", UriKind.Relative);

            foreach (String arg in Environment.GetCommandLineArgs())
            {
                if (arg == "admin") //we started this program from the admin project
                {
                    app.StartupUri = new Uri("AnswerLoader.xaml", UriKind.Relative);
                }
            }
            
            // Code to register events and set properties that were
            // defined in XAML in the application definition
            app.InitializeComponent();

            // Start running the application
            app.Run();
        }
    }
}
