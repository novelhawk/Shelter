using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Newtonsoft.Json;

namespace Mod.Managers
{
    public class ProfileManager
    {
        private ProfileFile _profileFile;

        public ProfileManager()
        {
            Load();
        }

        private void Load()
        {
            string json = ReadFile();
            Deserialize(json);
        }

        public bool Save()
        {
            return WriteFile(Serialize());
        }
        
        private string Serialize()
        {
            return JsonConvert.SerializeObject(_profileFile, typeof(ProfileFile), Formatting.None, new JsonSerializerSettings());
        }

        private void Deserialize(string json)
        {
            try
            {
                _profileFile = JsonConvert.DeserializeObject<ProfileFile>(json);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                Deserialize(InvalidateCurrentFile());
                return;
            }

            if (_profileFile.Profiles.Count < 1)
            {
                Deserialize(InvalidateCurrentFile());
                return;
            }

            if (_profileFile.Selected > _profileFile.Profiles.Count - 1)
                _profileFile.Selected = 0;
        }

        private static string InvalidateCurrentFile()
        {
            if (File.Exists(ProfileFile.Location))
                File.Move(ProfileFile.Location, ProfileFile.Location + ".broken." + Shelter.Stopwatch.ElapsedMilliseconds % 10000);
            return CreateDefaultFile();
        }

        private static bool WriteFile(string json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            
            try
            {
                File.WriteAllText(ProfileFile.Location, json, Encoding.UTF8);
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

        private static string ReadFile()
        {
            if (!File.Exists(ProfileFile.Location))
                return CreateDefaultFile();

            try
            {
                return File.ReadAllText(ProfileFile.Location, Encoding.UTF8);
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

        private static string CreateDefaultFile()
        {
            using (var stream = Shelter.Assembly.GetManifestResourceStream(@"Mod.Resources.profiles.json"))
            {
                if (stream == null)
                    throw new NullReferenceException(); //TODO
                using (var ms = new MemoryStream())
                {
                    using (var fs = new FileStream(ProfileFile.Location, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        byte[] buffer = new byte[1024];

                        int bytesRead;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, bytesRead);
                            fs.Write(buffer, 0, bytesRead);
                        }

                        fs.Flush();
                        fs.Close();

                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }

        public ProfileFile ProfileFile => _profileFile;
    }
}