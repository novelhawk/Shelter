public class RCGeneralEffect : Photon.MonoBehaviour
{
    private void Awake()
    {
        UnityEngine.Object.Destroy(base.gameObject, 1.5f);
    }
}

