// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkingPeer.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking (PUN)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon
{
    /// <summary>
    /// Detailed connection / networking peer state.
    /// PUN implements a loadbalancing and authentication workflow "behind the scenes", so
    /// some states will automatically advance to some follow up state. Those states are
    /// commented with "(will-change)".
    /// </summary>
    public enum ClientState
    {
        /// <summary>Not running. Only set before initialization and first use.</summary>
        Uninitialized,
    
        /// <summary>Created and available to connect.</summary>
        PeerCreated,
    
        /// <summary>Not used at the moment.</summary>
        Queued,
    
        /// <summary>The application is authenticated. PUN usually joins the lobby now.</summary>
        /// <remarks>(will-change) Unless AutoJoinLobby is false.</remarks>
        Authenticated,
    
        /// <summary>Client is in the lobby of the Master Server and gets room listings.</summary>
        /// <remarks>Use Join, Create or JoinRandom to get into a room to play.</remarks>
        JoinedLobby,
    
        /// <summary>Disconnecting.</summary>
        /// <remarks>(will-change)</remarks>
        DisconnectingFromMasterserver,
    
        /// <summary>Connecting to game server (to join/create a room and play).</summary>
        /// <remarks>(will-change)</remarks>
        ConnectingToGameserver,
    
        /// <summary>Similar to Connected state but on game server. Still in process to join/create room.</summary>
        /// <remarks>(will-change)</remarks>
        ConnectedToGameserver,
    
        /// <summary>In process to join/create room (on game server).</summary>
        /// <remarks>(will-change)</remarks>
        Joining,
    
        /// <summary>Final state of a room join/create sequence. This client can now exchange events / call RPCs with other clients.</summary>
        Joined,
    
        /// <summary>Leaving a room.</summary>
        /// <remarks>(will-change)</remarks>
        Leaving,
    
        /// <summary>Workflow is leaving the game server and will re-connect to the master server.</summary>
        /// <remarks>(will-change)</remarks>
        DisconnectingFromGameserver,
    
        /// <summary>Workflow is connected to master server and will establish encryption and authenticate your app.</summary>
        /// <remarks>(will-change)</remarks>
        ConnectingToMasterserver,
    
        /// <summary>Same Queued but coming from game server.</summary>
        /// <remarks>(will-change)</remarks>
        QueuedComingFromGameserver,
    
        /// <summary>PUN is disconnecting. This leads to Disconnected.</summary>
        /// <remarks>(will-change)</remarks>
        Disconnecting,
    
        /// <summary>No connection is setup, ready to connect. Similar to PeerCreated.</summary>
        Disconnected,
    
        /// <summary>Final state for connecting to master without joining the lobby (AutoJoinLobby is false).</summary>
        ConnectedToMaster,
    
        /// <summary>Client connects to the NameServer. This process includes low level connecting and setting up encryption. When done, state becomes ConnectedToNameServer.</summary>
        ConnectingToNameServer,
    
        /// <summary>Client is connected to the NameServer and established enctryption already. You should call OpGetRegions or ConnectToRegionMaster.</summary>
        ConnectedToNameServer,
    
        /// <summary>When disconnecting from a Photon NameServer.</summary>
        /// <remarks>(will-change)</remarks>
        DisconnectingFromNameServer,
    
        /// <summary>When connecting to a Photon Server, this state is intermediate before you can call any operations.</summary>
        /// <remarks>(will-change)</remarks>
        Authenticating
    }
}