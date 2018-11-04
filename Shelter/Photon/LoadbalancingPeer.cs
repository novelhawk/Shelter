using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using System.Collections.Generic;
using Photon;
using Photon.Enums;

// ReSharper disable once CheckNamespace
internal class LoadbalancingPeer : PhotonPeer
{
    public LoadbalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : base(listener, protocolType)
    {
    }

    public virtual bool OpAuthenticate(string appId, string appVersion, string userId, AuthenticationValues authValues, string regionCode)
    {
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>();
        if (authValues?.Secret != null)
        {
            customOpParameters[ParameterCode.Secret] = authValues.Secret;
            return this.OpCustom(OperationCode.Authenticate, customOpParameters, true, 0, false);
        }
        customOpParameters[ParameterCode.AppVersion] = appVersion;
        customOpParameters[ParameterCode.ApplicationId] = appId;
        
        if (!string.IsNullOrEmpty(regionCode))
            customOpParameters[ParameterCode.Region] = regionCode;
        
        if (!string.IsNullOrEmpty(userId))
            customOpParameters[ParameterCode.UserId] = userId;
        
        if (authValues != null && authValues.AuthType != CustomAuthenticationType.None)
        {
            if (!IsEncryptionAvailable)
            {
                Listener.DebugReturn(DebugLevel.ERROR, "OpAuthenticate() failed. When you want Custom Authentication encryption is mandatory.");
                return false;
            }
            customOpParameters[ParameterCode.ClientAuthenticationType] = (byte) authValues.AuthType;
            
            if (!string.IsNullOrEmpty(authValues.Secret))
                customOpParameters[ParameterCode.Secret] = authValues.Secret;
            
            if (!string.IsNullOrEmpty(authValues.AuthParameters))
                customOpParameters[ParameterCode.ClientAuthenticationParams] = authValues.AuthParameters;
            
            if (authValues.AuthPostData != null)
                customOpParameters[ParameterCode.ClientAuthenticationData] = authValues.AuthPostData;
        }
        
        return this.OpCustom(OperationCode.Authenticate, customOpParameters, true, 0, IsEncryptionAvailable);
    }

    public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
    {
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>(2);
        if (groupsToRemove != null)
            customOpParameters[ParameterCode.Remove] = groupsToRemove;
        
        if (groupsToAdd != null)
            customOpParameters[ParameterCode.Add] = groupsToAdd;
        
        return this.OpCustom(OperationCode.ChangeGroups, customOpParameters, true, 0);
    }

