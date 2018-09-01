using System;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Text;
using Mono.Security.X509.Extensions;
using UnityEngine;

namespace Mod.Logger
{
    public class FileLogger
    {
        private const string Latest = "latest.log";
        private const string Old = "{0}_{1}_{2} {3}_{4}.log";
        
        private readonly string _path = Application.dataPath + "/Logs/";
        private FileStream _fileStream;
        
        public FileLogger()
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

//            if (File.Exists(_path + Latest))
                
            
            SafeOpenFileStream(_path + Latest);
            Application.LogCallback = 
        }
        
        
        

//        private void MoveOldLogs()
//        {
//            string line;
//            using (var fs = File.Open(_path + Latest, FileMode.Open, FileAccess.Read, FileShare.Read))
//            {
//                using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
//                {
//                    reader.ReadLine();
//                    line = reader.ReadLine();
//                }
//            }
//            
//            line
//        }

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
            
            Log("# Shelter Mod Logs");
            DateTime now = DateTime.Now;
            Log("# Created {0}/{1}/{2} at {3}:{4}:{5}", 
                now.Day, now.Month, now.Year, 
                now.Hour, now.Minute, now.Second);
        }

        public void Log(object obj) => Log(obj as string);
        public void Log(string line, params object[] args) => Log(string.Format(line, args));
        public void Log(string line)
        {
            if (_fileStream == null || line == null)
                return;
            
            var bytes = Encoding.UTF8.GetBytes(line);
            _fileStream.Write(bytes, 0, bytes.Length);
        }
    }
}