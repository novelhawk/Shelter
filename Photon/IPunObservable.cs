namespace Photon
{
    public interface IPunObservable
    {
        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);
    }
}

