using System;
using System.Collections.Generic;
using Game;
using ExitGames.Client.Photon;
using Photon;
using LogType = Mod.Logging.LogType;

namespace Mod
{
    public class Room : IComparable
    {
        public static List<Room> List;

        private Hashtable _properties;
        private string _fullName;
        
        private readonly string _roomName;
        private readonly LevelInfo _roomMap;
        private readonly Difficulty _roomDifficulty;
        private readonly DayLight _roomDayLight;
        private readonly string _roomPassword;
        private readonly int _roomTime;
        private readonly int _roomUUID;

        private bool _removedFromList;
        private int _maxPlayers;
        private int _currentPlayers;
        private bool _isVisible;
        private bool _isOpen;
        private bool _doAutoCleanup;
        private int _playerTTL;
        private int _roomTTL;

        private Room(
            string fullName,
            string name,
            LevelInfo map,
            Difficulty difficulty, 
            int time, 
            DayLight daylight, 
            string password,
            int uuid)
        {
            _fullName = fullName;
            _roomName = name;
            _roomMap = map;
            _roomDifficulty = difficulty;
            _roomTime = time;
            _roomDayLight = daylight;
            _roomPassword = password;
            _roomUUID = uuid;
        }
        
        public Room(string fullName, Hashtable properties)
        {
            _fullName = fullName;
            
            var split = fullName.Split('`');
            if (split.Length < 7)
                throw new Exception("Room..ctor called with invalid arguments: " + nameof(fullName)); //TODO: LOG!

            _roomName = split[0].MaxChars(100);
            if (!LevelInfoManager.TryGet(split[1], out _roomMap))
                Shelter.LogBoth("Couldn't parse Room({0}) map.", LogType.Warning, _roomName);
            _roomDifficulty = DifficultyToEnum(split[2]);
            if (!int.TryParse(split[3], out _roomTime))
                Shelter.LogBoth("Couldn't parse Room({0}) time.", LogType.Warning, _roomName);
            _roomDayLight = DayLightToEnum(split[4]);
            _roomPassword = split[5];
            if (!int.TryParse(split[6], out _roomUUID))
                Shelter.LogBoth("Couldn't parse Room({0}) uuid.", LogType.Warning, _roomName);

            LoadOptions(new RoomOptions(properties));
        }

        public Room(string fullName, RoomOptions options)
        {
            _fullName = fullName;

            var split = fullName.Split('`');
            if (split.Length < 7)
                throw new Exception(); //TODO: LOG!

            _roomName = split[0];
            if (!LevelInfoManager.TryGet(split[1], out _roomMap))
                Shelter.LogBoth("Couldn't parse Room({0}) map.", LogType.Warning, _roomName);
            _roomDifficulty = DifficultyToEnum(split[2]);
            if (!int.TryParse(split[3], out _roomTime))
                Shelter.LogBoth("Couldn't parse Room({0}) time.", LogType.Warning, _roomName);
            _roomDayLight = DayLightToEnum(split[4]);
            _roomPassword = split[5];
            if (!int.TryParse(split[6], out _roomUUID))
                Shelter.LogBoth("Couldn't parse Room({0}) uuid.", LogType.Warning, _roomName);

            LoadOptions(options);
        }

        private RoomOptions Options { set => LoadOptions(value); }

        public void LoadOptions(RoomOptions options)
        {
            if (options == null)
                return;
            
            if (options.RemovedFromList != null)
                _removedFromList = options.RemovedFromList.Value;
            if (options.MaxPlayers != null) 
                _maxPlayers = options.MaxPlayers.Value;
            if (options.CurrentPlayers != null) 
                _currentPlayers = options.CurrentPlayers.Value;
            if (options.IsVisible != null) 
                _isVisible = options.IsVisible.Value;
            if (options.IsOpen != null) 
                _isOpen = options.IsOpen.Value;
            if (options.DoAutoCleanup != null) 
                _doAutoCleanup = options.DoAutoCleanup.Value;

            _properties = options;
        }

        public static bool TryParse(string fullName, Hashtable hashtable, out Room room) => TryParse(fullName, new RoomOptions(hashtable), out room);
        public static bool TryParse(string fullName, RoomOptions properties, out Room room)
        {
            room = null;
            
            var split = fullName.Split('`');
            if (split.Length != 7)
            {
                Shelter.Log("Room '{0}' cannot be parsed into a Room object: Parameters mismatch ({1} expected 7)", LogType.Error, fullName, split.Length);
                return false;
            }

            if (!LevelInfoManager.TryGet(split[1], out LevelInfo map))
                return false;

            if (!int.TryParse(split[3], out int time))
                return false;

            if (!int.TryParse(split[6], out int uuid))
                return false;

            room = new Room(
                fullName,
                split[0].MaxChars(100),
                map,
                DifficultyToEnum(split[2]),
                time,
                DayLightToEnum(split[4]),
                split[5],
                uuid)
            {
                Options = properties
            };
            
            return true;
        }

