using System.IO;

namespace AitLab7BeesAlg.Models
{
    public class SimpleFileLogger
    {
        private readonly string _filePath;

        public SimpleFileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void LogToFile(string data)
        {
            using var s = new FileStream(_filePath, FileMode.Append);
            using var sw = new StreamWriter(s);
            sw.WriteLine(data);
        }

        public void LogToFileTruncate(string data)
        {
            if (!File.Exists(_filePath))
            {
                using var s = new FileStream(_filePath, FileMode.OpenOrCreate);
                using var sw = new StreamWriter(s);
                sw.WriteLine(data);
            }
            else
            {
                using var s = new FileStream(_filePath, FileMode.Truncate);
                using var sw = new StreamWriter(s);
                sw.WriteLine(data);
            }
        }
    }
}