    public virtual bool OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, Hashtable playerProperties, bool onGameServer)
    {
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>();
        if (!string.IsNullOrEmpty(roomName))
        {
            customOpParameters[ParameterCode.RoomName] = roomName;
        }
        if (lobby != null)
        {
            customOpParameters[ParameterCode.LobbyName] = lobby.Name;
            customOpParameters[ParameterCode.LobbyType] = (byte) lobby.Type;
        }
        if (onGameServer)
        {
            if (playerProperties != null && playerProperties.Count > 0)
            {
                customOpParameters[ParameterCode.PlayerProperties] = playerProperties;
                customOpParameters[ParameterCode.Broadcast] = true;
            }
            if (roomOptions == null)
                roomOptions = new RoomOptions();
            
            Hashtable target = new Hashtable();
            customOpParameters[ParameterCode.GameProperties] = target;
            target.MergeStringKeys(roomOptions);
            target[GamePropertyKey.IsOpen] = roomOptions.IsOpen;
            target[GamePropertyKey.IsVisible] = roomOptions.IsVisible;
            target[GamePropertyKey.PropsListedInLobby] = new string[0];
            if (roomOptions.MaxPlayers > 0)
                target[GamePropertyKey.MaxPlayers] = roomOptions.MaxPlayers;
            
            if (roomOptions.DoAutoCleanup == true)
            {
                customOpParameters[ParameterCode.CleanupCacheOnLeave] = true;
                target[GamePropertyKey.CleanupCacheOnLeave] = true;
            }
        }
        return this.OpCustom(OperationCode.CreateGame, customOpParameters, true);
    }

    public virtual bool OpFindFriends(string[] friendsToFind)
    {
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>();
        if (friendsToFind != null && friendsToFind.Length > 0)
            customOpParameters[1] = friendsToFind;
        
        return this.OpCustom(OperationCode.FindFriends, customOpParameters, true);
    }

    public virtual bool OpGetRegions(string appId)
    {
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>
        {
            [ParameterCode.ApplicationId] = appId
        };
        return this.OpCustom(OperationCode.GetRegions, customOpParameters, true, 0, true);
    }

    public virtual bool OpJoinLobby(TypedLobby lobby)
    {
        if (DebugOut >= DebugLevel.INFO)
        {
            Listener.DebugReturn(DebugLevel.INFO, "OpJoinLobby()");
        }
        Dictionary<byte, object> customOpParameters = null;
        if (lobby != null && !lobby.IsDefault)
        {
            customOpParameters = new Dictionary<byte, object>
            {
                [ParameterCode.LobbyName] = lobby.Name,
                [ParameterCode.LobbyType] = (byte)lobby.Type
            };
        }
        return this.OpCustom(OperationCode.JoinLobby, customOpParameters, true);
    }

    public virtual bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, Hashtable playerProperties, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
    {
        Hashtable target = new Hashtable();
        target.MergeStringKeys(expectedCustomRoomProperties);
        if (expectedMaxPlayers > 0)
            target[GamePropertyKey.MaxPlayers] = expectedMaxPlayers;
        
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>();
        if (target.Count > 0)
            customOpParameters[ParameterCode.GameProperties] = target;
        
        if (playerProperties != null && playerProperties.Count > 0)
            customOpParameters[ParameterCode.PlayerProperties] = playerProperties;
        
        if (matchingType != MatchmakingMode.FillRoom)
            customOpParameters[ParameterCode.MatchMakingType] = (byte) matchingType;
        
        if (typedLobby != null)
        {
            customOpParameters[ParameterCode.LobbyName] = typedLobby.Name;
            customOpParameters[ParameterCode.LobbyType] = (byte) typedLobby.Type;
        }
        
        if (!string.IsNullOrEmpty(sqlLobbyFilter))
            customOpParameters[245] = sqlLobbyFilter;
        
        return this.OpCustom(OperationCode.JoinRandomGame, customOpParameters, true);
    }

    public virtual bool OpJoinRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, bool createIfNotExists, Hashtable playerProperties, bool onGameServer)
    {
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>();
        if (!string.IsNullOrEmpty(roomName))
        {
            customOpParameters[ParameterCode.RoomName] = roomName;
        }
        if (createIfNotExists)
        {
            customOpParameters[ParameterCode.CreateIfNotExists] = true;
            if (lobby != null)
            {
                customOpParameters[ParameterCode.LobbyName] = lobby.Name;
                customOpParameters[ParameterCode.LobbyType] = (byte) lobby.Type;
            }
        }
        if (onGameServer)
        {
            if (playerProperties != null && playerProperties.Count > 0)
            {
                customOpParameters[ParameterCode.PlayerProperties] = playerProperties;
                customOpParameters[ParameterCode.Broadcast] = true;
            }
            if (createIfNotExists)
            {
                if (roomOptions == null)
                {
                    roomOptions = new RoomOptions();
                }
                Hashtable target = new Hashtable();
                customOpParameters[248] = target;
                target.MergeStringKeys(roomOptions);
                target[GamePropertyKey.IsOpen] = roomOptions.IsOpen;
                target[GamePropertyKey.IsVisible] = roomOptions.IsVisible;
                target[GamePropertyKey.PropsListedInLobby] = new string[0];
                if (roomOptions.MaxPlayers > 0)
                {
                    target[GamePropertyKey.MaxPlayers] = roomOptions.MaxPlayers;
                }
                if (roomOptions.DoAutoCleanup == true)
                {
                    customOpParameters[ParameterCode.CleanupCacheOnLeave] = true;
                    target[GamePropertyKey.CleanupCacheOnLeave] = true;
                }
            }
        }
        return this.OpCustom(OperationCode.JoinGame, customOpParameters, true);
    }

    public virtual bool OpLeaveLobby()
    {
        if (DebugOut >= DebugLevel.INFO)
        {
            Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
        }
        return this.OpCustom(OperationCode.LeaveLobby, null, true);
    }

    public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
    {
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>
        {
            [ParameterCode.Code] = eventCode
        };
        if (customEventContent != null)
            customOpParameters[ParameterCode.Data] = customEventContent;
        
        if (raiseEventOptions == null)
        {
            raiseEventOptions = RaiseEventOptions.Default;
        }
        else
        {
            if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
                customOpParameters[ParameterCode.Cache] = (byte) raiseEventOptions.CachingOption;
            
            if (raiseEventOptions.Receivers != ReceiverGroup.Others)
                customOpParameters[ParameterCode.ReceiverGroup] = (byte) raiseEventOptions.Receivers;
            
            if (raiseEventOptions.InterestGroup != 0)
                customOpParameters[ParameterCode.Group] = raiseEventOptions.InterestGroup;
            
            if (raiseEventOptions.TargetActors != null)
                customOpParameters[ParameterCode.ActorList] = raiseEventOptions.TargetActors;
            
            if (raiseEventOptions.ForwardToWebhook)
                customOpParameters[ParameterCode.EventForward] = true;
        }
        return this.OpCustom(OperationCode.RaiseEvent, customOpParameters, sendReliable, raiseEventOptions.SequenceChannel, false);
    }

    public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties, bool broadcast, byte channelId)
    {
        return this.OpSetPropertiesOfActor(actorNr, actorProperties.StripToStringKeys(), broadcast, channelId);
    }

    public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
    {
        return this.OpSetPropertiesOfRoom(gameProperties.StripToStringKeys(), broadcast, channelId);
    }

    protected bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, bool broadcast, byte channelId)
    {
        if (DebugOut >= DebugLevel.INFO)
        {
            Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
        }
        if (actorNr > 0 && actorProperties != null)
        {
            Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>
            {
                { ParameterCode.Properties, actorProperties },
                { ParameterCode.ActorNr, actorNr }
            };
            
            if (broadcast)
                customOpParameters.Add(250, true);
            
            return this.OpCustom(OperationCode.SetProperties, customOpParameters, broadcast, channelId);
        }
        if (DebugOut >= DebugLevel.INFO)
            Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties != null.");
        return false;
    }

    public bool OpSetPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
    {
        if (DebugOut >= DebugLevel.INFO)
        {
            Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
        }
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>
        {
            { ParameterCode.Properties, gameProperties }
        };
        
        if (broadcast)
            customOpParameters.Add(ParameterCode.Broadcast, true);
        
        return this.OpCustom(OperationCode.SetProperties, customOpParameters, broadcast, channelId);
    }

    protected void OpSetPropertyOfRoom(byte propCode, object value)
    {
        Hashtable gameProperties = new Hashtable
        {
            [propCode] = value
        };
        this.OpSetPropertiesOfRoom(gameProperties, true, 0);
    }
}

