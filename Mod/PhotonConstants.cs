using System;

namespace Mod
{
    /// <summary>
    /// Class for constants. These (byte) values are for "well known" room/game properties used in Photon Loadbalancing.
    /// Pun uses these constants internally.
    /// </summary>
    /// <remarks>
    /// "Custom properties" have to use a string-type as key. They can be assigned at will.
    /// </remarks>
    public static class GamePropertyKey
    {
        /// <summary>(255) Max number of players that "fit" into this room. 0 is for "unlimited".</summary>
        public const byte MaxPlayers = 255;

        /// <summary>(254) Makes this room listed or not in the lobby on master.</summary>
        public const byte IsVisible = 254;

        /// <summary>(253) Allows more players to join a room (or not).</summary>
        public const byte IsOpen = 253;

        /// <summary>(252) Current count of players in the room. Used only in the lobby on master.</summary>
        public const byte PlayerCount = 252;

        /// <summary>(251) True if the room is to be removed from room listing (used in update to room list in lobby on master)</summary>
        public const byte Removed = 251;

        /// <summary>(250) A list of the room properties to pass to the RoomInfo list in a lobby. This is used in CreateRoom, which defines this list once per room.</summary>
        public const byte PropsListedInLobby = 250;

        /// <summary>(249) Equivalent of Operation Join parameter CleanupCacheOnLeave.</summary>
        public const byte CleanupCacheOnLeave = 249;

        /// <summary>(248) Code for MasterClientId, which is synced by server. When sent as op-parameter this is (byte)203. As room property this is (byte)248.</summary>
        /// <remarks>Tightly related to ParameterCode.MasterClientId.</remarks>
        public const byte MasterClientId = 248;

        /// <summary>(247) Code for ExpectedUsers in a room. Matchmaking keeps a slot open for the players with these userIDs.</summary>
        public const byte ExpectedUsers = 247;
    }
    
    /// <summary>
    /// Class for constants. These values are for events defined by Photon Loadbalancing.
    /// Pun uses these constants internally.
    /// </summary>
    /// <remarks>They start at 255 and go DOWN. Your own in-game events can start at 0.</remarks>
    public static class EventCode
    {
        /// <summary>(230) Initial list of RoomInfos (in lobby on Master)</summary>
        public const byte GameList = 230;

        /// <summary>(229) Update of RoomInfos to be merged into "initial" list (in lobby on Master)</summary>
        public const byte GameListUpdate = 229;

        /// <summary>(228) Currently not used. State of queueing in case of server-full</summary>
        public const byte QueueState = 228;

        /// <summary>(227) Currently not used. Event for matchmaking</summary>
        public const byte Match = 227;

        /// <summary>(226) Event with stats about this application (players, rooms, etc)</summary>
        public const byte AppStats = 226;

        /// <summary>(224) This event provides a list of lobbies with their player and game counts.</summary>
        public const byte LobbyStats = 224;

        /// <summary>(210) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureNodeInfo = 210;

        /// <summary>(255) Event Join: someone joined the game. The new actorNumber is provided as well as the properties of that actor (if set in OpJoin).</summary>
        public const byte Join = 255;

        /// <summary>(254) Event Leave: The player who left the game can be identified by the actorNumber.</summary>
        public const byte Leave = 254;

        /// <summary>(253) When you call OpSetProperties with the broadcast option "on", this event is fired. It contains the properties being set.</summary>
        public const byte PropertiesChanged = 253;

        /// <summary>(253) When you call OpSetProperties with the broadcast option "on", this event is fired. It contains the properties being set.</summary>
        [Obsolete("Use PropertiesChanged now.")]
        public const byte SetProperties = 253;

        /// <summary>(252) When player left game unexpected and the room has a playerTtl > 0, this event is fired to let everyone know about the timeout.</summary>
        /// Obsolete. Replaced by Leave. public const byte Disconnect = LiteEventCode.Disconnect;

        /// <summary>(251) Sent by Photon Cloud when a plugin-call or webhook-call failed. Usually, the execution on the server continues, despite the issue. Contains: ParameterCode.Info.</summary>
        /// <seealso cref="https://doc.photonengine.com/en/realtime/current/reference/webhooks#options"/>
        public const byte ErrorInfo = 251;

        /// <summary>(250) Sent by Photon whent he event cache slice was changed. Done by OpRaiseEvent.</summary>
        public const byte CacheSliceChanged = 250;

        /// <summary>(223) Sent by Photon to update a token before it times out.</summary>
        public const byte AuthEvent = 223;
    }
    
