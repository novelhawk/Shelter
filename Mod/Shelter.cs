using System;
using System.Collections.Generic;
using Mod.Managers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mod.Animation;
using Mod.Discord;
using Mod.Interface;
using Mod.Modules;
using UnityEngine;
using AnimationInfo = Mod.Animation.AnimationInfo;
using Animator = Mod.Animation.Animator;
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
        private static AnimationInfo _animation;
        private static InterfaceManager _interfaceManager;
        private static CommandManager _commandManager;
        private static AnimationManager _animationManager;
        private static ModuleManager _moduleManager;
        private static ProfileManager _profileManager;

        public Shelter()
        {
            if (!Directory.Exists(ModDirectory))
                Directory.CreateDirectory(ModDirectory);

            _animation = new AnimationInfo(AnimationType.Cycle, AnimationInfo.Rainbow);
        }
        
        public void InitComponents()
        {
            _interfaceManager = new InterfaceManager();
            _profileManager = new ProfileManager();
            _animationManager = new AnimationManager();
            _moduleManager = new ModuleManager();
        }

        public void Update()
        {
            DiscordApi.RunCallbacks();
            if (Input.GetKeyDown(KeyCode.I) && Input.GetKey(KeyCode.LeftControl))
                File.WriteAllLines($"GameObjects{Random.Range(0, 255)}.txt", FindObjectsOfType(typeof(GameObject)).OrderBy(x => x.GetInstanceID()).Select(x => $"{x.GetInstanceID()} - {x.name}").ToArray());
        }

        private void OnDisable()
        {
            DiscordApi.Shutdown();
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
            _interfaceManager.Enable(nameof(Scoreboard));
            if (_moduleManager.Enabled(nameof(ModuleShowGameInfo)))
                _interfaceManager.Enable(nameof(GameInfo));
            DiscordApi.UpdatePresence(new DiscordApi.RichPresence
            {
                details = "In Lobby",
                state = PhotonNetwork.Room.Name.RemoveColors().MaxChars(42),
                joinSecret = PhotonNetwork.Room.FullName,
                partyId = PhotonNetwork.Room.FullName,
                partyMax = PhotonNetwork.Room.MaxPlayers,
                partySize = Room.CurrentPlayers,
                startTimestamp = StartTime,
                largeImageKey = "city",
            });
        }

        public static bool TryFind(string name, out GameObject go)
        {
            if ((go = GameObject.Find(name)) != null)
                return true;
            return false;
        }

        #region Static methods

        public static void Log(params object[] messages)
        {
            foreach (var obj in messages)
                Log(obj);
        }

        public static void Log(object msg)
        {

        }

        public static Texture2D GetImage(string image)
        {
            if (Assembly.GetManifestResourceInfo($@"Mod.Resources.{image}.png") == null) return null;
            using (var stream = Assembly.GetManifestResourceStream($@"Mod.Resources.{image}.png"))
            {
                var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                texture.LoadImage(stream.ToBytes());
                texture.Apply();
                return texture;
            }
        }

        #endregion

        public static AnimationInfo Animation => _animation;

        public static ModuleManager ModuleManager => _moduleManager;
        public static ProfileManager ProfileManager => _profileManager;
        public static AnimationManager AnimationManager => _animationManager;
        public static InterfaceManager InterfaceManager => _interfaceManager;
        public static CommandManager CommandManager => _commandManager;
    }
}
