using System;
using System.Collections.Generic;
using Mod.Managers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Mod.Animation;
using Mod.Discord;
using Mod.Interface;
using Mod.Logging;
using Mod.Modules;
using UnityEngine;
using AnimationInfo = Mod.Animation.AnimationInfo;
using Animator = Mod.Animation.Animator;
using Console = Mod.Interface.Console;
using LogType = Mod.Logging.LogType;
using Random = UnityEngine.Random;

namespace Mod
{
    public class Shelter : MonoBehaviour
    {
        public static readonly string ModDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Shelter\\";
        public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        public static readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        public static List<Profile> Profiles => _profileManager.ProfileFile.Profiles;
        public static Profile Profile => _profileManager.ProfileFile.SelectedProfile;
        public static readonly long StartTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

        private static FileLogger _logger;
        private static ConsoleLogger _consoleLogger;
        private static AnimationInfo _animation;
        private static InterfaceManager _interfaceManager;
        private static CommandManager _commandManager;
        private static InputManager _inputManager;
        private static EventManager _eventManager;
        private static AnimationManager _animationManager;
        private static ProfileManager _profileManager;

        public Shelter()
        {
            if (!Directory.Exists(ModDirectory))
                Directory.CreateDirectory(ModDirectory);

            _animation = new AnimationInfo(AnimationType.Cycle, AnimationColor.Rainbow);
        }
        
        public static void InitComponents()
        {
            _logger = new FileLogger();
            _consoleLogger = new ConsoleLogger();
            _eventManager = new EventManager();
            _interfaceManager = new InterfaceManager();
            _profileManager = new ProfileManager();
            _inputManager = new InputManager();
            _animationManager = new AnimationManager();
            ModuleManager.LoadModules();
            
            Log("Shelter mod initialized.");
        }

        public void Update()
        {
//            DiscordApi.RunCallbacks();
            if (Input.GetKeyDown(KeyCode.I) && Input.GetKey(KeyCode.LeftControl))
                File.WriteAllLines($"GameObjects{Random.Range(0, 255)}.txt", FindObjectsOfType(typeof(GameObject)).OrderBy(x => x.GetInstanceID()).Select(x => $"{x.GetInstanceID()} - {x.name}").ToArray());
        }

        private void OnDisable()
        {
//            DiscordApi.Shutdown();
        }

        public static void OnMainMenu()
        {
            _commandManager?.Dispose();
            _commandManager = null;
            _interfaceManager.DisableAll();
            _interfaceManager.Enable(nameof(Background));
            _interfaceManager.Enable(nameof(Loading));
            _interfaceManager.Enable(nameof(MainMenu));
            DiscordRpc.SendMainMenu();
        }
        
        
        public static void OnJoinedGame()
        {
            _commandManager = new CommandManager();
            _interfaceManager.DisableAll();
            _interfaceManager.Enable(nameof(Chat));
            if (ModuleManager.Enabled(nameof(ModuleShowScoreboard)))
                _interfaceManager.Enable(nameof(Scoreboard));
            if (ModuleManager.Enabled(nameof(ModuleShowGameInfo)))
                _interfaceManager.Enable(nameof(GameInfo));
            if (ModuleManager.Enabled(nameof(ModuleShowConsole)))
                _interfaceManager.Enable(nameof(Console));
//            if (PhotonNetwork.Room != null)
//                DiscordApi.UpdatePresence(new DiscordApi.RichPresence
//                {
//                    details = "In Lobby",
//                    state = PhotonNetwork.Room.Name.RemoveColors().MaxChars(42),
//                    joinSecret = PhotonNetwork.Room.FullName,
//                    partyId = PhotonNetwork.Room.FullName,
//                    partyMax = PhotonNetwork.Room.MaxPlayers,
//                    partySize = Room.CurrentPlayers,
//                    startTimestamp = StartTime,
//                    largeImageKey = "city",
//                });
        }

        public static bool TryFind(string name, out GameObject go)
        {
            if ((go = GameObject.Find(name)) != null)
                return true;
            return false;
        }

        #region Static methods

        [StringFormatMethod("line")]
        public static void Log(string line, params object[] args) => Log(string.Format(line, args));
        [StringFormatMethod("line")]
        public static void Log(string line, LogType logType, params object[] args) => Log(string.Format(line, args), logType);
        public static void Log(object obj, LogType logType = LogType.Info) => Log(obj as string, logType);
        public static void Log(string line, LogType logType = LogType.Info)
        {
            if (_logger != null && line != null)
            {
                var elapsed = Stopwatch.Elapsed;
                _logger.Log("{0:00}:{1:00}.{2:000} [{3}] {4}{5}", 
                    elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds,
                    logType.ToString().ToUpper(),
                    line,
                    Environment.NewLine);
            }
        }
        
        [StringFormatMethod("line")]
        public static void LogBoth(string line, LogType logType, params object[] args) => LogBoth(string.Format(line, args), logType);
        [StringFormatMethod("line")]
        public static void LogBoth(string line, params object[] args) => LogBoth(string.Format(line, args));
        public static void LogBoth(object obj, LogType logType = LogType.Info) => LogBoth(obj as string, logType);
        public static void LogBoth(string line, LogType logType = LogType.Info)
        {
            Log(line, logType);
            LogConsole(line, logType);
        }

        [StringFormatMethod("line")]
        public static void LogConsole(string line, LogType logType, params object[] args) => LogConsole(string.Format(line, args), logType);
        [StringFormatMethod("line")]
        public static void LogConsole(string line, params object[] args) => LogConsole(string.Format(line, args));
        public static void LogConsole(object obj, LogType logType = LogType.Info) => LogConsole(obj as string, logType);
        public static void LogConsole(string line, LogType logType = LogType.Info)
        {
            if (_consoleLogger != null && line != null)
            {
                if (logType == LogType.Info)
                    _consoleLogger.Log("<color=#00D7FF>{0}</color>", line);
                else
                    _consoleLogger.Log("<color=#00D7FF>{0}</color> <i><color=#FF3600>{1}</color></i>",
                        line,
                        logType.ToString().ToUpper());
            }
        }

        public static Texture2D GetImage(string image)
        {
            if (Assembly.GetManifestResourceInfo($@"Mod.Resources.{image}.png") == null) return null;
            using (var stream = Assembly.GetManifestResourceStream($@"Mod.Resources.{image}.png"))
            {
                if (stream == null)
                {
                    Log("Cannot read '{0}.png' image. Internal error.", LogType.Error, image);
                    Application.Quit();
                    throw new Exception(); // Prevents ReSharper annotations
                }
                
                byte[] buffer = new byte[8 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        ms.Write(buffer, 0, read);
                    
                    var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                    texture.LoadImage(ms.ToArray());
                    texture.Apply();
                    return texture;
                }
            }
        }

        #endregion

        public static AnimationInfo Animation => _animation;

        public static ConsoleLogger ConsoleLogger => _consoleLogger;
        public static EventManager EventManager => _eventManager;
        public static ProfileManager ProfileManager => _profileManager;
        public static InputManager InputManager => _inputManager;
        public static AnimationManager AnimationManager => _animationManager;
        public static InterfaceManager InterfaceManager => _interfaceManager;
        public static CommandManager CommandManager => _commandManager;
    }
}