    /// <summary>
    /// Class for constants. These (byte) values define "well known" properties for an Actor / Player.
    /// Pun uses these constants internally.
    /// </summary>
    /// <remarks>
    /// "Custom properties" have to use a string-type as key. They can be assigned at will.
    /// </remarks>
    public static class ActorProperties
    {
        /// <summary>(255) Name of a player/actor.</summary>
        public const byte PlayerName = 255; // was: 1

        /// <summary>(254) Tells you if the player is currently in this game (getting events live).</summary>
        /// <remarks>A server-set value for async games, where players can leave the game and return later.</remarks>
        public const byte IsInactive = 254;

        /// <summary>(253) UserId of the player. Sent when room gets created with RoomOptions.PublishUserId = true.</summary>
        public const byte UserId = 253;
    }
    
    /// <summary>
    /// ErrorCode defines the default codes associated with Photon client/server communication.
    /// </summary>
    public static class ErrorCode
    {
        /// <summary>(0) is always "OK", anything else an error or specific situation.</summary>
        public const int Ok = 0;

        // server - Photon low(er) level: <= 0

        /// <summary>
        /// (-3) Operation can't be executed yet (e.g. OpJoin can't be called before being authenticated, RaiseEvent cant be used before getting into a room).
        /// </summary>
        /// <remarks>
        /// Before you call any operations on the Cloud servers, the automated client workflow must complete its authorization.
        /// In PUN, wait until State is: JoinedLobby (with AutoJoinLobby = true) or ConnectedToMaster (AutoJoinLobby = false)
        /// </remarks>
        public const int OperationNotAllowedInCurrentState = -3;

        /// <summary>(-2) The operation you called is not implemented on the server (application) you connect to. Make sure you run the fitting applications.</summary>
        [Obsolete("Use InvalidOperation.")]
        public const int InvalidOperationCode = -2;

        /// <summary>(-2) The operation you called could not be executed on the server.</summary>
        /// <remarks>
        /// Make sure you are connected to the server you expect.
        ///
        /// This code is used in several cases:
        /// The arguments/parameters of the operation might be out of range, missing entirely or conflicting.
        /// The operation you called is not implemented on the server (application). Server-side plugins affect the available operations.
        /// </remarks>
        public const int InvalidOperation = -2;

        /// <summary>(-1) Something went wrong in the server. Try to reproduce and contact Exit Games.</summary>
        public const int InternalServerError = -1;

        // server - PhotonNetwork: 0x7FFF and down
        // logic-level error codes start with short.max

        /// <summary>(32767) Authentication failed. Possible cause: AppId is unknown to Photon (in cloud service).</summary>
        public const int InvalidAuthentication = 0x7FFF;

        /// <summary>(32766) GameId (name) already in use (can't create another). Change name.</summary>
        public const int GameIdAlreadyExists = 0x7FFF - 1;

        /// <summary>(32765) Game is full. This rarely happens when some player joined the room before your join completed.</summary>
        public const int GameFull = 0x7FFF - 2;

        /// <summary>(32764) Game is closed and can't be joined. Join another game.</summary>
        public const int GameClosed = 0x7FFF - 3;

        [Obsolete("No longer used, cause random matchmaking is no longer a process.")]
        public const int AlreadyMatched = 0x7FFF - 4;

        /// <summary>(32762) Not in use currently.</summary>
        public const int ServerFull = 0x7FFF - 5;

        /// <summary>(32761) Not in use currently.</summary>
        public const int UserBlocked = 0x7FFF - 6;

        /// <summary>(32760) Random matchmaking only succeeds if a room exists thats neither closed nor full. Repeat in a few seconds or create a new room.</summary>
        public const int NoRandomMatchFound = 0x7FFF - 7;

        /// <summary>(32758) Join can fail if the room (name) is not existing (anymore). This can happen when players leave while you join.</summary>
        public const int GameDoesNotExist = 0x7FFF - 9;

        /// <summary>(32757) Authorization on the Photon Cloud failed becaus the concurrent users (CCU) limit of the app's subscription is reached.</summary>
        /// <remarks>
        /// Unless you have a plan with "CCU Burst", clients might fail the authentication step during connect.
        /// Affected client are unable to call operations. Please note that players who end a game and return
        /// to the master server will disconnect and re-connect, which means that they just played and are rejected
        /// in the next minute / re-connect.
        /// This is a temporary measure. Once the CCU is below the limit, players will be able to connect an play again.
        ///
        /// OpAuthorize is part of connection workflow but only on the Photon Cloud, this error can happen.
        /// Self-hosted Photon servers with a CCU limited license won't let a client connect at all.
        /// </remarks>
        public const int MaxCcuReached = 0x7FFF - 10;

