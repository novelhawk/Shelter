namespace Photon.Enums
{
    /// <summary>
    /// High level connection state of the client. Better use the more detailed <see cref="ClientState"/>.
    /// </summary>
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Disconnecting,
        InitializingApplication
    }
}