        private static Difficulty DifficultyToEnum(string difficulty)
        {
            switch (difficulty.ToLowerInvariant())
            {
                default:
                    return Difficulty.Normal;
                case "hard":
                    return Difficulty.Hard;
                case "abnormal":
                    return Difficulty.Abnormal;
            }
        }

        public static DayLight DayLightToEnum(string dayLight)
        {
            switch (dayLight.ToLowerInvariant())
            {
                default:
                    return DayLight.Day;
                case "dawn":
                    return DayLight.Dawn;
                case "night":
                    return DayLight.Night;
            }
        }


        public void SetCustomProperties(Hashtable propertiesToSet)
        {
            if (propertiesToSet == null) 
                return;
            
            _properties.MergeStringKeys(propertiesToSet);
            _properties.StripKeysWithNullValues();
            Hashtable gameProperties = propertiesToSet.StripToStringKeys();
            if (!PhotonNetwork.offlineMode)
                PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfRoom(gameProperties, true, 0);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, propertiesToSet);
        }

        public int CompareTo(object obj)
        {
            if (obj is Room room)
            {
                if (IsJoinable && !room.IsJoinable)
                    return -1;
                
                if (IsJoinable == room.IsJoinable && 
                    !IsProtected && room.IsProtected)
                    return -1;

                if (IsProtected == room.IsProtected && 
                    IsJoinable && !room.IsJoinable)
                    return -1;
            }
            return 0;
        }

        public bool Join()
        {
#if ABUSIVE
            return PhotonNetwork.JoinRoom(_fullName);
#else
            if (!IsProtected)
                return PhotonNetwork.JoinRoom(_fullName);
    
            Shelter.LogConsole("Password is required to join the room."); //TODO: Add password ui
            return false;
#endif
        }

        public Hashtable Properties => _properties;

        public string FullName
        {
            get => _fullName;
            set => _fullName = value;
        }

        public string Name => _roomName;
        public LevelInfo Map => _roomMap;
        public Difficulty Difficulty => _roomDifficulty;
        public DayLight DayLight => _roomDayLight;
        public string Password => _roomPassword;
        public int Time => _roomTime;

        public bool IsJoinable => (_maxPlayers == 0 || _currentPlayers < _maxPlayers) && _isOpen;
        public bool IsProtected => _roomPassword.Length > 0;

        public int Players => _currentPlayers;
        
        public static int CurrentPlayers
        {
            get
            {
                if (PhotonNetwork.PlayerList != null)
                    return PhotonNetwork.PlayerList.Length;
                return 0;
            }
        }

        public int MaxPlayers
        {
            get => _maxPlayers;
            set
            {
                if (!Equals(PhotonNetwork.Room))
                    throw new Exception("Can't set open when not in that room."); //TODO: LOG
                
                if (value != _maxPlayers && !PhotonNetwork.offlineMode)
                {
                    PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
                    {
                        { GamePropertyKey.MaxPlayers, (byte) Math.Min(value, 255) }
                    }, true, 0);
                }
                _maxPlayers = value;
            }
        }

        public bool RemovedFromList => _removedFromList;

        public int PlayerTTL
        {
            get => _playerTTL;
            set
            {
                if (!Equals(PhotonNetwork.Room))
                    throw new Exception("Can't set open when not in that room."); //TODO: LOG
                
                if (value != _playerTTL && !PhotonNetwork.offlineMode)
                {
                    PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
                    {
                        { ParameterCode.PlayerTTL, value }
                    }, true, 0);

                    _playerTTL = value;
                }
            }
        }

        public int RoomTTL
        {
            get => _roomTTL;
            set
            {
                if (!Equals(PhotonNetwork.Room))
                    throw new Exception("Can't set open when not in that room."); //TODO: LOG
                
                if (value != _roomTTL && !PhotonNetwork.offlineMode)
                {
                    PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
                    {
                        { ParameterCode.EmptyRoomTTL, value }
                    }, true, 0);

                    _roomTTL = value;
                }
            }
        }
        
        public bool DoAutoCleanup => _doAutoCleanup;
        public bool IsLocalClientInside { get; set; }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (!Equals(PhotonNetwork.Room))
                    throw new Exception("Can't set open when not in that room."); //TODO: LOG
                
                if (value != _isVisible && !PhotonNetwork.offlineMode)
                {
                    PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
                    {
                        { GamePropertyKey.IsVisible, value }
                    }, true, 0);

                    _isVisible = value;
                }
            }
        }

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (!Equals(PhotonNetwork.Room))
                    throw new Exception("Can't set open when not in that room."); //TODO: LOG

                if (value != _isOpen && !PhotonNetwork.offlineMode)
                {
                    PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
                    {
                        { GamePropertyKey.IsOpen, value }
                    }, true, 0);
                    
                    _isOpen = value;
                }
            }
        }
    }
}
