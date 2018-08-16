using System;
using System.IO;
using System.Security;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Mod.Managers
{
    public class ConfigManager<T>
    {
        private readonly string _file;
        
        public ConfigManager(string file)
        {
            _file = file;
        }
        
        public string Serialize(T obj)
        {
            return JsonConvert.SerializeObject(obj, typeof(T), Formatting.None, new JsonSerializerSettings());
        }

        public T Deserialize()
        {
            return Deserialize(ReadFile());
        }
        
        public T Deserialize(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
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
            Debug.Log(Shelter.ModDirectory + _file);
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
            catch (IOException)
            {
                // log
                throw;
            }
            catch (SecurityException)
            {
                // log
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                // log
                throw;
            }
        }

        public string ReadFile()
        {
            if (!File.Exists(Shelter.ModDirectory + _file))
                return CreateDefaultFile();

            try
            {
                return File.ReadAllText(Shelter.ModDirectory + _file, Encoding.UTF8);
            }
            catch (IOException)
            {
                // log
                throw;
            }
            catch (SecurityException)
            {
                // log
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                // log
                throw;
            }
        }

        private string CreateDefaultFile()
        {
            using (var stream = Shelter.Assembly.GetManifestResourceStream($@"Mod.Resources.Config.{_file}"))
            {
                if (stream == null)
                    throw new NullReferenceException(); //TODO
                using (var ms = new MemoryStream())
                {
                    using (var fs = new FileStream(Shelter.ModDirectory + _file, FileMode.Create, FileAccess.Write, FileShare.Read))
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

                        return ReadFile();
//                        return Encoding.UTF8.GetString(ms.ToArray()); TODO: Make single step
                    }
                }
            }
        }
    }
}