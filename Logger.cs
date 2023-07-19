using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Append, FileAccess.Write))
            using (sr = new StreamWriter(fs))
            {
                // Add some time and message to file
                string time = DateTime.Now.ToLongTimeString();
                time = "Date Time: " + time;
                string message = "Message: " + ex.Message.ToString();
                sr.WriteLine(time);
                sr.WriteLine(message);
                sr.WriteLine();
                sr.Close();
                fs.Close();
            }
            write its details associated with datetime 

        }
    }
}
