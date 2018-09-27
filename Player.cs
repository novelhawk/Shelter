using System.CodeDom;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using Mod;
using Mod.Interface;
using Mod.Logging;
using Mono.Security.X509;
using UnityEngine;
using LogType = Mod.Logging.LogType;

public class Player
{
    public PlayerProperties Properties { get; }
    private int _id;
    private readonly bool _isLocal;
    private string _friendName;
    private bool _ignored;

    protected internal Player(bool isLocal, int id, Hashtable properties)
    {
        this._id = -1;
        this._friendName = string.Empty;
        this._isLocal = isLocal;
        this._id = id;
        
        this.Properties = new PlayerProperties();
        this.InternalCacheProperties(properties);
    }

    public Player(bool isLocal, int id, string friendName)
    {
        this._id = -1;
        this._friendName = string.Empty;
        this._isLocal = isLocal;
        this._id = id;
        this._friendName = friendName;
        
        this.Properties = new PlayerProperties();
    }

    public override bool Equals(object p)
    {
        Player player = p as Player;
        return player != null && this.GetHashCode() == player.GetHashCode();
    }

    public void SendPrivateMessage(string message)
    {
        Chat.AddMessage($"<color=#1068D4>TO<color=#108CD4>></color></color> <color=#{Chat.SystemColor}>{Properties.HexName}: {message}</color>");
        Chat.SendMessage($"<color=#1068D4>PM<color=#108CD4>></color></color> <color=#{Chat.SystemColor}>{Self.Properties.HexName}: {message}</color>", this);
    }
    
    public static Player Find(int id)
    {
        return PhotonNetwork.playerList.FirstOrDefault(player => player.ID == id);
    }

    public static bool TryParse(string idStr, out Player player)
    {
        if (int.TryParse(idStr, out int id))
            if (TryParse(id, out player))
                return true;
        player = null;
        return false;
    }

    public static bool TryParse(int id, out Player player)
    {
        if ((player = Find(id)) != null)
            return true;
        return false;
    }

    public Player Get(int id)
    {
        return Find(id);
    }

    public override int GetHashCode() => ID;
    
    public Player GetNext() => GetNextFor(this.ID);
    private static Player GetNextFor(int currentPlayerId)
    {
        if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.Count < 2)
            return null;
        
