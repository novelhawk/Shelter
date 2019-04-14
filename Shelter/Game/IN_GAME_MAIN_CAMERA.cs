using Game;
using Game.Enums;
using Mod;
using Mod.Keybinds;
using Mod.Managers;
using Mod.Modules;
using Photon.Enums;
using UnityEngine;
using Extensions = Photon.Extensions;
using MonoBehaviour = UnityEngine.MonoBehaviour;

// ReSharper disable once CheckNamespace
public class IN_GAME_MAIN_CAMERA : MonoBehaviour
{
    public static IN_GAME_MAIN_CAMERA instance;
//    public RotationAxes axes;
    public AudioSource bgmusic;
    public static float cameraDistance = 0.6f;
    public static CameraType cameraMode;
    private float closestDistance;
    private int currentPeekPlayerIndex;
    public static Daylight Daylight = Daylight.Dawn;
    private float decay;
    public static int difficulty;
    private float distance = 10f;
    private float distanceMulti;
    private float distanceOffsetMulti;
    private float duration;
    private float flashDuration;
    private bool flip;
    public static GameMode GameMode;
    public bool gameOver;
    public static GameType GameType = GameType.NotInRoom;
    private bool hasSnapShot;
    private Transform head;
    public float height = 5f;
    public float heightDamping = 2f;
    private float heightMulti;
    public static bool isPausing;
    public static bool isTyping;
    public float justHit;
    public int lastScore;
    private bool lockAngle;
    private Vector3 lockCameraPosition;
    private GameObject locker;
    private GameObject lockTarget;
    public GameObject main_object;
    public float maximumX = 360f;
    public float maximumY = 60f;
    public float minimumX = -360f;
    public float minimumY = -60f;
    private bool needSetHUD;
    private float R;
    public float rotationY;
    public int score;
    public static float sensitivityMulti = 0.5f;
    public static string singleCharacter;
    public Material skyBoxDAWN;
    public Material skyBoxDAY;
    public Material skyBoxNIGHT;
    private Texture2D snapshot1;
    private Texture2D snapshot2;
    private Texture2D snapshot3;
    public GameObject snapShotCamera;
    private int snapShotCount;
    private float snapShotCountDown;
    private int snapShotDmg;
    private float snapShotInterval = 0.02f;
    public RenderTexture snapshotRT;
    private float snapShotStartCountDownTime;
    private GameObject snapShotTarget;
    private Vector3 snapShotTargetPosition;
    public bool spectatorMode;
    private bool startSnapShotFrameCount;
    public Transform target;
    public Texture texture;
    public float timer;
    public static bool triggerAutoLock;
    public static bool usingTitan;
    private Vector3 verticalHeightOffset = Vector3.zero;
    public float verticalRotationOffset;
    public float xSpeed = -3f;
    public float ySpeed = -0.8f;

    private void Awake()
    {
        isTyping = false;
        isPausing = false;
        name = "MainCamera";
        GetComponent<TiltShift>().enabled = ModuleManager.Enabled(nameof(ModuleTiltShift));
    }

    private void OnApplicationFocus(bool hasFocus) //TODO: Stop mouse movement while not focussed
    {
        Screen.showCursor = !hasFocus;
    }