        /// <summary>(32756) Authorization on the Photon Cloud failed because the app's subscription does not allow to use a particular region's server.</summary>
        /// <remarks>
        /// Some subscription plans for the Photon Cloud are region-bound. Servers of other regions can't be used then.
        /// Check your master server address and compare it with your Photon Cloud Dashboard's info.
        /// https://cloud.photonengine.com/dashboard
        ///
        /// OpAuthorize is part of connection workflow but only on the Photon Cloud, this error can happen.
        /// Self-hosted Photon servers with a CCU limited license won't let a client connect at all.
        /// </remarks>
        public const int InvalidRegion = 0x7FFF - 11;

        /// <summary>
        /// (32755) Custom Authentication of the user failed due to setup reasons (see Cloud Dashboard) or the provided user data (like username or token). Check error message for details.
        /// </summary>
        public const int CustomAuthenticationFailed = 0x7FFF - 12;

        /// <summary>(32753) The Authentication ticket expired. Usually, this is refreshed behind the scenes. Connect (and authorize) again.</summary>
        public const int AuthenticationTicketExpired = 0x7FF1;

        /// <summary>
        /// (32752) A server-side plugin (or webhook) failed to execute and reported an error. Check the OperationResponse.DebugMessage.
        /// </summary>
        public const int PluginReportedError = 0x7FFF - 15;

        /// <summary>
        /// (32751) CreateRoom/JoinRoom/Join operation fails if expected plugin does not correspond to loaded one.
        /// </summary>
        public const int PluginMismatch = 0x7FFF - 16;

        /// <summary>
        /// (32750) for join requests. Indicates the current peer already called join and is joined to the room.
        /// </summary>
        public const int JoinFailedPeerAlreadyJoined = 32750; // 0x7FFF - 17,

        /// <summary>
        /// (32749)  for join requests. Indicates the list of InactiveActors already contains an actor with the requested ActorNr or UserId.
        /// </summary>
        public const int JoinFailedFoundInactiveJoiner = 32749; // 0x7FFF - 18,

        /// <summary>
        /// (32748) for join requests. Indicates the list of Actors (active and inactive) did not contain an actor with the requested ActorNr or UserId.
        /// </summary>
        public const int JoinFailedWithRejoinerNotFound = 32748; // 0x7FFF - 19,

        /// <summary>
        /// (32747) for join requests. Note: for future use - Indicates the requested UserId was found in the ExcludedList.
        /// </summary>
        public const int JoinFailedFoundExcludedUserId = 32747; // 0x7FFF - 20,

        /// <summary>
        /// (32746) for join requests. Indicates the list of ActiveActors already contains an actor with the requested ActorNr or UserId.
        /// </summary>
        public const int JoinFailedFoundActiveJoiner = 32746; // 0x7FFF - 21,

        /// <summary>
        /// (32745)  for SetProerties and Raisevent (if flag HttpForward is true) requests. Indicates the maximum allowd http requests per minute was reached.
        /// </summary>
        public const int HttpLimitReached = 32745; // 0x7FFF - 22,

        /// <summary>
        /// (32744) for WebRpc requests. Indicates the the call to the external service failed.
        /// </summary>
        public const int ExternalHttpCallFailed = 32744; // 0x7FFF - 23,

        /// <summary>
        /// (32742) Server error during matchmaking with slot reservation. E.g. the reserved slots can not exceed MaxPlayers.
        /// </summary>
        public const int SlotError = 32742; // 0x7FFF - 25,

        /// <summary>
        /// (32741) Server will react with this error if invalid encryption parameters provided by token
        /// </summary>
        public const int InvalidEncryptionParameters = 32741; // 0x7FFF - 24,
    }

