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

    public void OnDestroy()
    {
        if (this.storedHero != null)
        {
            this.storedHero.myCannonRegion = null;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        GameObject gameObject = collider.transform.root.gameObject;
        if (gameObject.layer == 8 && gameObject.GetPhotonView().isMine)
        {
            HERO component = gameObject.GetComponent<HERO>();
            if (component != null && !component.isCannon)
            {
                if (component.myCannonRegion != null)
                {
                    component.myCannonRegion.storedHero = null;
                }
                component.myCannonRegion = this;
                this.storedHero = component;
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        GameObject gameObject = collider.transform.root.gameObject;
        if (gameObject.layer == 8 && gameObject.GetPhotonView().isMine)
        {
            HERO component = gameObject.GetComponent<HERO>();
            if (component != null && this.storedHero != null && component == this.storedHero)
            {
                component.myCannonRegion = null;
                this.storedHero = null;
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    public void RequestControlRPC(int viewID, PhotonMessageInfo info)
    {
        if (!(!photonView.isMine || !PhotonNetwork.isMasterClient || this.disabled))
        {
            HERO component = PhotonView.Find(viewID).gameObject.GetComponent<HERO>();
            if (component != null && component.photonView.owner == info.sender && !GameManager.instance.allowedToCannon.ContainsKey(info.sender.ID))
            {
                this.disabled = true;
                StartCoroutine(this.WaitAndEnable());
                GameManager.instance.allowedToCannon.Add(info.sender.ID, new CannonValues(photonView.viewID, this.settings));
                component.photonView.RPC(Rpc.SpawnCannon, info.sender, new object[] { this.settings });
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
            if (!strArray[2].EqualsIgnoreCase("default"))
            {
                if (strArray[2].EqualsIgnoreCase("transparent"))
                {
                    foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
                    {
                        renderer.material = (Material) GameManager.RCassets.Load("transparent");
                        if (Convert.ToSingle(strArray[10]) != 1f || Convert.ToSingle(strArray[11]) != 1f)
                        {
                            renderer.material.mainTextureScale = new Vector2(renderer.material.mainTextureScale.x * Convert.ToSingle(strArray[10]), renderer.material.mainTextureScale.y * Convert.ToSingle(strArray[11]));
                        }
                    }
                }
                else
                {
                    foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
                    {
                        renderer.material = (Material) GameManager.RCassets.Load(strArray[2]);
                        if (Convert.ToSingle(strArray[10]) != 1f || Convert.ToSingle(strArray[11]) != 1f)
                        {
                            renderer.material.mainTextureScale = new Vector2(
                                renderer.material.mainTextureScale.x * Convert.ToSingle(strArray[10]),
                                renderer.material.mainTextureScale.y * Convert.ToSingle(strArray[11]));
                        }
                    }
                }
            }

            var scale = gameObject.transform.localScale;
            float x = scale.x * Convert.ToSingle(strArray[3]) - 0.001f;
            float y = scale.y * Convert.ToSingle(strArray[4]);
            float z = scale.z * Convert.ToSingle(strArray[5]);
            gameObject.transform.localScale = new Vector3(x, y, z);
            if (strArray[6] != "0")
            {
                Color color = new Color(Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), 1f);
                foreach (MeshFilter filter in gameObject.GetComponentsInChildren<MeshFilter>())
                {
                    Mesh mesh = filter.mesh;
                    Color[] colorArray = new Color[mesh.vertexCount];
                    for (int i = 0; i < mesh.vertexCount; i++)
                    {
                        colorArray[i] = color;
                    }
                    mesh.colors = colorArray;
                }
            }
        }
    }

    public void Start()
    {
        if (false) //TODO: Probably broken || was `(int) settings[64] >= 100`
        {
            GetComponent<Collider>().enabled = false;
        }
    }

    public IEnumerator WaitAndEnable()
    {
        yield return new WaitForSeconds(5f);
        if (!this.destroyed)
        {
            this.disabled = false;
        }
    }
}