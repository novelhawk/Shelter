using System;
using System.IO;
using System.Security;
using System.Text;
using Mod.Logging;
using Newtonsoft.Json;
using UnityEngine;
using LogType = Mod.Logging.LogType;

namespace Mod.Managers
{
    public class ConfigManager<T>
    {
        private readonly string _file;
        private readonly JsonSerializerSettings _settings;
        
        public ConfigManager(string file)
        {
            _file = file;
            _settings = null;
        }

        public ConfigManager(string file, JsonSerializerSettings settings)
        {
            _file = file;
            _settings = settings;
        }
        
        public string Serialize(T obj)
        {
            return JsonConvert.SerializeObject(obj, typeof(T), Formatting.None, _settings);
        }

        public T Deserialize()
        {
            return Deserialize(ReadFile());
        }
        
        public T Deserialize(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, _settings);
            }
            catch (Exception e)
            {
                Debug.Log("Error deserializing");
                Debug.Log(json);
                Debug.LogException(e);
                return Deserialize(InvalidateCurrentFile());
            }
        }

        public string InvalidateCurrentFile()
        {
            if (File.Exists(Shelter.ModDirectory + _file))
                File.Delete(Shelter.ModDirectory + _file);
//                File.Move(Shelter.ModDirectory + _file, ProfileFile.Name + ".broken." + Shelter.Stopwatch.ElapsedMilliseconds % 100);
            return CreateDefaultFile();
        }

        public bool WriteFile(string json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            
            try
            {
                File.WriteAllText(Shelter.ModDirectory + _file, json, Encoding.UTF8);
                return true;
            }
            catch (IOException e)
            {
                Shelter.LogConsole("Error opening '{0}'. More details in Logs/{1}.", LogType.Error, _file, FileLogger.Latest);
                Shelter.Log("{0} occurred while opening {1}. Reason: {2}", LogType.Error, e.GetType().Name, _file, e.Message);
            }
            catch (SecurityException e)
            {
                Shelter.LogConsole("Cannot access file '{0}'. More details in Logs/{1}.", LogType.Error, _file, FileLogger.Latest);
                Shelter.Log("{0} occurred while opening {1}. Reason: {2}", LogType.Error, e.GetType().Name, _file, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Shelter.LogConsole("File '{0}' is marked as Read-Only. Cannot save changes.", LogType.Error, _file);
                Shelter.Log("{0} occurred while opening {1}. Reason: {2}", LogType.Error, e.GetType().Name, _file, e.Message);
            }
            return false;
        }

        public string ReadFile()
        {
            if (!File.Exists(Shelter.ModDirectory + _file))
                return CreateDefaultFile();

            try
            {
                return File.ReadAllText(Shelter.ModDirectory + _file, Encoding.UTF8);
            }
            catch (IOException e)
            {
                Shelter.LogConsole("Error opening '{0}'. More details in Logs/{1}.", LogType.Error, _file, FileLogger.Latest);
                Shelter.Log("{0} occurred while opening {1}. Reason: {2}", LogType.Error, e.GetType().Name, _file, e.Message);
                throw;
            }
            catch (Exception e)
            {
                Shelter.LogConsole("Cannot access file '{0}'. More details in Logs/{1}.", LogType.Error, _file, FileLogger.Latest);
                Shelter.Log("{0} occurred while opening {1}. Reason: {2}", LogType.Error, e.GetType().Name, _file, e.Message);
                throw;
            }
        }

        private string CreateDefaultFile()
        {
            using (var stream = Shelter.Assembly.GetManifestResourceStream($@"Mod.Resources.Config.{_file}"))
            {
                if (stream == null)
                {
                    Shelter.Log("Cannot create default '{0}'. Internal error.", LogType.Error, _file);
                    Application.Quit();
                    throw new Exception(); // Prevents ReSharper annotations
                }
                using (var ms = new MemoryStream())
                {
                    using (var fs = File.Open(Shelter.ModDirectory + _file, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        byte[] buffer = new byte[512];

                        int bytesRead;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, bytesRead);
                            fs.Write(buffer, 0, bytesRead);
                        }

                        fs.Flush();
                        ms.Flush();

                        byte[] content = ms.ToArray();
                        if (content.Length >= 3 && content[0] == 0xEF && content[1] == 0xBB && content[2] == 0xBF)
                            return Encoding.UTF8.GetString(content, 3, content.Length - 3);
                        return Encoding.UTF8.GetString(content);
                    }
                }
            }
        }
    }
}