    public static class Rpc
    {
        public const string OnGameLose = "netGameLose";
        public const string OnGameWin = "netGameWin";
        public const string Ignore = "ignorePlayer";
        public const string IgnoreMultiple = "ignorePlayerArray";
        public const string SetMasterclient = "setMasterRC";
        public const string ClearLevel = "clearlevel";
        public const string GetRacingResults = "getRacingResult";
        public const string RefreshRacingResult = "netRefreshRacingResult";
        public const string Settings = "settingRPC";
        public const string UpdateKillInfo = "updateKillInfo";
        public const string CheckPlayerPresence = "verifyPlayerHasLeft";
        public const string SwitchToHuman = "backToHumanRPC";
        public const string ContinueAnimation = "netContinueAnimation";
        public const string SetErenTitanOwner = "whoIsMyErenTitan";
        public const string GasEmission = "net3DMGSMOKE";
        public const string OnPlayerDeath = "someOneIsDead";
        public const string PauseAnimation = "netPauseAnimation";
        public const string ReleasePlayerHook = "badGuyReleaseMe";
        public const string HookFailed = "hookFail";
        public const string SetTeam = "setMyTeam";
        public const string GrabRemove = "netSetIsGrabbedFalse";
        public const string GrabEscape = "grabbedTargetEscape";
        public const string TauntAttack = "netTauntAttack";
        public const string LaughAttack = "netlaughAttack";
        public const string HeadBlow = "dieHeadBlowRPC";
        public const string SetTarget = "setMyTarget";
        public const string SetLookTarget = "setIfLookTarget";
        public const string NeckHitRight = "hitRRPC";
        public const string NeckHitLeft = "hitLRPC";
        public const string SetAbnormalType = "netSetAbnormalType";
        public const string SetLevel = "netSetLevel";
        public const string Initialize = "initRPC";
        public const string HitByFemaleTitan = "hitByFTRPC";
        public const string PlayRockAnimation = "rockPlayAnimation";
        public const string HitByTitan = "hitByTitanRPC";
        public const string StartRockAnimation = "startMovingRock";
        public const string StopRockAnimation = "endMovingRock";
        public const string HorseParticles = "setDust";
        public const string CheckpointChangeState = "changeState";
        public const string TitanOverCheckpoint = "changeTitanPt";
        public const string HumanOverCheckpoint = "changeHumanPt";
        public const string MoveTo = "moveToRPC";
        public const string SpawnPlayerAt = "spawnPlayerAtRPC";
        public const string ThrowRock = "launchRPC";
        public const string HitAnkle = "hitAnkleRPC";
        public const string Laugh = "laugh";
        public const string Explode = "dieBlowRPC";
        public const string CustomLevel = "customlevelRPC";
        public const string netUngrabbed = "netUngrabbed";
        public const string GrabLeft = "grabToLeft";
        public const string GrabRight = "grabToRight";
        public const string Grabbed = "netGrabbed";
        public const string ChangeDoor = "changeDoor";
        public const string StopSweepSmoke = "stopSweepSmoke";
        public const string UpdateHealthLabel = "labelRPC";
        public const string SetSize = "setSize";
        public const string PlaySound = "playsoundRPC";
        public const string PlayAnimationAt = "netPlayAnimationAt";
        public const string PlayAnimation = "netPlayAnimation";
        public const string StartNeckStream = "startNeckSteam";
        public const string LoadSkin = "loadskinRPC";
        public const string CrossFade = "netCrossFade";
        public const string BlowAway = "blowAway";
        public const string StartSweepSmoke = "startSweepSmoke";
        public const string setVelocityAndLeft = "setVelocityAndLeft";
        public const string HookOwner = "myMasterIs";
        public const string SetHookPosition = "tieMeTo";
        public const string SetHookedObject = "tieMeToOBJ";
        public const string SetHookPhase = "setPhase";
        public const string OneTitanDown = "oneTitanDown";
        public const string ShowDamage = "netShowDamage";
        public const string RefreshStatus = "refreshStatus";
        public const string RefreshStatus_PVP = "refreshPVPStatus";
        public const string RefreshStatus_PVP_AHSS = "refreshPVPStatus_AHSS";
        public const string Die = "netDie";
        public const string DieRC = "netDie2";
        public const string TitanGetHit = "titanGetHit";
        public const string RestartByClient = "restartGameByClient";
        public const string HitLeftAnkle = "hitAnkleLRPC";
        public const string HitRightAnkle = "hitAnkleRRPC";
        public const string HitEye = "hitEyeRPC";
        public const string Chat = "Chat";
        public const string Pause = "pauseRPC";
        public const string Respawn = "respawnHeroInNewRound";
        public const string SetHairColor = "setHairPRC";
        public const string SetHairSkin = "setHairRPC2";
        public const string ShowResult = "showResult";
        public const string ReturnFromCannon = "ReturnFromCannon";
        public const string DieByCannon = "DieByCannon";
        public const string SpawnCannon = "SpawnCannonRPC";
        public const string LoadLevel = "RPCLoadLevel";
        public const string RequireStatus = "RequireStatus";
        public const string HookedByHuman = "RPCHookedByHuman";
        public const string SetMyCannon = "SetMyCannon";
        public const string SetMyPhotonCamera = "SetMyPhotonCamera";
        public const string RequestControl = "RequestControlRPC";
        public const string PunRespawn = "PunRespawn";
        public const string PunPickup = "PunPickup";
        public const string RequestForPickupTimes = "RequestForPickupTimes";
        public const string PickupItemInit = "PickupItemInit";
        public const string PunPickupSimple = "PunPickupSimple";
    }
    