        Dictionary<int, Player> mActors = PhotonNetwork.networkingPeer.mActors;
        int returnId = 2147483647;
        int playerId = currentPlayerId;
        foreach (int id in mActors.Keys)
        {
            if (id < playerId)
            {
                playerId = id;
            }
            else if (id > currentPlayerId && id < returnId)
            {
                returnId = id;
            }
        }
        return returnId == 2147483647 ? mActors[playerId] : mActors[returnId];
    }

    internal void InternalCacheProperties(Hashtable properties)
    {
        if (properties != null && properties.Count != 0) //  && !this.Properties.Equals(properties)
        {
            if (!string.IsNullOrEmpty(properties[PlayerProperty.FriendName] as string))
                this._friendName = (string) properties[PlayerProperty.FriendName];
            
            this.Properties.MergeStringKeys(properties);
            this.Properties.StripKeysWithNullValues();

            if (Mod <= CustomMod.RC)
            {
                if (properties.ContainsKey("AoTTG_Mod"))
                    Mod = CustomMod.HawkMain;
                else if (properties.ContainsKey("HawkUser"))
                    Mod = CustomMod.HawkUser;
                else if (properties.ContainsKey(PlayerProperty.Shelter))
                    Mod = CustomMod.Shelter;
                else if (properties.ContainsKey("AlphaX"))
                    Mod = CustomMod.AlphaX;
                else if (properties.ContainsKey("Alpha"))
                    Mod = CustomMod.Alpha;
                else if (properties.ContainsKey("coin") || 
                         properties.ContainsKey("UPublica2") || 
                         properties.ContainsKey("Hats") ||
                         properties.ContainsKey(string.Empty))
                    Mod = CustomMod.Universe;
                else if (properties.ContainsKey("CyanMod"))
                    Mod = CustomMod.Cyan;
                else if (properties.ContainsKey("PBCheater"))
                    Mod = CustomMod.Pedobear;
                else if (properties.ContainsKey("SRC"))
                    Mod = CustomMod.SRC;
                else if (properties.ContainsKey("RCteam"))
                    Mod = CustomMod.RC;
            }
            
            if (properties.ContainsKey(PlayerProperty.Name))
                Properties.HexName = Properties.Name.HexColor();
        }
    }

    public void SetCustomProperties(Hashtable properties)
    {
        if (properties == null) 
            return;

        this.Properties.MergeStringKeys(properties);
        this.Properties.StripKeysWithNullValues();
        Hashtable actorProperties = properties.StripToStringKeys();
        if (this._id > 0 && !PhotonNetwork.offlineMode)
            PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(this._id, actorProperties, true, 0);
        NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, this, properties);
        
        if (Mod <= CustomMod.RC)
        {
            if (properties.ContainsKey("AoTTG_Mod"))
                Mod = CustomMod.HawkMain;
            else if (properties.ContainsKey("HawkUser"))
                Mod = CustomMod.HawkUser;
            else if (properties.ContainsKey(PlayerProperty.Shelter))
                Mod = CustomMod.Shelter;
            else if (properties.ContainsKey("AlphaX"))
                Mod = CustomMod.AlphaX;
            else if (properties.ContainsKey("Alpha"))
                Mod = CustomMod.Alpha;
            else if (properties.ContainsKey("coin") || 
                     properties.ContainsKey("UPublica2") || 
                     properties.ContainsKey("Hats") ||
                     properties.ContainsKey(string.Empty))
                Mod = CustomMod.Universe;
            else if (properties.ContainsKey("CyanMod"))
                Mod = CustomMod.Cyan;
            else if (properties.ContainsKey("PBCheater"))
                Mod = CustomMod.Pedobear;
            else if (properties.ContainsKey("SRC"))
                Mod = CustomMod.SRC;
            else if (properties.ContainsKey("RCteam"))
                Mod = CustomMod.RC;
        }

        if (properties.ContainsKey(PlayerProperty.Name))
            Properties.HexName = Properties.Name.HexColor();
    }

    internal void InternalChangeLocalID(int id)
    {
        if (!_isLocal)
        {
            Shelter.LogConsole("Cannot change {0}'s ID.", LogType.Error, ToString());  
            Shelter.Log("Tried to change {0}'s ID to {1}", LogType.Error, ToString(), id);
            return;
        }
        _id = id;
    }

    public override string ToString() => $"{(IsMasterClient ? "[M] " : string.Empty)}{Properties.Name} ({ID})";

    public Hashtable AllProperties
    {
        get
        {
            Hashtable target = new Hashtable();
            target.Merge(this.Properties);
            target[(byte)255] = this.FriendName;
            return target;
        }
    }

    public int ID => this._id;
    public bool IsMasterClient => Equals(PhotonNetwork.networkingPeer.mMasterClient, this);
    public static Player Self => PhotonNetwork.networkingPeer?.mLocalActor;

    public HERO Hero { get; set; }
    public TITAN Titan { get; set; }
    public CustomMod Mod { get; set; }
    public bool IsLocal => _isLocal;
    public bool IsIgnored
    {
        get => _ignored && !_isLocal;
        set
        {
            if (_isLocal)
                return;
            
            if (value != _ignored)
            {
                if (value)
                    FengGameManagerMKII.ignoreList.Add(_id);
                else
                    FengGameManagerMKII.ignoreList.Remove(_id);
            }
            
            _ignored = value; 
        }
    }
    
    public string FriendName // Friend name? 
    {
        get => _friendName;
        set
        {
            if (!this._isLocal)
            {
                Debug.LogError("Error: Cannot change the name of a remote player!");
            }
            _friendName = value;
        }
    }

    public static bool operator !=(Player a, Player b)
    {
        return !ReferenceEquals(a, b);
    }

    public static bool operator ==(Player a, Player b)
    {
        return ReferenceEquals(a, b);
    }
}

