using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS
{
    public class Logger
    {
        public static string LogFile { get; set; } = "Log.txt";
        public static void Log(string message)
        {
            using (var writer = File.AppendText(LogFile))
            {
                writer.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(), message));
                writer.Flush();
                writer.Close();
            }
        }
    }
}