    /// <summary>
    /// Class for constants. Codes for parameters of Operations and Events.
    /// Pun uses these constants internally.
    /// </summary>
    public static class ParameterCode
    {
        /// <summary>(237) A bool parameter for creating games. If set to true, no room events are sent to the clients on join and leave. Default: false (and not sent).</summary>
        public const byte SuppressRoomEvents = 237;

        /// <summary>(236) Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds.</summary>
        public const byte EmptyRoomTTL = 236;

        /// <summary>(235) Time To Live (TTL) for an 'actor' in a room. If a client disconnects, this actor is inactive first and removed after this timeout. In milliseconds.</summary>
        public const byte PlayerTTL = 235;

        /// <summary>(234) Optional parameter of OpRaiseEvent and OpSetCustomProperties to forward the event/operation to a web-service.</summary>
        public const byte EventForward = 234;

        /// <summary>(233) Optional parameter of OpLeave in async games. If false, the player does abandons the game (forever). By default players become inactive and can re-join.</summary>
        [Obsolete("Use: IsInactive")]
        public const byte IsComingBack = 233;

        /// <summary>(233) Used in EvLeave to describe if a user is inactive (and might come back) or not. In rooms with PlayerTTL, becoming inactive is the default case.</summary>
        public const byte IsInactive = 233;

        /// <summary>(232) Used when creating rooms to define if any userid can join the room only once.</summary>
        public const byte CheckUserOnJoin = 232;

        /// <summary>(231) Code for "Check And Swap" (CAS) when changing properties.</summary>
        public const byte ExpectedValues = 231;

        /// <summary>(230) Address of a (game) server to use.</summary>
        public const byte Address = 230;

        /// <summary>(229) Count of players in this application in a rooms (used in stats event)</summary>
        public const byte PeerCount = 229;

        /// <summary>(228) Count of games in this application (used in stats event)</summary>
        public const byte GameCount = 228;

        /// <summary>(227) Count of players on the master server (in this app, looking for rooms)</summary>
        public const byte MasterPeerCount = 227;

        /// <summary>(225) User's ID</summary>
        public const byte UserId = 225;

        /// <summary>(224) Your application's ID: a name on your own Photon or a GUID on the Photon Cloud</summary>
        public const byte ApplicationId = 224;

        /// <summary>(223) Not used currently (as "Position"). If you get queued before connect, this is your position</summary>
        public const byte Position = 223;

        /// <summary>(223) Modifies the matchmaking algorithm used for OpJoinRandom. Allowed parameter values are defined in enum MatchmakingMode.</summary>
        public const byte MatchMakingType = 223;

        /// <summary>(222) List of RoomInfos about open / listed rooms</summary>
        public const byte GameList = 222;

        /// <summary>(221) Internally used to establish encryption</summary>
        public const byte Secret = 221;

        /// <summary>(220) Version of your application</summary>
        public const byte AppVersion = 220;

        /// <summary>(210) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureNodeInfo = 210;	// only used within events, so use: EventCode.AzureNodeInfo

        /// <summary>(209) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureLocalNodeId = 209;

        /// <summary>(208) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureMasterNodeId = 208;

        /// <summary>(255) Code for the gameId/roomName (a unique name per room). Used in OpJoin and similar.</summary>
        public const byte RoomName = 255;

        /// <summary>(250) Code for broadcast parameter of OpSetProperties method.</summary>
        public const byte Broadcast = 250;

        /// <summary>(252) Code for list of players in a room. Currently not used.</summary>
        public const byte ActorList = 252;

        /// <summary>(254) Code of the Actor of an operation. Used for property get and set.</summary>
        public const byte ActorNr = 254;

        /// <summary>(249) Code for property set (Hashtable).</summary>
        public const byte PlayerProperties = 249;

        /// <summary>(245) Code of data/custom content of an event. Used in OpRaiseEvent.</summary>
        public const byte CustomEventContent = 245;

