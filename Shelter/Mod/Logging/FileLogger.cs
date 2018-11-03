using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Mod.Logging
{
    public class FileLogger : ILogger
    {
        public const string Latest = "latest.log";
        private const string FormerLogFileName = "{0:00}-{1:00}-{2} {3:00}-{4:00}-{5:00}.log";
        
        private readonly string _path = Application.dataPath + "/Logs/";
        private FileStream _fileStream;
        
        public FileLogger()
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            if (File.Exists(_path + Latest))
                DisposeFormerLog();
                
            SafeOpenFileStream(_path + Latest);
        }
        
        private void DisposeFormerLog()
        {
            var info = new FileInfo(_path + Latest);

            var fileName = DateFormat(FormerLogFileName, info.CreationTime);
            File.Move(_path + Latest, _path + fileName);
        }

        private static string DateFormat(string line, DateTime time)
        {
            return string.Format(line, 
                time.Day, time.Month, time.Year, 
                time.Hour, time.Minute, time.Second);
        }

        private void SafeOpenFileStream(string path)
        {
            try
            {
                _fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't create the FileLogger. Client will now close.");
                Debug.LogException(e);
                Application.Quit();
            }
            
            Log($"# Shelter Logs{Environment.NewLine}");
            Log("{0}{1}{1}", DateFormat("# Created {0:00}/{1:00}/{2} at {3:00}:{4:00}:{5:00}", DateTime.Now), Environment.NewLine);
        }

        public void Log(string line, params object[] args) => Log(string.Format(line, args));
        public void Log(string line) // Not thread-safe. Use lock if we start to use threads.
        {
            if (_fileStream == null)
                return;

            var bytes = Encoding.UTF8.GetBytes(line);
            _fileStream.Write(bytes, 0, bytes.Length);
            _fileStream.Flush();
        }
    }
}