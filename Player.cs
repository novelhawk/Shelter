using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;
using Mod;
using UnityEngine;

public class Player
{
    public PlayerProperties Properties { get; }
    private int actorID;
    public readonly bool isLocal;
    private string nameField;
    private string _hexName;

    protected internal Player(bool isLocal, int actorID, Hashtable properties)
    {
        this.actorID = -1;
        this.nameField = string.Empty;
        this.Properties = new PlayerProperties();
        this.isLocal = isLocal;
        this.actorID = actorID;
        this.InternalCacheProperties(properties);
    }

    public Player(bool isLocal, int actorID, string name)
    {
        this.actorID = -1;
        this.nameField = string.Empty;
        this.Properties = new PlayerProperties();
        this.isLocal = isLocal;
        this.actorID = actorID;
        this.nameField = name;
    }

    public string HexName => _hexName;

    public override bool Equals(object p)
    {
        Player player = p as Player;
        return player != null && this.GetHashCode() == player.GetHashCode();
    }

    public static Player Find(int ID)
    {
        return PhotonNetwork.playerList.FirstOrDefault(player => player.ID == ID);
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

    public bool Has(string prop)
    {
        return Properties[prop] != null;
    }

    internal void InternalCacheProperties(Hashtable properties)
    {
        if (properties != null && properties.Count != 0 && !this.Properties.Equals(properties))
        {
            if (properties.ContainsKey((byte)255))
            {
                this.nameField = (string)properties[(byte)255];
            }
            this.Properties.MergeStringKeys(properties);
            this.Properties.StripKeysWithNullValues();
            _hexName = Properties.Name.HexColor();
        }
    }

    internal void InternalChangeLocalID(int newID)
    {
        if (!this.isLocal)
        {
            Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
        }
        else
        {
            this.actorID = newID;
        }
    }

    public void SetCustomProperties(Hashtable propertiesToSet)
    {
        if (propertiesToSet != null)
        {
            this.Properties.MergeStringKeys(propertiesToSet);
            this.Properties.StripKeysWithNullValues();
            Hashtable actorProperties = propertiesToSet.StripToStringKeys();
            if (this.actorID > 0 && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(this.actorID, actorProperties, true, 0);
            }
            object[] parameters = new object[] { this, propertiesToSet };
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, parameters);
            _hexName = Properties.Name.HexColor();
        }
    }

    public override string ToString() => $"{(IsMasterClient ? "[M] " : string.Empty)}{HexName} ({ID})";

    public Hashtable AllProperties
    {
        get
        {
            Hashtable target = new Hashtable();
            target.Merge(this.Properties);
            target[(byte)255] = this.name;
            return target;
        }
    }

    public int ID => this.actorID;
    public bool IsMasterClient => Equals(PhotonNetwork.networkingPeer.mMasterClient, this);
    public static Player Self => PhotonNetwork.networkingPeer?.mLocalActor;

    public string name // Friend name? 
    {
        get => this.nameField;
        set
        {
            if (!this.isLocal)
            {
                Debug.LogError("Error: Cannot change the name of a remote player!");
            }
            else
            {
                this.nameField = value;
            }
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