        /// <summary>(245) Code of data of an event. Used in OpRaiseEvent.</summary>
        public const byte Data = 245;

        /// <summary>(244) Code used when sending some code-related parameter, like OpRaiseEvent's event-code.</summary>
        /// <remarks>This is not the same as the Operation's code, which is no longer sent as part of the parameter Dictionary in Photon 3.</remarks>
        public const byte Code = 244;

        /// <summary>(248) Code for property set (Hashtable).</summary>
        public const byte GameProperties = 248;

        /// <summary>
        /// (251) Code for property-set (Hashtable). This key is used when sending only one set of properties.
        /// If either ActorProperties or GameProperties are used (or both), check those keys.
        /// </summary>
        public const byte Properties = 251;

        /// <summary>(253) Code of the target Actor of an operation. Used for property set. Is 0 for game</summary>
        public const byte TargetActorNr = 253;

        /// <summary>(246) Code to select the receivers of events (used in Lite, Operation RaiseEvent).</summary>
        public const byte ReceiverGroup = 246;

        /// <summary>(247) Code for caching events while raising them.</summary>
        public const byte Cache = 247;

        /// <summary>(241) Bool parameter of CreateRoom Operation. If true, server cleans up roomcache of leaving players (their cached events get removed).</summary>
        public const byte CleanupCacheOnLeave = 241;

        /// <summary>(240) Code for "group" operation-parameter (as used in Op RaiseEvent).</summary>
        public const byte Group = 240;

        /// <summary>(239) The "Remove" operation-parameter can be used to remove something from a list. E.g. remove groups from player's interest groups.</summary>
        public const byte Remove = 239;

        /// <summary>(239) Used in Op Join to define if UserIds of the players are broadcast in the room. Useful for FindFriends and reserving slots for expected users.</summary>
        public const byte PublishUserId = 239;

        /// <summary>(238) The "Add" operation-parameter can be used to add something to some list or set. E.g. add groups to player's interest groups.</summary>
        public const byte Add = 238;

        /// <summary>(218) Content for EventCode.ErrorInfo and internal debug operations.</summary>
        public const byte Info = 218;

        /// <summary>(217) This key's (byte) value defines the target custom authentication type/service the client connects with. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationType = 217;

        /// <summary>(216) This key's (string) value provides parameters sent to the custom authentication type/service the client connects with. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationParams = 216;

        /// <summary>(215) Makes the server create a room if it doesn't exist. OpJoin uses this to always enter a room, unless it exists and is full/closed.</summary>
        public const byte CreateIfNotExists = 215;

        /// <summary>(215) The JoinMode enum defines which variant of joining a room will be executed: Join only if available, create if not exists or re-join.</summary>
        /// <remarks>Replaces CreateIfNotExists which was only a bool-value.</remarks>
        public const byte JoinMode = 215;

        /// <summary>(214) This key's (string or byte[]) value provides parameters sent to the custom authentication service setup in Photon Dashboard. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationData = 214;

        /// <summary>(203) Code for MasterClientId, which is synced by server. When sent as op-parameter this is code 203.</summary>
        /// <remarks>Tightly related to GamePropertyKey.MasterClientId.</remarks>
        public const byte MasterClientId = 203;

        /// <summary>(1) Used in Op FindFriends request. Value must be string[] of friends to look up.</summary>
        public const byte FindFriendsRequestList = 1;

        /// <summary>(1) Used in Op FindFriends response. Contains bool[] list of online states (false if not online).</summary>
        public const byte FindFriendsResponseOnlineList = 1;

        /// <summary>(2) Used in Op FindFriends response. Contains string[] of room names ("" where not known or no room joined).</summary>
        public const byte FindFriendsResponseRoomIdList = 2;

        /// <summary>(213) Used in matchmaking-related methods and when creating a room to name a lobby (to join or to attach a room to).</summary>
        public const byte LobbyName = 213;

        /// <summary>(212) Used in matchmaking-related methods and when creating a room to define the type of a lobby. Combined with the lobby name this identifies the lobby.</summary>
        public const byte LobbyType = 212;

        /// <summary>(211) This (optional) parameter can be sent in Op Authenticate to turn on Lobby Stats (info about lobby names and their user- and game-counts). See: PhotonNetwork.Lobbies</summary>
        public const byte LobbyStats = 211;

        /// <summary>(210) Used for region values in OpAuth and OpGetRegions.</summary>
        public const byte Region = 210;

        /// <summary>(209) Path of the WebRPC that got called. Also known as "WebRpc Name". Type: string.</summary>
        public const byte UriPath = 209;

