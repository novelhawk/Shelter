using ExitGames.Client.Photon;

namespace Photon
{
    /// <summary>
    /// Summarizes the cause for a disconnect. Used in: OnConnectionFail and OnFailedToConnectToPhoton.
    /// </summary>
    /// <remarks>Extracted from the status codes from ExitGames.Client.Photon.StatusCode.</remarks>
    /// <seealso cref="PhotonNetworkingMessage"/>
    /// \ingroup publicApi
    public enum DisconnectCause
    {
        /// <summary>Server actively disconnected this client.
        /// Possible cause: The server's user limit was hit and client was forced to disconnect (on connect).</summary>
        DisconnectByServerUserLimit = StatusCode.DisconnectByServerUserLimit,
    
        /// <summary>Connection could not be established.
        /// Possible cause: Local server not running.</summary>
        ExceptionOnConnect = StatusCode.ExceptionOnConnect,
    
        /// <summary>Timeout disconnect by server (which decided an ACK was missing for too long).</summary>
        DisconnectByServerTimeout = StatusCode.DisconnectByServer,
    
        /// <summary>Server actively disconnected this client.
        /// Possible cause: Server's send buffer full (too much data for client).</summary>
        DisconnectByServerLogic = StatusCode.DisconnectByServerLogic,
    
        /// <summary>Some exception caused the connection to close.</summary>
        Exception = StatusCode.Exception,
    
        /// <summary>(32767) The Photon Cloud rejected the sent AppId. Check your Dashboard and make sure the AppId you use is complete and correct.</summary>
        InvalidAuthentication = ErrorCode.InvalidAuthentication,
    
        /// <summary>(32757) Authorization on the Photon Cloud failed because the concurrent users (CCU) limit of the app's subscription is reached.</summary>
        MaxCcuReached = ErrorCode.MaxCcuReached,
    
        /// <summary>(32756) Authorization on the Photon Cloud failed because the app's subscription does not allow to use a particular region's server.</summary>
        InvalidRegion = ErrorCode.InvalidRegion,
    
        /// <summary>The security settings for client or server don't allow a connection (see remarks).</summary>
        /// <remarks>
        /// A common cause for this is that browser clients read a "crossdomain" file from the server.
        /// If that file is unavailable or not configured to let the client connect, this exception is thrown.
        /// Photon usually provides this crossdomain file for Unity.
        /// If it fails, read:
        /// https://doc.photonengine.com/en-us/onpremise/current/operations/policy-files
        /// </remarks>
        SecurityExceptionOnConnect = StatusCode.SecurityExceptionOnConnect,
    
        /// <summary>Timeout disconnect by client (which decided an ACK was missing for too long).</summary>
        DisconnectByClientTimeout = StatusCode.TimeoutDisconnect,
    
        /// <summary>Exception in the receive-loop.
        /// Possible cause: Socket failure.</summary>
        InternalReceiveException = StatusCode.ExceptionOnReceive,
    
        /// <summary>(32753) The Authentication ticket expired. Handle this by connecting again (which includes an authenticate to get a fresh ticket).</summary>
        AuthenticationTicketExpired = 32753,
    }
}

