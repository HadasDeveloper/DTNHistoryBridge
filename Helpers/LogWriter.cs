using System;
using System.IO;
using System.Text;

namespace DTNHistoryBridge.Helpers
{
    public static class LogWriter
    {
        public static void Write(UnhandledExceptionEventArgs e, string path, string fileName, bool append)
        {
            Exception ex = (Exception)e.ExceptionObject;

            StringBuilder b = new StringBuilder();

            b.Append("\r\n");
            b.Append(DateTime.Now + " " + ex.Message);
            b.Append("\r\n");
            b.Append("_________________");
            b.Append("\r\n");
            b.Append(ex.StackTrace);
            b.Append("\r\n");
            if (ex.InnerException != null)
            {
                b.Append("\r\n");
                b.Append("\r\n");
                b.Append("Inner exception:");
                b.Append("\r\n");
                b.Append(ex.InnerException.Message);
                b.Append("\r\n");
                b.Append("_______________________");
                b.Append("\r\n");
                b.Append(ex.InnerException.StackTrace);
                b.Append("\r\n");
            }
            b.Append("_________________");
            b.Append("\r\n");
            b.Append("\r\n");
            b.Append("_________________");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (StreamWriter w = new StreamWriter(path + fileName, append))
            {
                w.Write(b.ToString());
            }
        }
    }
}