        /// <summary>(208) Parameters for a WebRPC as: Dictionary&lt;string, object&gt;. This will get serialized to JSon.</summary>
        public const byte WebRpcParameters = 208;

        /// <summary>(207) ReturnCode for the WebRPC, as sent by the web service (not by Photon, which uses ErrorCode). Type: byte.</summary>
        public const byte WebRpcReturnCode = 207;

        /// <summary>(206) Message returned by WebRPC server. Analog to Photon's debug message. Type: string.</summary>
        public const byte WebRpcReturnMessage = 206;

        /// <summary>(205) Used to define a "slice" for cached events. Slices can easily be removed from cache. Type: int.</summary>
        public const byte CacheSliceIndex = 205;

        /// <summary>(204) Informs the server of the expected plugin setup.</summary>
        /// <remarks>
        /// The operation will fail in case of a plugin mismatch returning error code PluginMismatch 32751(0x7FFF - 16).
        /// Setting string[]{} means the client expects no plugin to be setup.
        /// Note: for backwards compatibility null omits any check.
        /// </remarks>
        public const byte Plugins = 204;

        /// <summary>(202) Used by the server in Operation Responses, when it sends the nickname of the client (the user's nickname).</summary>
        public const byte NickName = 202;

        /// <summary>(201) Informs user about name of plugin load to game</summary>
        public const byte PluginName = 201;

        /// <summary>(200) Informs user about version of plugin load to game</summary>
        public const byte PluginVersion = 200;

        /// <summary>(195) Protocol which will be used by client to connect master/game servers. Used for nameserver.</summary>
        public const byte ExpectedProtocol = 195;

        /// <summary>(194) Set of custom parameters which are sent in auth request.</summary>
        public const byte CustomInitData = 194;

        /// <summary>(193) How are we going to encrypt data.</summary>
        public const byte EncryptionMode = 193;

        /// <summary>(192) Parameter of Authentication, which contains encryption keys (depends on AuthMode and EncryptionMode).</summary>
        public const byte EncryptionData = 192;
    }

    
    /// <summary>
    /// Class for constants. Contains operation codes.
    /// Pun uses these constants internally.
    /// </summary>
    public static class OperationCode
    {
        [Obsolete("Exchanging encrpytion keys is done internally in the lib now. Don't expect this operation-result.")]
        public const byte ExchangeKeysForEncryption = 250;

        /// <summary>(255) Code for OpJoin, to get into a room.</summary>
        public const byte Join = 255;

        /// <summary>(231) Authenticates this peer and connects to a virtual application</summary>
        public const byte AuthenticateOnce = 231;

        /// <summary>(230) Authenticates this peer and connects to a virtual application</summary>
        public const byte Authenticate = 230;

        /// <summary>(229) Joins lobby (on master)</summary>
        public const byte JoinLobby = 229;

        /// <summary>(228) Leaves lobby (on master)</summary>
        public const byte LeaveLobby = 228;

        /// <summary>(227) Creates a game (or fails if name exists)</summary>
        public const byte CreateGame = 227;

        /// <summary>(226) Join game (by name)</summary>
        public const byte JoinGame = 226;

        /// <summary>(225) Joins random game (on master)</summary>
        public const byte JoinRandomGame = 225;

        // public const byte CancelJoinRandom = 224; // obsolete, cause JoinRandom no longer is a "process". now provides result immediately

        /// <summary>(254) Code for OpLeave, to get out of a room.</summary>
        public const byte Leave = 254;

        /// <summary>(253) Raise event (in a room, for other actors/players)</summary>
        public const byte RaiseEvent = 253;

        /// <summary>(252) Set Properties (of room or actor/player)</summary>
        public const byte SetProperties = 252;

        /// <summary>(251) Get Properties</summary>
        public const byte GetProperties = 251;

        /// <summary>(248) Operation code to change interest groups in Rooms (Lite application and extending ones).</summary>
        public const byte ChangeGroups = 248;

        /// <summary>(222) Request the rooms and online status for a list of friends (by name, which should be unique).</summary>
        public const byte FindFriends = 222;

        /// <summary>(221) Request statistics about a specific list of lobbies (their user and game count).</summary>
        public const byte GetLobbyStats = 221;

        /// <summary>(220) Get list of regional servers from a NameServer.</summary>
        public const byte GetRegions = 220;

        /// <summary>(219) WebRpc Operation.</summary>
        public const byte WebRpc = 219;

