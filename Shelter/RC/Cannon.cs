using System;
using JetBrains.Annotations;
using Mod;
using Mod.Exceptions;
using Mod.Keybinds;
using Photon;
using Photon.Enums;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class Cannon : Photon.MonoBehaviour
{
    public Transform ballPoint;
    public Transform barrel;
    private Quaternion correctBarrelRot = Quaternion.identity;
    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    public float currentRot = 0f;
    public Transform firingPoint;
    public bool isCannonGround;
    public GameObject myCannonBall;
    public LineRenderer myCannonLine;
    public HERO myHero;
    public string settings;
    public float SmoothingDelay = 5f;

    public void Awake()
    {
        if (photonView != null)
        {
            photonView.observed = this;
            this.barrel = transform.Find("Barrel");
            this.correctPlayerPos = transform.position;
            this.correctPlayerRot = transform.rotation;
            this.correctBarrelRot = this.barrel.rotation;
            if (photonView.isMine)
            {
                this.firingPoint = this.barrel.Find("FiringPoint");
                this.ballPoint = this.barrel.Find("BallPoint");
                this.myCannonLine = this.ballPoint.GetComponent<LineRenderer>();
                if (gameObject.name.Contains("CannonGround"))
                {
                    this.isCannonGround = true;
                }
            }
            if (PhotonNetwork.isMasterClient)
            {
                Player owner = photonView.owner;
                if (GameManager.instance.allowedToCannon.ContainsKey(owner.ID))
                {
                    this.settings = GameManager.instance.allowedToCannon[owner.ID].settings;
                    photonView.RPC(Rpc.SetSize, PhotonTargets.All, this.settings);
                    int viewID = GameManager.instance.allowedToCannon[owner.ID].viewID;
                    GameManager.instance.allowedToCannon.Remove(owner.ID);
                    CannonPropRegion component = PhotonView.Find(viewID).gameObject.GetComponent<CannonPropRegion>();
                    if (component != null)
                    {
                        component.disabled = true;
                        component.destroyed = true;
                        PhotonNetwork.Destroy(component.gameObject);
                    }
                }
                else if (!(owner.IsLocal || GameManager.instance.restartingMC))
                {
                    GameManager.instance.KickPlayerRC(owner, false, "spawning cannon without request.");
                }
            }
        }
    }

    public void CannonFire()
    {
        if (this.myHero.skillCDDuration <= 0f)
        {
            foreach (EnemyCheckCollider c in PhotonNetwork.Instantiate("FX/boom2", this.firingPoint.position, this.firingPoint.rotation, 0).GetComponentsInChildren<EnemyCheckCollider>())
                c.dmg = 0;
            this.myCannonBall = PhotonNetwork.Instantiate("RCAsset/CannonBallObject", this.ballPoint.position, this.firingPoint.rotation, 0);
            this.myCannonBall.rigidbody.velocity = this.firingPoint.forward * 300f;
            this.myCannonBall.GetComponent<CannonBall>().myHero = this.myHero;
            this.myHero.skillCDDuration = 3.5f;
        }
    }

    public void OnDestroy()
    {
        if (PhotonNetwork.isMasterClient && !GameManager.instance.isRestarting)
        {
            string[] strArray = this.settings.Split(',');
            if (strArray[0] == "photon")
            {
                if (strArray.Length > 15)
                {
                    GameObject go = PhotonNetwork.Instantiate("RCAsset/" + strArray[1] + "Prop", new Vector3(float.Parse(strArray[12]), float.Parse(strArray[13]), float.Parse(strArray[14])), new Quaternion(float.Parse(strArray[15]), float.Parse(strArray[16]), float.Parse(strArray[17]), float.Parse(strArray[18])), 0);
                    go.GetComponent<CannonPropRegion>().settings = this.settings;
                    go.GetPhotonView().RPC(Rpc.SetSize, PhotonTargets.AllBuffered, this.settings);
                }
                else
                {
                    PhotonNetwork.Instantiate("RCAsset/" + strArray[1] + "Prop", new Vector3(float.Parse(strArray[2]), float.Parse(strArray[3]), float.Parse(strArray[4])), new Quaternion(float.Parse(strArray[5]), float.Parse(strArray[6]), float.Parse(strArray[7]), float.Parse(strArray[8])), 0).GetComponent<CannonPropRegion>().settings = this.settings;
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(this.barrel.rotation);
        }
        else
        {
            this.correctPlayerPos = (Vector3) stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion) stream.ReceiveNext();
            this.correctBarrelRot = (Quaternion) stream.ReceiveNext();
        }
    }

    [RPC]
    [UsedImplicitly]
    public void SetSize(string settings, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient) 
            throw new NotAllowedException(nameof(SetSize), info);
        
        string[] strArray = settings.Split(',');
        if (strArray.Length > 15)
        {
            float a = 1f;
            GameObject gameObject;
            gameObject = this.gameObject;
            if (strArray[2] != "default")
            {
                if (strArray[2].EqualsIgnoreCase("transparent"))
                {
                    if (float.TryParse(strArray[2].Substring(11), out var num2))
                    {
                        a = num2;
                    }
                    foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
                    {
                        renderer.material = (Material) GameManager.RCassets.Load("transparent");
                        if (float.Parse(strArray[10]) != 1f || float.Parse(strArray[11]) != 1f)
                        {
                            renderer.material.mainTextureScale = new Vector2(renderer.material.mainTextureScale.x * float.Parse(strArray[10]), renderer.material.mainTextureScale.y * float.Parse(strArray[11]));
                        }
                    }
                }
                else
                {
                    foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
                    {
                        if (!renderer.name.Contains("Line Renderer"))
                        {
                            renderer.material = (Material) GameManager.RCassets.Load(strArray[2]);
                            if (float.Parse(strArray[10]) != 1f || float.Parse(strArray[11]) != 1f)
                            {
                                renderer.material.mainTextureScale = new Vector2(renderer.material.mainTextureScale.x * float.Parse(strArray[10]), renderer.material.mainTextureScale.y * float.Parse(strArray[11]));
                            }
                        }
                    }
                }
            }

            Vector3 localScale = gameObject.transform.localScale;
            gameObject.transform.localScale = new Vector3(
                localScale.x * float.Parse(strArray[3]) - 0.001f,
                localScale.y * float.Parse(strArray[4]), 
                localScale.z * float.Parse(strArray[5]));
            if (strArray[6] != "0")
            {
                Color color = new Color(float.Parse(strArray[7]), float.Parse(strArray[8]), float.Parse(strArray[9]), a);
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

    public void Update()
    {
        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
            this.barrel.rotation = Quaternion.Lerp(this.barrel.rotation, this.correctBarrelRot, Time.deltaTime * this.SmoothingDelay);
            return;
        }

        Vector3 vector = new Vector3(0f, -30f, 0f);
        Vector3 position = this.ballPoint.position;
        Vector3 vector3 = this.ballPoint.forward * 300f;
        float num = 40f / vector3.magnitude;
        this.myCannonLine.SetWidth(0.5f, 40f);
        this.myCannonLine.SetVertexCount(100);
        for (int i = 0; i < 100; i++)
        {
            this.myCannonLine.SetPosition(i, position);
            position += vector3 * num + 0.5f * vector * num * num;
            vector3 += vector * num;
        }
        const float speed = 20f;
        if (this.isCannonGround)
        {
            if (Shelter.InputManager.IsKeyPressed(InputAction.Forward))
            {
                if (this.currentRot <= 32f)
                {
                    this.currentRot += Time.deltaTime * speed;
                    this.barrel.Rotate(new Vector3(0f, 0f, Time.deltaTime * speed));
                }
            }
            else if (Shelter.InputManager.IsKeyPressed(InputAction.Back) && this.currentRot >= -18f)
            {
                this.currentRot += Time.deltaTime * -speed;
                this.barrel.Rotate(new Vector3(0f, 0f, Time.deltaTime * -speed));
            }
        }
        else
        {
            if (Shelter.InputManager.IsKeyPressed(InputAction.Forward))
            {
                if (this.currentRot >= -50f)
                {
                    this.currentRot += Time.deltaTime * -speed;
                    this.barrel.Rotate(new Vector3(Time.deltaTime * -speed, 0f, 0f));
                }
            }
            else if (Shelter.InputManager.IsKeyPressed(InputAction.Back) && this.currentRot <= 40f)
            {
                this.currentRot += Time.deltaTime * speed;
                this.barrel.Rotate(new Vector3(Time.deltaTime * speed, 0f, 0f));
            }
        }
        if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
            transform.Rotate(new Vector3(0f, Time.deltaTime * -speed, 0f));
        else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
            transform.Rotate(new Vector3(0f, Time.deltaTime * speed, 0f));
        
        if (Shelter.InputManager.IsDown(InputAction.Attack))
        {
            CannonFire();
        }
        else if (Shelter.InputManager.IsDown(InputAction.EnterCannon))
        {
            if (myHero != null)
                myHero.ExitCannon();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

