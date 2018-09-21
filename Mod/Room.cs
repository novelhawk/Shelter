using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using ExitGames.Client.Photon;
using Mod.Interface;
using UnityEngine;

namespace Mod
{
    public class Room : IComparable
    {
        public static List<Room> List;
        
        private readonly Hashtable _properties = new Hashtable();

        public string FullName { get; set; }
        private readonly string _roomName;
        private readonly LevelInfo _roomMap;
        private readonly Difficulty _roomDifficulty;
        private readonly DayLight _roomDayLight;
        private readonly string _roomPassword;
        private readonly int _roomTime;
        private int _roomUUID;


        private bool _removedFromList;
        private int _maxPlayers;
        private int _currentPlayers;
        private bool _isVisible;
        private bool _isOpen;
        private bool _doAutoCleanup;
        private int _playerTTL;
        private int _roomTTL;

        public Room(string fullName, Hashtable properties)
        {
            FullName = fullName;
            
            var split = fullName.Split('`');
            if (split.Length < 6)
                throw new Exception("Room..ctor called with invalid arguments: " + nameof(fullName)); //TODO: LOG!

            _roomName = split[0].MaxChars(100);
            _roomMap = LevelInfoManager.GetInfo(split[1]);
            _roomDifficulty = DifficultyToEnum(split[2]);
            if (int.TryParse(split[3], out int time))
                _roomTime = time;
            _roomDayLight = DayLightToEnum(split[4]);
            _roomPassword = split[5];
            if (int.TryParse(split[6], out int uuid))
                _roomUUID = uuid;

            LoadFromHashtable(properties);
        }

        public Room(string fullName, RoomOptions options)
        {
            FullName = fullName;

            var split = fullName.Split('`');
            if (split.Length < 6)
                throw new Exception(); //TODO: LOG!

            _roomName = split[0];
            _roomMap = LevelInfoManager.GetInfo(split[1]);
            _roomDifficulty = DifficultyToEnum(split[2]);
            if (int.TryParse(split[3], out int time))
                _roomTime = time;
            _roomDayLight = DayLightToEnum(split[4]);
            _roomPassword = split[5];
            if (int.TryParse(split[6], out int uuid))
                _roomUUID = uuid;

            if (options != null)
            {
                _removedFromList = options.RemovedFromList;
                _maxPlayers = options.MaxPlayers;
                _currentPlayers = options.CurrentPlayers;
                _isVisible = options.IsVisible;
                _isOpen = options.IsOpen;
                _doAutoCleanup = options.DoAutoCleanup;
            }
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

        public void LoadFromHashtable(Hashtable hash)
        {
            if (hash != null && hash.Count > 0)
            {
                if (hash.ContainsKey((byte) 251))
                    _removedFromList = (bool) hash[(byte) 251];
                if (_removedFromList)
                    return;
                
                if (hash.ContainsKey((byte) 255))
                    _maxPlayers = (byte) hash[(byte) 255];
                
                if (hash.ContainsKey((byte) 253))
                    _isOpen = (bool) hash[(byte) 253];
                
                if (hash.ContainsKey((byte) 254))
                    _isVisible = (bool) hash[(byte) 254];
                
                if (hash.ContainsKey((byte) 252))
                    _currentPlayers = (byte) hash[(byte) 252];
                
                if (hash.ContainsKey((byte) 249))
                    _doAutoCleanup = (bool) hash[(byte) 249];

                if (hash.ContainsKey((byte) 235))
                    _playerTTL = (int) hash[(byte) 235];

                if (hash.ContainsKey((byte) 236))
                    _roomTTL = (int) hash[(byte) 236];
                
                _properties.MergeStringKeys(hash);
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

        public bool Join() => PhotonNetwork.JoinRoom(FullName);
        public string ToString(int alpha)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("<color=#5D334B{0:X2}>", alpha);
            if (IsProtected)
                builder.AppendFormat("<color=#034C94{0:X2}>[</color><color=#1191D1{0:X2}>PW</color><color=#034C94{0:X2}>]</color> ", alpha);
            if (!_isOpen)
                builder.AppendFormat("<color=#034C94{0:X2}>[</color><color=#FF0000{0:X2}>CLOSED</color><color=#034C94{0:X2}>]</color> ", alpha);
            builder.AppendFormat("{0} || {1} || ", Name.RemoveColors(), Map.Name);
            builder.AppendFormat("<color=#{1}{0:X2}>", alpha, IsJoinable ? "00FF00" : "FF0000");
            builder.AppendFormat("{0}/{1}", _currentPlayers, _maxPlayers);
            builder.Append("</color></color>");
            return builder.ToString();
        }


        public Hashtable Properties => _properties;
        
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
                if (PhotonNetwork.playerList != null)
                    return PhotonNetwork.playerList.Length;
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
                        { (byte)255, (byte) Math.Min(value, 255) }
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
                        { (byte)235, value }
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
                        { (byte)236, value }
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
                        { (byte)254, value }
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
                        { (byte)253, value }
                    }, true, 0);
                    
                    _isOpen = value;
                }
            }
        }
    }
}
