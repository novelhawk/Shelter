namespace Photon.Enums
{
    /// <summary>Available server (types) for internally used field: server.</summary>
    /// <remarks>Photon uses 3 different roles of servers: Name Server, Master Server and Game Server.</remarks>
    public enum ServerConnection
    {
        /// <summary>This server is where matchmaking gets done and where clients can get lists of rooms in lobbies.</summary>
        MasterServer,
        /// <summary>This server handles a number of rooms to execute and relay the messages between players (in a room).</summary>
        GameServer,
        /// <summary>This server is used initially to get the address (IP) of a Master Server for a specific region. Not used for Photon OnPremise (self hosted).</summary>
        NameServer
    }
}

