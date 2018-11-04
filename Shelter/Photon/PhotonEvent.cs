using Mod;
using Photon.Enums;

namespace Photon
{
    public enum PhotonEvent : byte
    {
        ModIdentify = 101,
        RPC = PunEvent.RPC,
        SendSerialize = PunEvent.SendSerialize,
        SendSerializeReliable = PunEvent.SendSerializeReliable,
        Instantiation = PunEvent.Instantiation,
        CloseConnection = PunEvent.CloseConnection,
        Destroy = PunEvent.Destroy,
        RemoveCachedRPCs = PunEvent.RemoveCachedRPCs,
        DestroyPlayer = PunEvent.DestroyPlayer,
        SetMasterClient = PunEvent.SetMasterClient,
        OwnershipRequest = PunEvent.OwnershipRequest,
        OwnershipTransfer = PunEvent.OwnershipTransfer,
        VacantViewIds = PunEvent.VacantViewIds,
        AppStats = EventCode.AppStats,
        QueueState = EventCode.QueueState,
        Match = EventCode.Match,
        LobbyStats = EventCode.LobbyStats,
        ErrorInfo = EventCode.ErrorInfo,
        CacheSliceChanged = EventCode.CacheSliceChanged,
        AuthEvent = EventCode.AuthEvent,
        GameList = EventCode.GameList,
        GameListUpdate = EventCode.GameListUpdate,
        PropertiesChanged = EventCode.PropertiesChanged,
        Leave = EventCode.Leave,
        Join = EventCode.Join,
    }
}