using System;
using System.Collections;
using JetBrains.Annotations;
using Mod;
using Photon;
using RC;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class CannonPropRegion : Photon.MonoBehaviour
{
    public bool destroyed;
    public bool disabled;
    public string settings;
    public HERO storedHero;

//    public void OnDestroy()
//    {
//        if (this.storedHero != null)
//        {
//            this.storedHero.myCannonRegion = null;
//        }
//    }

//    public void OnTriggerEnter(Collider c)
//    {
//        GameObject go = c.transform.root.gameObject;
//        if (go.layer == 8 && go.GetPhotonView().isMine)
//        {
//            HERO hero = go.GetComponent<HERO>();
//            if (hero != null && !hero.isCannon)
//            {
//                if (hero.myCannonRegion != null)
//                {
//                    hero.myCannonRegion.storedHero = null;
//                }
//                hero.myCannonRegion = this;
//                this.storedHero = hero;
//            }
//        }
//    }

//    public void OnTriggerExit(Collider c)
//    {
//        GameObject go = c.transform.root.gameObject;
//        if (go.layer == 8 && go.GetPhotonView().isMine)
//        {
//            HERO hero = go.GetComponent<HERO>();
//            if (hero != null && this.storedHero != null && hero == this.storedHero)
//            {
//                hero.myCannonRegion = null;
//                this.storedHero = null;
//            }
//        }
//    }

    [RPC]
    [UsedImplicitly]
    public void RequestControlRPC(int viewID, PhotonMessageInfo info) //TODO: Check for viewID validity
    {
        if (!(!photonView.isMine || !PhotonNetwork.isMasterClient || this.disabled))
        {
            HERO component = PhotonView.Find(viewID).gameObject.GetComponent<HERO>();
            if (component != null && component.photonView.owner == info.sender && !GameManager.instance.allowedToCannon.ContainsKey(info.sender.ID))
            {
                this.disabled = true;
                StartCoroutine(this.WaitAndEnable());
                GameManager.instance.allowedToCannon.Add(info.sender.ID, new CannonValues(photonView.viewID, this.settings));
                component.photonView.RPC(Rpc.SpawnCannon, info.sender, this.settings);
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    public void SetSize(string settings, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient) 
            return;
        
        string[] strArray = settings.Split(',');
        if (strArray.Length > 15)
        {
            if (!string.Equals(strArray[2], "default", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(strArray[2], "transparent", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Renderer render in gameObject.GetComponentsInChildren<Renderer>())
                    {
                        render.material = (Material) GameManager.RCassets.Load("transparent");
                        if (float.Parse(strArray[10]) != 1f || float.Parse(strArray[11]) != 1f)
                        {
                            render.material.mainTextureScale = new Vector2(
                                render.material.mainTextureScale.x * float.Parse(strArray[10]), 
                                render.material.mainTextureScale.y * float.Parse(strArray[11]));
                        }
                    }
                }
                else
                {
                    foreach (Renderer render in gameObject.GetComponentsInChildren<Renderer>())
                    {
                        render.material = (Material) GameManager.RCassets.Load(strArray[2]);
                        if (float.Parse(strArray[10]) != 1f || float.Parse(strArray[11]) != 1f)
                        {
                            render.material.mainTextureScale = new Vector2(
                                render.material.mainTextureScale.x * float.Parse(strArray[10]),
                                render.material.mainTextureScale.y * float.Parse(strArray[11]));
                        }
                    }
                }
            }

            var scale = gameObject.transform.localScale;
            float x = scale.x * float.Parse(strArray[3]) - 0.001f;
            float y = scale.y * float.Parse(strArray[3]);
            float z = scale.z * float.Parse(strArray[3]);
            gameObject.transform.localScale = new Vector3(x, y, z);
            if (strArray[6] != "0")
            {
                Color color = new Color(
                    float.Parse(strArray[7]), 
                    float.Parse(strArray[8]), 
                    float.Parse(strArray[9]), 
                    1f);
                foreach (MeshFilter filter in gameObject.GetComponentsInChildren<MeshFilter>())
                {
                    Mesh mesh = filter.mesh;
                    Color[] colorArray = new Color[mesh.vertexCount];
                    for (int i = 0; i < mesh.vertexCount; i++)
                        colorArray[i] = color;
                    mesh.colors = colorArray;
                }
            }
        }
    }

    public void Start()
    {
        Destroy(GetComponent<Collider>());
        if (false) //TODO: Probably broken || was `(int) settings[64] >= 100`
        {
            GetComponent<Collider>().enabled = false;
        }
    }

    private IEnumerator WaitAndEnable()
    {
        yield return new WaitForSeconds(5f);
        if (!destroyed)
            disabled = false;
    }
}