        /// <summary>(218) Operation to set some server settings. Used with different parameters on various servers.</summary>
        public const byte ServerSettings = 218;
    }
    
    /// <summary>Defines Photon event-codes as used by PUN.</summary>
    public static class PunEvent
    {
        /// <summary>(200)</summary>
        public const byte RPC = 200;
        
        /// <summary>(201)</summary>
        public const byte SendSerialize = 201;
        
        /// <summary>(202)</summary>
        public const byte Instantiation = 202;
        
        /// <summary>(203)</summary>
        public const byte CloseConnection = 203;
        
        /// <summary>(204)</summary>
        public const byte Destroy = 204;
        
        /// <summary>(205)</summary>
        public const byte RemoveCachedRPCs = 205;
        
        /// <summary>(206)</summary>
        public const byte SendSerializeReliable = 206;  // TS: added this but it's not really needed anymore
        
        /// <summary>(207)</summary>
        public const byte DestroyPlayer = 207;  // TS: added to make others remove all GOs of a player
        
        /// <summary>(208)</summary>
        public const byte SetMasterClient = 208;  // TS: added to assign someone master client (overriding the current)
        
        /// <summary>(209)</summary>
        public const byte OwnershipRequest = 209;
        
        /// <summary>(210)</summary>
        public const byte OwnershipTransfer = 210;
        
        /// <summary>(211)</summary>
        public const byte VacantViewIds = 211;
    }
    
    /// <summary>Defines possible values for OpJoinRoom and OpJoinOrCreate. It tells the server if the room can be only be joined normally, created implicitly or found on a web-service for Turnbased games.</summary>
    /// <remarks>These values are not directly used by a game but implicitly set.</remarks>
    public enum JoinMode : byte
    {
        /// <summary>Regular join. The room must exist.</summary>
        Default = 0,

        /// <summary>Join or create the room if it's not existing. Used for OpJoinOrCreate for example.</summary>
        CreateIfNotExists = 1,

        /// <summary>The room might be out of memory and should be loaded (if possible) from a Turnbased web-service.</summary>
        JoinOrRejoin = 2,

        /// <summary>Only re-join will be allowed. If the user is not yet in the room, this will fail.</summary>
        RejoinOnly = 3,
    }
    
    /// <summary>
    /// Options for matchmaking rules for OpJoinRandom.
    /// </summary>
    public enum MatchmakingMode : byte
    {
        /// <summary>Fills up rooms (oldest first) to get players together as fast as possible. Default.</summary>
        /// <remarks>Makes most sense with MaxPlayers > 0 and games that can only start with more players.</remarks>
        FillRoom = 0,

        /// <summary>Distributes players across available rooms sequentially but takes filter into account. Without filter, rooms get players evenly distributed.</summary>
        SerialMatching = 1,

        /// <summary>Joins a (fully) random room. Expected properties must match but aside from this, any available room might be selected.</summary>
        RandomMatching = 2
    }

    /// <summary>
    /// Flags for "types of properties", being used as filter in OpGetProperties.
    /// </summary>
    [Flags]
    public enum PropertyTypeFlag : byte
    {
        /// <summary>(0x00) Flag type for no property type.</summary>
        None = 0x00,

        /// <summary>(0x01) Flag type for game-attached properties.</summary>
        Game = 0x01,

        /// <summary>(0x02) Flag type for actor related propeties.</summary>
        Actor = 0x02,

        /// <summary>(0x01) Flag type for game AND actor properties. Equal to 'Game'</summary>
        GameAndActor = Game | Actor
    }
    
    /// <summary>
    /// Options for optional "Custom Authentication" services used with Photon. Used by OpAuthenticate after connecting to Photon.
    /// </summary>
    public enum CustomAuthenticationType : byte
    {
        /// <summary>Use a custom authentification service. Currently the only implemented option.</summary>
        Custom = 0,

        /// <summary>Authenticates users by their Steam Account. Set auth values accordingly!</summary>
        Steam = 1,

        /// <summary>Authenticates users by their Facebook Account. Set auth values accordingly!</summary>
        Facebook = 2,

        /// <summary>Authenticates users by their Oculus Account and token.</summary>
        Oculus = 3,

        /// <summary>Authenticates users by their PSN Account and token.</summary>
        PlayStation = 4,

        /// <summary>Authenticates users by their Xbox Account and XSTS token.</summary>
        Xbox = 5,

        /// <summary>Disables custom authentification. Same as not providing any AuthenticationValues for connect (more precisely for: OpAuthenticate).</summary>
        None = byte.MaxValue
    }
}