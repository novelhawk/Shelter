using ExitGames.Client.Photon;
using System;
using System.Runtime.CompilerServices;

public class RoomInfo
{
    protected bool autoCleanUpField = PhotonNetwork.autoCleanUpPlayerObjects;
    private Hashtable customPropertiesField = new Hashtable();
    protected byte maxPlayersField;
    protected string nameField;
    protected bool openField = true;
    protected bool visibleField = true;

    protected internal RoomInfo(string roomName, Hashtable properties)
    {
        this.CacheProperties(properties);
        this.nameField = roomName;
    }

    protected internal void CacheProperties(Hashtable propertiesToCache)
    {
        if (((propertiesToCache != null) && (propertiesToCache.Count != 0)) && !this.customPropertiesField.Equals(propertiesToCache))
        {
            if (propertiesToCache.ContainsKey((byte) 251))
            {
                this.removedFromList = (bool) propertiesToCache[(byte) 251];
                if (this.removedFromList)
                {
                    return;
                }
            }
            if (propertiesToCache.ContainsKey((byte) 255))
            {
                this.maxPlayersField = (byte) propertiesToCache[(byte) 255];
            }
            if (propertiesToCache.ContainsKey((byte) 253))
            {
                this.openField = (bool) propertiesToCache[(byte) 253];
            }
            if (propertiesToCache.ContainsKey((byte) 254))
            {
                this.visibleField = (bool) propertiesToCache[(byte) 254];
            }
            if (propertiesToCache.ContainsKey((byte) 252))
            {
                this.playerCount = (byte) propertiesToCache[(byte) 252];
            }
            if (propertiesToCache.ContainsKey((byte) 249))
            {
                this.autoCleanUpField = (bool) propertiesToCache[(byte) 249];
            }
            this.customPropertiesField.MergeStringKeys(propertiesToCache);
        }
    }

    public override bool Equals(object p)
    {
        Room room = p as Room;
        return ((room != null) && this.nameField.Equals(room.nameField));
    }

    public override int GetHashCode()
    {
        return this.nameField.GetHashCode();
    }

    public override string ToString()
    {
        object[] args = new object[] { this.nameField, !this.visibleField ? "hidden" : "visible", !this.openField ? "closed" : "open", this.maxPlayersField, this.playerCount };
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", args);
    }

    public string ToStringFull()
    {
        object[] args = new object[] { this.nameField, !this.visibleField ? "hidden" : "visible", !this.openField ? "closed" : "open", this.maxPlayersField, this.playerCount, this.customPropertiesField.ToStringFull() };
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", args);
    }

    public Hashtable customProperties
    {
        get
        {
            return this.customPropertiesField;
        }
    }

    public bool isLocalClientInside { get; set; }

    public byte maxPlayers
    {
        get
        {
            return this.maxPlayersField;
        }
    }

    public string name
    {
        get
        {
            return this.nameField;
        }
    }

    public bool open
    {
        get
        {
            return this.openField;
        }
    }

    public int playerCount { get; private set; }

    public bool removedFromList { get; internal set; }

    public bool visible
    {
        get
        {
            return this.visibleField;
        }
    }
}

