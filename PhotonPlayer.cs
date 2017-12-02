using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;
using Mod;
using UnityEngine;

public class PhotonPlayer
{
    private int actorID;
    public readonly bool isLocal;
    private string nameField;
    public object TagObject;
    private string _hexName;

    protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
    {
        this.actorID = -1;
        this.nameField = string.Empty;
        this.CustomProperties = new Hashtable();
        this.isLocal = isLocal;
        this.actorID = actorID;
        this.InternalCacheProperties(properties);
    }

    public PhotonPlayer(bool isLocal, int actorID, string name)
    {
        this.actorID = -1;
        this.nameField = string.Empty;
        this.CustomProperties = new Hashtable();
        this.isLocal = isLocal;
        this.actorID = actorID;
        this.nameField = name;
    }

    public string HexName => _hexName;

    public override bool Equals(object p)
    {
        PhotonPlayer player = p as PhotonPlayer;
        return player != null && this.GetHashCode() == player.GetHashCode();
    }

    public static PhotonPlayer Find(int ID)
    {
        return PhotonNetwork.playerList.FirstOrDefault(player => player.ID == ID);
    }

    public static bool TryParse(string idStr, out PhotonPlayer player)
    {
        if (int.TryParse(idStr, out int id))
            if (TryParse(id, out player))
                return true;
        player = null;
        return false;
    }

    public static bool TryParse(int id, out PhotonPlayer player)
    {
        player = Find(id);
        if (player == null)
            return false;
        return true;
    }

    public PhotonPlayer Get(int id)
    {
        return Find(id);
    }

    public override int GetHashCode()
    {
        return this.ID;
    }

    public PhotonPlayer GetNext()
    {
        return this.GetNextFor(this.ID);
    }

    public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
    {
        if (currentPlayer == null)
        {
            return null;
        }
        return this.GetNextFor(currentPlayer.ID);
    }

    public PhotonPlayer GetNextFor(int currentPlayerId)
    {
        if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.Count < 2)
        {
            return null;
        }
        Dictionary<int, PhotonPlayer> mActors = PhotonNetwork.networkingPeer.mActors;
        int num = 2147483647;
        int num2 = currentPlayerId;
        foreach (int num3 in mActors.Keys)
        {
            if (num3 < num2)
            {
                num2 = num3;
            }
            else if (num3 > currentPlayerId && num3 < num)
            {
                num = num3;
            }
        }
        return num == 2147483647 ? mActors[num2] : mActors[num];
    }

    public bool Has(string prop)
    {
        return CustomProperties[prop] != null;
    }

    internal void InternalCacheProperties(Hashtable properties)
    {
        if (properties != null && properties.Count != 0 && !this.CustomProperties.Equals(properties))
        {
            if (properties.ContainsKey((byte)255))
            {
                this.nameField = (string)properties[(byte)255];
            }
            this.CustomProperties.MergeStringKeys(properties);
            this.CustomProperties.StripKeysWithNullValues();
            _hexName = CustomProperties[PhotonPlayerProperty.name].ToString().HexColor();
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
            this.CustomProperties.MergeStringKeys(propertiesToSet);
            this.CustomProperties.StripKeysWithNullValues();
            Hashtable actorProperties = propertiesToSet.StripToStringKeys();
            if (this.actorID > 0 && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(this.actorID, actorProperties, true, 0);
            }
            object[] parameters = new object[] { this, propertiesToSet };
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, parameters);
            _hexName = CustomProperties[PhotonPlayerProperty.name].ToString().HexColor();
        }
    }

    public override string ToString() => $"{(IsMasterClient ? "[M] " : string.Empty)}{HexName} ({ID})";

    public string ToStringFull()
    {
        return string.Format("#{0:00} '{1}' {2}", this.ID, this.name, this.CustomProperties.ToStringFull());
    }

    public Hashtable allProperties
    {
        get
        {
            Hashtable target = new Hashtable();
            target.Merge(this.CustomProperties);
            target[(byte)255] = this.name;
            return target;
        }
    }

    public Hashtable CustomProperties { get; private set; }

    public int ID
    {
        get
        {
            return this.actorID;
        }
    }

    public bool IsMasterClient => Equals(PhotonNetwork.networkingPeer.mMasterClient, this);

    public static PhotonPlayer Self => PhotonNetwork.networkingPeer?.mLocalActor;

    public string name
    {
        get
        {
            return this.nameField;
        }
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

    public static bool operator !=(PhotonPlayer a, PhotonPlayer b)
    {
        return !ReferenceEquals(a, b);
    }

    public static bool operator ==(PhotonPlayer a, PhotonPlayer b)
    {
        return ReferenceEquals(a, b);
    }
}

