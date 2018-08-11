//using System.Linq;
//using ExitGames.Client.Photon;
//using UnityEngine;
//
//namespace Mod
//{
//    public class Player
//    {
//        private readonly int _id;
//        private readonly int _photonId;
//        private readonly bool _IsLocal;
//        private readonly bool _isMasterclient;
//        private string _hexName;
//
//        public PlayerProperties Properties { get; }
//
//        public Player(int id)
//        {
//            _photonId = id; // remove later
//            _IsLocal = id == -1 || id == PhotonNetwork.networkingPeer.mLocalActor.ID;
//            _isMasterclient = PhotonNetwork.networkingPeer != null && id == PhotonNetwork.networkingPeer.mMasterClient.ID;
//            if (_IsLocal)
//                _id = 0;
//            else if (_isMasterclient)
//                _id = 1;
//            else
//                _id = GetLastID();
//
//            _hexName = "Unknown";
//            Properties = new PlayerProperties();
//            Shelter.Playerlist.Add(this);
//        }
//
//        public Player(int id, PlayerProperties properties)
//        {
//            _photonId = id; // remove later
//            _IsLocal = id == PhotonNetwork.networkingPeer.mLocalActor.ID;
//            _isMasterclient = id == PhotonNetwork.networkingPeer.mMasterClient.ID;
//            if (_IsLocal)
//                _id = 0;
//            else if (_isMasterclient)
//                _id = 1;
//            else
//                _id = GetLastID();
//
//            _hexName = Properties.Name.HexColor();
//            Properties = properties;
//            Shelter.Playerlist.Add(this);
//        }
//
//        public static Player Self => PhotonNetwork.networkingPeer.mLocalActor;
//
//        private static int GetLastID()
//        {
//            int lastID = 2;
//            foreach (Player player in PhotonNetwork.playerList)
//                if (player._id >= lastID)
//                    lastID = player._id + 1;
//            return lastID;
//        }
//
//        public void InternalChangeProperties(Hashtable properties)
//        {
//            if (properties == null || properties.Count == 0) return;
//            Properties.ConvertFromHashtable(properties);
//            _hexName = Properties.Name.HexColor();
//        }
//
//        public void SetCustomProperties(Hashtable properties)
//        {
//            if (properties == null || properties.Count == 0) return;
//            Properties.ConvertFromHashtable(properties);
//            _hexName = Properties.Name.HexColor();
//            if (!PhotonNetwork.offlineMode)
//                PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(_photonId, Properties.ToHashtable(), true, 0);
//            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPlayerPropertiesChanged, this, properties);
//        }
//
//        public void InternalChangeLocalID(int id)
//        {
//            if (IsLocal)
//            {
//                Debug.Log($"Player.InternalChangeLocalID(): ID: {_id} PhotonID: {_photonId} NewID: {id}");
//                Application.Quit();
////                _photonId = id;
////                ID = newID; TODO: don't allow it with new system
//            }
//        }
//
//        public static bool TryParse(string id, out Player player)
//        {
//            if (int.TryParse(id, out int intId))
//                return TryParse(intId, out player);
//            player = null;
//            return false;
//        }
//
//        public static bool TryParse(int id, out Player player)
//        {
//            player = Shelter.Playerlist.GetFromID(id);
//            if (player == null)
//                return false;
//            return true;
//        }
//
//        public static bool FromPhoton(int id, out Player player)
//        {
//            player = Shelter.Playerlist.GetFromPhotonID(id);
//            if (player == null)
//                return false;
//            return true;
//        }
//
//        public override string ToString() => $"{(_isMasterclient ? "[M] " : string.Empty)}{_hexName} ({_id})";
//
//        public override bool Equals(object obj)
//        {
//            if (obj is Player player && ReferenceEquals(this, player))
//                return true;
//            return false;
//        }
//
//        public override int GetHashCode()
//        {
//            return _id;
//        }
//
//        public static bool operator ==(Player a, Player b)
//        {
//            return ReferenceEquals(a, b);
//        }
//
//        public static bool operator !=(Player a, Player b)
//        {
//            return !ReferenceEquals(a, b);
//        }
//
//        public HERO Hero { get; set; }
//        public int ID => _photonId;
//        public int PhotonID => _photonId;
//        public bool IsMasterclient => _isMasterclient;
//        public string Name => _hexName;
//        public string Guild => Properties.Guild;
//
//        public string FriendName
//        {
//            get => Properties.FriendName;
//            set => Properties.FriendName = value;
//        }
//
//        public bool IsLocal => _IsLocal;
//        public bool IsIgnored => FengGameManagerMKII.ignoreList.Contains(_id); //todo: substitute with _id
//
//    }
//}