    private void CameraMovement()
    {
        distanceOffsetMulti = cameraDistance * (200f - camera.fieldOfView) / 150f;
        transform.position = this.head == null ? this.main_object.transform.position : this.head.transform.position;
        transform.position += Vector3.up * this.heightMulti;
        transform.position -= Vector3.up * (0.6f - cameraDistance) * 2f;

        if (Screen.showCursor)
        {
            transform.position -= this.transform.forward * this.distance * this.distanceMulti * this.distanceOffsetMulti;
            if (cameraDistance < 0.65f)
                transform.position += this.transform.right * Mathf.Max((0.6f - cameraDistance) * 2f, 0.65f);
            return;
        }

        if (cameraMode == CameraType.Stop)
        {
            if (Input.GetKey(KeyCode.Mouse2))
            {
                float angle = Input.GetAxis("Mouse X") * 10f * this.getSensitivityMulti();
                float num2 = -Input.GetAxis("Mouse Y") * 10f * this.getSensitivityMulti();
                this.transform.RotateAround(this.transform.position, Vector3.up, angle);
                this.transform.RotateAround(this.transform.position, this.transform.right, num2);
            }
            transform.position -= this.transform.forward * this.distance * this.distanceMulti * this.distanceOffsetMulti;
        }
        else if (cameraMode == CameraType.Original)
        {
            if (Input.mousePosition.x < Screen.width * 0.4f)
            {
                float num3 = -((Screen.width * 0.4f - Input.mousePosition.x) / Screen.width * 0.4f) * this.getSensitivityMultiWithDeltaTime() * 150f;
                this.transform.RotateAround(this.transform.position, Vector3.up, num3);
            }
            else if (Input.mousePosition.x > Screen.width * 0.6f)
            {
                float num3 = (Input.mousePosition.x - Screen.width * 0.6f) / Screen.width * 0.4f * this.getSensitivityMultiWithDeltaTime() * 150f;
                this.transform.RotateAround(this.transform.position, Vector3.up, num3);
            }
            float x = 140f * (Screen.height * 0.6f - Input.mousePosition.y) / Screen.height * 0.5f;
            this.transform.rotation = Quaternion.Euler(x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
            transform.position -= this.transform.forward * this.distance * this.distanceMulti * this.distanceOffsetMulti;
        }
        else if (cameraMode == CameraType.TPS)
        {
            Screen.lockCursor = true;
            float num5 = Input.GetAxis("Mouse X") * 10f * this.getSensitivityMulti();
            float num6 = -Input.GetAxis("Mouse Y") * 10f * this.getSensitivityMulti();
            this.transform.RotateAround(this.transform.position, Vector3.up, num5);
            float num7 = this.transform.rotation.eulerAngles.x % 360f;
            float num8 = num7 + num6;
            if ((num6 <= 0f || (num7 >= 260f || num8 <= 260f) && (num7 >= 80f || num8 <= 80f)) && (num6 >= 0f || (num7 <= 280f || num8 >= 280f) && (num7 <= 100f || num8 >= 100f)))
            {
                this.transform.RotateAround(this.transform.position, this.transform.right, num6);
            }
            transform.position -= this.transform.forward * this.distance * this.distanceMulti * this.distanceOffsetMulti;
        }
        if (cameraDistance < 0.65f)
            transform.position += this.transform.right * Mathf.Max((0.6f - cameraDistance) * 2f, 0.65f);
    }

    // ReSharper disable once UnusedMember.Global
    /// <summary>
    /// Changes the FOV of the camera
    /// </summary>
    /// <param name="hero"></param>
    public void CameraMovementLive(HERO hero) // TODO: Look what that did, and re-implement it
    {
        float magnitude = hero.rigidbody.velocity.magnitude;
        if (magnitude > 10f)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, Mathf.Min(100f, magnitude + 40f), 0.1f);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 50f, 0.1f);
        }
        float num2 = HERO.CameraMultiplier * (200f - Camera.main.fieldOfView) / 150f;
        Transform t = this.transform;
        t.position = this.head.transform.position + Vector3.up * this.heightMulti - Vector3.up * (0.6f - cameraDistance) * 2f;
        t.position -= t.forward * this.distance * this.distanceMulti * num2;
        if (HERO.CameraMultiplier < 0.65f)
        {
            Transform transform2 = this.transform;
            transform2.position += this.transform.right * Mathf.Max((0.6f - HERO.CameraMultiplier) * 2f, 0.65f);
        }
        this.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, hero.GetComponent<SmoothSyncMovement>().correctCameraRot, Time.deltaTime * 5f);
    }

    public void CreateMinimap()
    {
        if (!ModuleManager.Enabled(nameof(ModuleShowMap)) || !GameManager.settings.IsMapAllowed)
            return;
        
        LevelInfo info = LevelInfoManager.Get(GameManager.Level);
        Minimap minimap = gameObject.AddComponent<Minimap>();
        if (minimap.myCam == null)
        {
            minimap.myCam = new GameObject().AddComponent<Camera>();
            minimap.myCam.nearClipPlane = 0.3f;
            minimap.myCam.farClipPlane = 1000f;
            minimap.myCam.enabled = false;
        }
        minimap.CreateMinimap(minimap.myCam, 512, 0.3f, info.MinimapPreset);
    }

    private void TakeSnapshotRT()
    {
        if (this.snapshotRT != null)
        {
            this.snapshotRT.Release();
        }
        if (QualitySettings.GetQualityLevel() > 3)
        {
            this.snapshotRT = new RenderTexture((int) (Screen.width * 0.8f), (int) (Screen.height * 0.8f), 24);
            this.snapShotCamera.GetComponent<Camera>().targetTexture = this.snapshotRT;
        }
        else
        {
            this.snapshotRT = new RenderTexture((int) (Screen.width * 0.4f), (int) (Screen.height * 0.4f), 24);
            this.snapShotCamera.GetComponent<Camera>().targetTexture = this.snapshotRT;
        }
    }

    private GameObject findNearestTitan()
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("titan");
        GameObject obj2 = null;
        float positiveInfinity = float.PositiveInfinity;
        this.closestDistance = float.PositiveInfinity;
        float num2 = positiveInfinity;
        Vector3 position = this.main_object.transform.position;
        foreach (GameObject obj3 in objArray)
        {
            Vector3 vector2 = obj3.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position - position;
            float magnitude = vector2.magnitude;
            if (magnitude < num2 && (obj3.GetComponent<TITAN>() == null || !obj3.GetComponent<TITAN>().hasDie))
            {
                obj2 = obj3;
                num2 = magnitude;
                this.closestDistance = num2;
            }
        }
        return obj2;
    }

    public void flashBlind()
    {
        GameObject.Find("flash").GetComponent<UISprite>().alpha = 1f;
        this.flashDuration = 2f;
    }

    private float getSensitivityMulti()
    {
        return sensitivityMulti;
    }

    private float getSensitivityMultiWithDeltaTime()
    {
        return sensitivityMulti * Time.deltaTime * 62f;
    }

    private void reset()
    {
        if (GameType == GameType.Singleplayer)
        {
            GameManager.instance.RestartSingleplayer();
        }
    }

    private Texture2D RTImage(Camera cam)
    {
        RenderTexture tmp = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D textured = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        int num = (int) (cam.targetTexture.width * 0.04f);
        int destX = (int) (cam.targetTexture.width * 0.02f);
        try
        {
            textured.SetPixel(0, 0, Color.white);
            textured.ReadPixels(new Rect(num, num, cam.targetTexture.width - num, cam.targetTexture.height - num), destX, destX);
            textured.Apply();
            RenderTexture.active = tmp;
        }
        catch
        {
            textured = new Texture2D(1, 1);
            textured.SetPixel(0, 0, Color.white);
            return textured;
        }
        return textured;
    }

    public void SetDayLight(Daylight val)
    {
        Daylight = val;
        switch (Daylight)
        {
            case Daylight.Night:
                GameObject obj2 = (GameObject) Instantiate(Resources.Load("flashlight"));
                obj2.transform.parent = transform;
                obj2.transform.position = transform.position;
                obj2.transform.rotation = Quaternion.Euler(353f, 0f, 0f);
                RenderSettings.ambientLight = FengColor.NightAmbientLight;
                GameObject.Find("mainLight").GetComponent<Light>().color = FengColor.NightLight;
                gameObject.GetComponent<Skybox>().material = this.skyBoxNIGHT;
                break;
            case Daylight.Day:
                RenderSettings.ambientLight = FengColor.DayAmbientLight;
                GameObject.Find("mainLight").GetComponent<Light>().color = FengColor.DayLight;
                gameObject.GetComponent<Skybox>().material = this.skyBoxDAY;
                break;
            case Daylight.Dawn:
                RenderSettings.ambientLight = FengColor.DawnAmbientLight;
                GameObject.Find("mainLight").GetComponent<Light>().color = FengColor.DawnLight;
                gameObject.GetComponent<Skybox>().material = this.skyBoxDAWN;
                break;
        }

        this.snapShotCamera.gameObject.GetComponent<Skybox>().material = gameObject.GetComponent<Skybox>().material;
    }

    public void SetInterfacePosition() //TODO: Redo it
    {
        if (Shelter.TryFind("Flare", out GameObject obj))
            Destroy(obj);
        
        if (Shelter.TryFind("Chatroom", out obj))
            Destroy(obj);
        
        if (Shelter.TryFind("UILabel", out obj))
            Destroy(obj);
        
        if (Shelter.TryFind("LabelNetworkStatus", out obj))
            Destroy(obj);
        
        if (Shelter.TryFind("LabelInfoBottomRight", out obj))
            Destroy(obj);
        
        if (Shelter.TryFind("LabelInfoTopCenter", out obj))
            Destroy(obj);
        
        if (Shelter.TryFind("LabelInfoTopRight", out obj))
            Destroy(obj);
        
        
//        GameObject.Find("Flare").transform.localPosition = new Vector3((int)(-Screen.width * 0.5f) + 14, (int)(-Screen.height * 0.5f), 0f);
//        GameObject.Find("LabelInfoBottomRight").transform.localPosition = new Vector3((int)(Screen.width * 0.5f), (int)(-Screen.height * 0.5f), 0f);
//        obj2.GetComponent<UILabel>().text = "Pause : " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.pause] + " ";
//        GameObject.Find("LabelInfoTopCenter").transform.localPosition = new Vector3(0f, (int)(Screen.height * 0.5f), 0f);
//        GameObject.Find("LabelInfoTopRight").transform.localPosition = new Vector3((int)(Screen.width * 0.5f), (int)(Screen.height * 0.5f), 0f);
//        GameObject.Find("LabelNetworkStatus").transform.localPosition = new Vector3((float) ((int) (-Screen.width * 0.5f)), (float) ((int) (Screen.height * 0.5f)), 0f); MOD: Removed Connection state top left
//        GameObject.Find("LabelInfoTopLeft").transform.localPosition = new Vector3((int)(-Screen.width * 0.5f), (int)(Screen.height * 0.5f - 20f), 0f);
//        Destroy(GameObject.Find("Chatroom"));
//        GameObject.Find("Chatroom").transform.localPosition = new Vector3((float) ((int) (-Screen.width * 0.5f)), (float) ((int) (-Screen.height * 0.5f)), 0f);
//        if (GameObject.Find("Chatroom") != null)
//        {
//            GameObject.Find("Chatroom").GetComponent<InRoomChat>().setPosition();
//        }
        if (usingTitan && GameType != GameType.Singleplayer)
        {
            Vector3 vector = new Vector3(0f, 4000, 0f);
            if (Shelter.TryFind("skill_cd_bottom", out obj))
                obj.transform.localPosition = vector;
            
            if (Shelter.TryFind("skill_cd_armin", out obj))
                obj.transform.localPosition = vector;
            if (Shelter.TryFind("skill_cd_eren", out obj))
                obj.transform.localPosition = vector;
            if (Shelter.TryFind("skill_cd_jean", out obj))
                obj.transform.localPosition = vector;
            if (Shelter.TryFind("skill_cd_levi", out obj))
                obj.transform.localPosition = vector;
            if (Shelter.TryFind("skill_cd_marco", out obj))
                obj.transform.localPosition = vector;
            if (Shelter.TryFind("skill_cd_mikasa", out obj))
                obj.transform.localPosition = vector;
            if (Shelter.TryFind("skill_cd_petra", out obj))
                obj.transform.localPosition = vector;
            if (Shelter.TryFind("skill_cd_sasha", out obj))
                obj.transform.localPosition = vector;
            
            if (Shelter.TryFind("GasUI", out obj))
                obj.transform.localPosition = vector;
            
            if (Shelter.TryFind("stamina_titan", out obj))
                obj.transform.localPosition = new Vector3(-160f, (int)(-Screen.height * 0.5f + 15f), 0f);
            if (Shelter.TryFind("stamina_titan_bottom", out obj))
                obj.transform.localPosition = new Vector3(-160f, (int)(-Screen.height * 0.5f + 15f), 0f);
        }
        else
        {
            if (Shelter.TryFind("skill_cd_bottom", out obj))
                obj.transform.localPosition = new Vector3(0f, (int)(-Screen.height * 0.5f + 5f), 0f);
            
            if (Shelter.TryFind("GasUI", out obj))
                obj.transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
            
            if (Shelter.TryFind("stamina_titan", out obj))
                obj.transform.localPosition = new Vector3(0f, 4000, 0f);
            if (Shelter.TryFind("stamina_titan_bottom", out obj))
                obj.transform.localPosition = new Vector3(0f, 4000, 0f);
        }
        if (this.main_object != null && this.main_object.GetComponent<HERO>() != null)
        {
            if (GameType == GameType.Singleplayer)
            {
                this.main_object.GetComponent<HERO>().SetSkillHUDPosition();
            }
            else if (Extensions.GetPhotonView(this.main_object) != null && Extensions.GetPhotonView(this.main_object).isMine)
            {
                this.main_object.GetComponent<HERO>().SetSkillHUDPosition();
            }
        }

        this.TakeSnapshotRT();
    }

    public GameObject setMainObject(GameObject obj, bool resetRotation = true, bool doLockAngle = false)
    {
        float num;
        this.main_object = obj;
        if (obj == null)
        {
            this.head = null;
            num = 1f;
            this.heightMulti = 1f;
            this.distanceMulti = num;
        }
        else if (this.main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head") != null)
        {
            this.head = this.main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
            this.distanceMulti = this.head != null ? Vector3.Distance(this.head.transform.position, this.main_object.transform.position) * 0.2f : 1f;
            this.heightMulti = this.head != null ? Vector3.Distance(this.head.transform.position, this.main_object.transform.position) * 0.33f : 1f;
            if (resetRotation)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        else if (this.main_object.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head") != null)
        {
            this.head = this.main_object.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
            num = 0.64f;
            this.heightMulti = 0.64f;
            this.distanceMulti = num;
            if (resetRotation)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        else
        {
            this.head = null;
            num = 1f;
            this.heightMulti = 1f;
            this.distanceMulti = num;
            if (resetRotation)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        this.lockAngle = doLockAngle;
        return obj;
    }

    public GameObject setMainObjectASTITAN(GameObject obj)
    {
        this.main_object = obj;
        if (this.main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head") != null)
        {
            this.head = this.main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
            this.distanceMulti = this.head != null ? Vector3.Distance(this.head.transform.position, this.main_object.transform.position) * 0.4f : 1f;
            this.heightMulti = this.head != null ? Vector3.Distance(this.head.transform.position, this.main_object.transform.position) * 0.45f : 1f;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        return obj;
    }

    public void setSpectorMode(bool val)
    {
        this.spectatorMode = val;
        GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = !val;
        GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = !val;
    }

    private void shakeUpdate()
    {
        if (duration > 0f)
        {
            duration -= Time.deltaTime;
            
            if (flip)
                gameObject.transform.position += Vector3.up * this.R;
            else
                gameObject.transform.position -= Vector3.up * this.R;
            
            flip = !flip;
            R *= decay;
        }
    }

    public void snapShot2(int index)
    {
        Vector3 vector;
        RaycastHit hit;
        this.snapShotCamera.transform.position = this.head == null ? this.main_object.transform.position : this.head.transform.position;
        Transform t = this.snapShotCamera.transform;
        t.position += Vector3.up * this.heightMulti;
        Transform transform2 = this.snapShotCamera.transform;
        transform2.position -= Vector3.up * 1.1f;
        Vector3 worldPosition = vector = this.snapShotCamera.transform.position;
        Vector3 vector3 = (worldPosition + this.snapShotTargetPosition) * 0.5f;
        this.snapShotCamera.transform.position = vector3;
        worldPosition = vector3;
        this.snapShotCamera.transform.LookAt(this.snapShotTargetPosition);
        if (index == 3)
        {
            this.snapShotCamera.transform.RotateAround(this.transform.position, Vector3.up, Random.Range(-180f, 180f));
        }
        else
        {
            this.snapShotCamera.transform.RotateAround(this.transform.position, Vector3.up, Random.Range(-20f, 20f));
        }
        this.snapShotCamera.transform.LookAt(worldPosition);
        this.snapShotCamera.transform.RotateAround(worldPosition, this.transform.right, Random.Range(-20f, 20f));
        float num = Vector3.Distance(this.snapShotTargetPosition, vector);
        if (this.snapShotTarget != null && this.snapShotTarget.GetComponent<TITAN>() != null)
        {
            num += (index - 1) * this.snapShotTarget.transform.localScale.x * 10f;
        }
        Transform transform3 = this.snapShotCamera.transform;
        transform3.position -= this.snapShotCamera.transform.forward * Random.Range(num + 3f, num + 10f);
        this.snapShotCamera.transform.LookAt(worldPosition);
        this.snapShotCamera.transform.RotateAround(worldPosition, this.transform.forward, Random.Range(-30f, 30f));
        Vector3 end = this.head == null ? this.main_object.transform.position : this.head.transform.position;
        Vector3 vector5 = (this.head == null ? this.main_object.transform.position : this.head.transform.position) - this.snapShotCamera.transform.position;
        end -= vector5;
        LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
        LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
        LayerMask mask3 = mask | mask2;
        if (this.head != null)
        {
            if (Physics.Linecast(this.head.transform.position, end, out hit, mask))
            {
                this.snapShotCamera.transform.position = hit.point;
            }
            else if (Physics.Linecast(this.head.transform.position - vector5 * this.distanceMulti * 3f, end, out hit, mask3))
            {
                this.snapShotCamera.transform.position = hit.point;
            }
        }
        else if (Physics.Linecast(this.main_object.transform.position + Vector3.up, end, out hit, mask3))
        {
            this.snapShotCamera.transform.position = hit.point;
        }
        switch (index)
        {
            case 1:
                this.snapshot1 = this.RTImage(this.snapShotCamera.GetComponent<Camera>());
                SnapShotSaves.addIMG(this.snapshot1, this.snapShotDmg);
                break;

            case 2:
                this.snapshot2 = this.RTImage(this.snapShotCamera.GetComponent<Camera>());
                SnapShotSaves.addIMG(this.snapshot2, this.snapShotDmg);
                break;

            case 3:
                this.snapshot3 = this.RTImage(this.snapShotCamera.GetComponent<Camera>());
                SnapShotSaves.addIMG(this.snapshot3, this.snapShotDmg);
                break;
        }
        this.snapShotCount = index;
        this.hasSnapShot = true;
        this.snapShotCountDown = 2f;
        if (index == 1)
        {
            GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().mainTexture = this.snapshot1;
            GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().transform.localScale = new Vector3(Screen.width * 0.4f, Screen.height * 0.4f, 1f);
            GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().transform.localPosition = new Vector3(-Screen.width * 0.225f, Screen.height * 0.225f, 0f);
            GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().transform.rotation = Quaternion.Euler(0f, 0f, 10f);
            if (PlayerPrefs.HasKey("showSSInGame") && PlayerPrefs.GetInt("showSSInGame") == 1)
            {
                GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().enabled = true;
            }
            else
            {
                GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().enabled = false;
            }
        }
    }

    public void SnapshotUpdate()
    {
        if (this.startSnapShotFrameCount)
        {
            this.snapShotStartCountDownTime -= Time.deltaTime;
            if (this.snapShotStartCountDownTime <= 0f)
            {
                this.snapShot2(1);
                this.startSnapShotFrameCount = false;
            }
        }
        if (this.hasSnapShot)
        {
            this.snapShotCountDown -= Time.deltaTime;
            if (this.snapShotCountDown <= 0f)
            {
                GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().enabled = false;
                this.hasSnapShot = false;
                this.snapShotCountDown = 0f;
            }
            else if (this.snapShotCountDown < 1f)
            {
                GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().mainTexture = this.snapshot3;
            }
            else if (this.snapShotCountDown < 1.5f)
            {
                GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().mainTexture = this.snapshot2;
            }
            if (this.snapShotCount < 3)
            {
                this.snapShotInterval -= Time.deltaTime;
                if (this.snapShotInterval <= 0f)
                {
                    this.snapShotInterval = 0.05f;
                    this.snapShotCount++;
                    this.snapShot2(this.snapShotCount);
                }
            }
        }
    }

    private void Start()
    {
        instance = this;
        isPausing = false;
        sensitivityMulti = PlayerPrefs.GetFloat("MouseSensitivity");
        this.SetDayLight(Daylight);
        this.locker = GameObject.Find("locker");
        if (PlayerPrefs.HasKey("cameraDistance"))
        {
            cameraDistance = PlayerPrefs.GetFloat("cameraDistance") + 0.3f;
        }
        this.TakeSnapshotRT();
    }

    public void startShake(float force, float time, float effectDecay = 0.95f)
    {
        if (this.duration < time)
        {
            this.R = force;
            this.duration = time;
            this.decay = effectDecay;
        }
    }

    public void TakeScreenshot(Vector3 p, int dmg, GameObject targ, float startTime)
    {
        if (!ModuleManager.Enabled(nameof(ModuleSnapshot)))
            return;
        
        if (GameManager.settings.SnapshotDamage > 0 && dmg >= GameManager.settings.SnapshotDamage)
        {
            this.snapShotCount = 1;
            this.startSnapShotFrameCount = true;
            this.snapShotTargetPosition = p;
            this.snapShotTarget = targ;
            this.snapShotStartCountDownTime = startTime;
            this.snapShotInterval = 0.05f + Random.Range(0f, 0.03f);
            this.snapShotDmg = dmg;
        }
    }

    public void DoUpdate()
    {
        if (this.flashDuration > 0f)
        {
            this.flashDuration -= Time.deltaTime;
            if (this.flashDuration <= 0f)
            {
                this.flashDuration = 0f;
            }
            GameObject.Find("flash").GetComponent<UISprite>().alpha = this.flashDuration * 0.5f;
        }
        if (GameType == GameType.NotInRoom)
        {
            Screen.showCursor = true;
            Screen.lockCursor = false;
        }
        else
        {
            if (GameType != GameType.Singleplayer && this.gameOver)
            {
                if (Shelter.InputManager.IsDown(InputAction.Special))
                {
                    if (this.spectatorMode)
                    {
                        this.setSpectorMode(false);
                    }
                    else
                    {
                        this.setSpectorMode(true);
                    }
                }
                if (Shelter.InputManager.IsDown(InputAction.GreenFlare))
                {
                    this.currentPeekPlayerIndex++;
                    int length = GameObject.FindGameObjectsWithTag("Player").Length;
                    if (this.currentPeekPlayerIndex >= length)
                    {
                        this.currentPeekPlayerIndex = 0;
                    }
                    if (length > 0)
                    {
                        this.setMainObject(GameObject.FindGameObjectsWithTag("Player")[this.currentPeekPlayerIndex]);
                        this.setSpectorMode(false);
                        this.lockAngle = false;
                    }
                }
                if (Shelter.InputManager.IsDown(InputAction.RedFlare))
                {
                    this.currentPeekPlayerIndex--;
                    int num2 = GameObject.FindGameObjectsWithTag("Player").Length;
                    if (this.currentPeekPlayerIndex >= num2)
                    {
                        this.currentPeekPlayerIndex = 0;
                    }
                    if (this.currentPeekPlayerIndex < 0)
                    {
                        this.currentPeekPlayerIndex = num2;
                    }
                    if (num2 > 0)
                    {
                        this.setMainObject(GameObject.FindGameObjectsWithTag("Player")[this.currentPeekPlayerIndex]);
                        this.setSpectorMode(false);
                        this.lockAngle = false;
                    }
                }
                if (this.spectatorMode)
                {
                    return;
                }
            }
            if (Shelter.InputManager.IsDown(InputAction.OpenMenu))
            {
                isPausing = !isPausing;
                if (isPausing)
                {
                    if (GameType == GameType.Singleplayer)
                        Time.timeScale = 0f;
                    Screen.showCursor = true;
                    Screen.lockCursor = false;
                }
            }
            if (this.needSetHUD)
            {
                this.needSetHUD = false;
                this.SetInterfacePosition();
                Screen.lockCursor = !Screen.lockCursor;
                Screen.lockCursor = !Screen.lockCursor;
            }
            if (Shelter.InputManager.IsDown(InputAction.ToggleFullscreen) && !Screen.showCursor)
            {
                Screen.fullScreen = !Screen.fullScreen;
                if (Screen.fullScreen)
                {
                    Screen.SetResolution(1280, 720, false);
                }
                else
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                }
                this.needSetHUD = true;
                Minimap.OnScreenResolutionChanged(); //TODO: Call OnResize in all GUIs
            }
            if (Shelter.InputManager.IsDown(InputAction.Suicide))
            {
                this.reset();
            }
            if (this.main_object != null)
            {
                RaycastHit hit;
                if (Shelter.InputManager.IsDown(InputAction.ChangeCamera))
                {
                    if (cameraMode == CameraType.Original)
                    {
                        cameraMode = CameraType.Stop;
                        Screen.lockCursor = false;
                    }
                    else if (cameraMode == CameraType.Stop)
                    {
                        cameraMode = CameraType.TPS;
                        Screen.lockCursor = true;
                    }
                    else if (cameraMode == CameraType.TPS)
                    {
                        cameraMode = CameraType.Original;
                        Screen.lockCursor = false;
                    }
                    this.verticalRotationOffset = 0f;
                    if (GameManager.settings.InSpectatorMode || this.main_object.GetComponent<HERO>() == null)
                    {
                        Screen.showCursor = false;
                    }
                }
                if (Shelter.InputManager.IsDown(InputAction.LockTitan))
                {
                    triggerAutoLock = !triggerAutoLock;
                    if (triggerAutoLock)
                    {
                        this.lockTarget = this.findNearestTitan();
                        if (this.closestDistance >= 150f)
                        {
                            this.lockTarget = null;
                            triggerAutoLock = false;
                        }
                    }
                }
                if (this.gameOver && this.main_object != null)
                {
//                    if (GameManager.inputRC.isInputHumanDown(InputCodeRC.liveCam))
//                    {
//                        if ((int) GameManager.settings[263] == 0)
//                        {
//                            GameManager.settings[263] = 1;
//                        }
//                        else
//                        {
//                            GameManager.settings[263] = 0;
//                        }
//                    }
//                    HERO component = this.main_object.GetComponent<HERO>();
                    /*if (component != null && (int) GameManager.settings[263] == 1 && component.GetComponent<SmoothSyncMovement>().enabled && component.isPhotonCamera)
                    {
                        this.CameraMovementLive(component);
                    }
                    else */if (this.lockAngle)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, this.main_object.transform.rotation, 0.2f);
                        transform.position = Vector3.Lerp(transform.position, this.main_object.transform.position - this.main_object.transform.forward * 5f, 0.2f);
                    }
                    else
                    {
                        this.CameraMovement();
                    }
                }
                else
                {
                    this.CameraMovement();
                }
                if (triggerAutoLock && this.lockTarget != null)
                {
                    float z = this.transform.eulerAngles.z;
                    Transform t = this.lockTarget.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                    Vector3 vector2 = t.position - (this.head == null ? this.main_object.transform.position : this.head.transform.position);
                    vector2.Normalize();
                    this.lockCameraPosition = this.head == null ? this.main_object.transform.position : this.head.transform.position;
                    this.lockCameraPosition -= vector2 * this.distance * this.distanceMulti * this.distanceOffsetMulti;
                    this.lockCameraPosition += Vector3.up * 3f * this.heightMulti * this.distanceOffsetMulti;
                    this.transform.position = Vector3.Lerp(this.transform.position, this.lockCameraPosition, Time.deltaTime * 4f);
                    if (this.head != null)
                    {
                        this.transform.LookAt(this.head.transform.position * 0.8f + t.position * 0.2f);
                    }
                    else
                    {
                        this.transform.LookAt(this.main_object.transform.position * 0.8f + t.position * 0.2f);
                    }
                    this.transform.localEulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, z);
                    Vector2 vector3 = camera.WorldToScreenPoint(t.position - t.forward * this.lockTarget.transform.localScale.x);
                    this.locker.transform.localPosition = new Vector3(vector3.x - Screen.width * 0.5f, vector3.y - Screen.height * 0.5f, 0f);
                    if (this.lockTarget.GetComponent<TITAN>() != null && this.lockTarget.GetComponent<TITAN>().hasDie)
                    {
                        this.lockTarget = null;
                    }
                }
                else
                {
                    this.locker.transform.localPosition = new Vector3(0f, -Screen.height * 0.5f - 50f, 0f);
                }
                Vector3 end = this.head == null ? this.main_object.transform.position : this.head.transform.position;
                Vector3 vector5 = (this.head == null ? this.main_object.transform.position : this.head.transform.position) - this.transform.position;
                Vector3 normalized = vector5.normalized;
                end -= this.distance * normalized * this.distanceMulti;
                LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
                LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
                LayerMask mask3 = mask | mask2;
                if (this.head != null)
                {
                    if (Physics.Linecast(this.head.transform.position, end, out hit, mask))
                    {
                        transform.position = hit.point;
                    }
                    else if (Physics.Linecast(this.head.transform.position - normalized * this.distanceMulti * 3f, end, out hit, mask2))
                    {
                        transform.position = hit.point;
                    }
                    Debug.DrawLine(this.head.transform.position - normalized * this.distanceMulti * 3f, end, Color.red);
                }
                else if (Physics.Linecast(this.main_object.transform.position + Vector3.up, end, out hit, mask3))
                {
                    transform.position = hit.point;
                }
                this.shakeUpdate();
            }
        }
    }

}

