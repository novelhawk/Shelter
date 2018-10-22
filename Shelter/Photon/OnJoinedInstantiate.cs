using JetBrains.Annotations;
using Photon;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class OnJoinedInstantiate : MonoBehaviour
{
    public float PositionOffset = 2f;
    public GameObject[] PrefabsToInstantiate;
    public Transform SpawnPosition;

    [UsedImplicitly]
    public void OnJoinedRoom()
    {
        if (this.PrefabsToInstantiate != null)
        {
            foreach (GameObject obj2 in this.PrefabsToInstantiate)
            {
                Debug.Log("Instantiating: " + obj2.name);
                Vector3 up = Vector3.up;
                if (this.SpawnPosition != null)
                {
                    up = this.SpawnPosition.position;
                }
                Vector3 insideUnitSphere = Random.insideUnitSphere;
                insideUnitSphere.y = 0f;
                insideUnitSphere = insideUnitSphere.normalized;
                Vector3 position = up + this.PositionOffset * insideUnitSphere;
                PhotonNetwork.Instantiate(obj2.name, position, Quaternion.identity, 0);
            }
        }
    }
}
