using System.Diagnostics;
using System.IO;

namespace StartSurvey
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\SurveyTool.exe";
            proc.Arguments = "admin";
            Process.Start(proc);
        }
    }
}
