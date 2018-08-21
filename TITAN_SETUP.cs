using System.Collections;
using Mod;
using UnityEngine;

public class TITAN_SETUP : Photon.MonoBehaviour
{
    public GameObject eye;
    private CostumeHair hair;
    private GameObject hair_go_ref;
    private int hairType;
    public bool haseye;
    private GameObject part_hair;
    public int skin;

    private void Awake()
    {
        CostumeHair.init();
        CharacterMaterials.init();
        HeroCostume.init2();
        this.hair_go_ref = new GameObject();
        this.eye.transform.parent = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
        this.hair_go_ref.transform.position = this.eye.transform.position + Vector3.up * 3.5f + transform.forward * 5.2f;
        this.hair_go_ref.transform.rotation = this.eye.transform.rotation;
        this.hair_go_ref.transform.RotateAround(this.eye.transform.position, transform.right, -20f);
        this.hair_go_ref.transform.localScale = new Vector3(210f, 210f, 210f);
        this.hair_go_ref.transform.parent = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
    }

    [RPC]
    public IEnumerator LoadskinE(int hair, int eye, string hairlink)
    {
        bool iteratorVariable0 = false;
        Destroy(this.part_hair);
        this.hair = CostumeHair.hairsM[hair];
        this.hairType = hair;
        if (this.hair.hair != string.Empty)
        {
            GameObject iteratorVariable1 = (GameObject) Instantiate(Resources.Load("Character/" + this.hair.hair));
            iteratorVariable1.transform.parent = this.hair_go_ref.transform.parent;
            iteratorVariable1.transform.position = this.hair_go_ref.transform.position;
            iteratorVariable1.transform.rotation = this.hair_go_ref.transform.rotation;
            iteratorVariable1.transform.localScale = this.hair_go_ref.transform.localScale;
            iteratorVariable1.renderer.material = CharacterMaterials.materials[this.hair.texture];
            bool mipmap = (int) FengGameManagerMKII.settings[63] != 1;
            if (Utility.IsValidImageUrl(hairlink))
            {
                if (hairlink.ToLower() == "transparent")
                {
                    iteratorVariable1.renderer.enabled = false;
                }

                else if (!FengGameManagerMKII.linkHash[0].ContainsKey(hairlink))
                {
                    if (FengGameManagerMKII.linkHash[0].ContainsKey(hairlink))
                    {
                        iteratorVariable1.renderer.material = (Material) FengGameManagerMKII.linkHash[0][hairlink];
                    }
                    else
                    {
                        using (WWW www = new WWW(hairlink))
                        {
                            yield return www;
                            iteratorVariable1.renderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);;
                        }
                        iteratorVariable0 = true;
                        FengGameManagerMKII.linkHash[0].Add(hairlink, iteratorVariable1.renderer.material);
                        iteratorVariable1.renderer.material = (Material) FengGameManagerMKII.linkHash[0][hairlink];
                    }
                }
                else
                {
                    iteratorVariable1.renderer.material = (Material) FengGameManagerMKII.linkHash[0][hairlink];
                }
            }

            this.part_hair = iteratorVariable1;
        }
        if (eye >= 0)
        {
            this.setFacialTexture(this.eye, eye);
        }
        if (iteratorVariable0)
        {
            FengGameManagerMKII.instance.UnloadAssets();
        }
    }

    public void setFacialTexture(GameObject go, int id)
    {
        if (id >= 0)
        {
            float num2 = 0.125f;
            float x = num2 * (int)(id / 8f);
            float y = -0.25f * (id % 4);
            go.renderer.material.mainTextureOffset = new Vector2(x, y);
        }
    }

    public void setHair()
    {
        Destroy(this.part_hair);
        int index = Random.Range(0, CostumeHair.hairsM.Length);
        if (index == 3)
        {
            index = 9;
        }
        this.hairType = index;
        this.hair = CostumeHair.hairsM[index];
        if (this.hair.hair == string.Empty)
        {
            this.hair = CostumeHair.hairsM[9];
            this.hairType = 9;
        }
        this.part_hair = (GameObject) Instantiate(Resources.Load("Character/" + this.hair.hair));
        this.part_hair.transform.parent = this.hair_go_ref.transform.parent;
        this.part_hair.transform.position = this.hair_go_ref.transform.position;
        this.part_hair.transform.rotation = this.hair_go_ref.transform.rotation;
        this.part_hair.transform.localScale = this.hair_go_ref.transform.localScale;
        this.part_hair.renderer.material = CharacterMaterials.materials[this.hair.texture];
        this.part_hair.renderer.material.color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
        int id = Random.Range(1, 8);
        this.setFacialTexture(this.eye, id);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            object[] parameters = new object[] { this.hairType, id, this.part_hair.renderer.material.color.r, this.part_hair.renderer.material.color.g, this.part_hair.renderer.material.color.b };
            photonView.RPC("setHairPRC", PhotonTargets.OthersBuffered, parameters);
        }
    }

    public void setHair2()
    {
        int num;
        object[] objArray2;
        if ((int) FengGameManagerMKII.settings[1] == 1 && (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine))
        {
            Color color;
            num = Random.Range(0, 9);
            if (num == 3)
            {
                num = 9;
            }
            int index = this.skin - 70;
            if ((int) FengGameManagerMKII.settings[32] == 1)
            {
                index = Random.Range(16, 20);
            }
            if ((int) FengGameManagerMKII.settings[index] >= 0)
            {
                num = (int) FengGameManagerMKII.settings[index];
            }
            string hairlink = (string) FengGameManagerMKII.settings[index + 5];
            int eye = Random.Range(1, 8);
            if (this.haseye)
            {
                eye = 0;
            }
            bool valid = Utility.IsValidImageUrl(hairlink);
            switch (IN_GAME_MAIN_CAMERA.GameType)
            {
                case GameType.Multiplayer when photonView.isMine && valid:
                    photonView.RPC("setHairRPC2", PhotonTargets.AllBuffered, num, eye, hairlink);
                    break;
                
                case GameType.Multiplayer when photonView.isMine:
                    color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
                    photonView.RPC("setHairPRC", PhotonTargets.AllBuffered, num, eye, color.r, color.g, color.b);
                    break;
                
                case GameType.Singleplayer when valid:
                    StartCoroutine(this.LoadskinE(num, eye, hairlink));
                    break;
                
                case GameType.Singleplayer:
                    color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
                    this.SetHairPRC(num, eye, color.r, color.g, color.b);
                    break;
            }
        }
        else
        {
            num = Random.Range(0, CostumeHair.hairsM.Length);
            if (num == 3)
            {
                num = 9;
            }
            Destroy(this.part_hair);
            this.hairType = num;
            this.hair = CostumeHair.hairsM[num];
            if (this.hair.hair == string.Empty)
            {
                this.hair = CostumeHair.hairsM[9];
                this.hairType = 9;
            }
            this.part_hair = (GameObject) Instantiate(Resources.Load("Character/" + this.hair.hair));
            this.part_hair.transform.parent = this.hair_go_ref.transform.parent;
            this.part_hair.transform.position = this.hair_go_ref.transform.position;
            this.part_hair.transform.rotation = this.hair_go_ref.transform.rotation;
            this.part_hair.transform.localScale = this.hair_go_ref.transform.localScale;
            this.part_hair.renderer.material = CharacterMaterials.materials[this.hair.texture];
            this.part_hair.renderer.material.color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
            int id = Random.Range(1, 8);
            this.setFacialTexture(this.eye, id);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
            {
                objArray2 = new object[] { this.hairType, id, this.part_hair.renderer.material.color.r, this.part_hair.renderer.material.color.g, this.part_hair.renderer.material.color.b };
                photonView.RPC("setHairPRC", PhotonTargets.OthersBuffered, objArray2);
            }
        }
    }

    [RPC]
    private void SetHairPRC(int type, int eye_type, float c1, float c2, float c3)
    {
        Destroy(this.part_hair);
        this.hair = CostumeHair.hairsM[type];
        this.hairType = type;
        if (this.hair.hair != string.Empty)
        {
            GameObject obj2 = (GameObject) Instantiate(Resources.Load("Character/" + this.hair.hair));
            obj2.transform.parent = this.hair_go_ref.transform.parent;
            obj2.transform.position = this.hair_go_ref.transform.position;
            obj2.transform.rotation = this.hair_go_ref.transform.rotation;
            obj2.transform.localScale = this.hair_go_ref.transform.localScale;
            obj2.renderer.material = CharacterMaterials.materials[this.hair.texture];
            obj2.renderer.material.color = new Color(c1, c2, c3);
            this.part_hair = obj2;
        }
        this.setFacialTexture(this.eye, eye_type);
    }

    [RPC]
    public void SetHairRPC2(int hair, int eye, string hairlink)
    {
        if ((int) FengGameManagerMKII.settings[1] == 1)
        {
            StartCoroutine(this.LoadskinE(hair, eye, hairlink));
        }
    }

    public void setPunkHair()
    {
        Destroy(this.part_hair);
        this.hair = CostumeHair.hairsM[3];
        this.hairType = 3;
        GameObject obj2 = (GameObject) Instantiate(Resources.Load("Character/" + this.hair.hair));
        obj2.transform.parent = this.hair_go_ref.transform.parent;
        obj2.transform.position = this.hair_go_ref.transform.position;
        obj2.transform.rotation = this.hair_go_ref.transform.rotation;
        obj2.transform.localScale = this.hair_go_ref.transform.localScale;
        obj2.renderer.material = CharacterMaterials.materials[this.hair.texture];
        switch (Random.Range(1, 4))
        {
            case 1:
                obj2.renderer.material.color = FengColor.hairPunk1;
                break;

            case 2:
                obj2.renderer.material.color = FengColor.hairPunk2;
                break;

            case 3:
                obj2.renderer.material.color = FengColor.hairPunk3;
                break;
        }
        this.part_hair = obj2;
        this.setFacialTexture(this.eye, 0);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            object[] parameters = new object[] { this.hairType, 0, this.part_hair.renderer.material.color.r, this.part_hair.renderer.material.color.g, this.part_hair.renderer.material.color.b };
            photonView.RPC("setHairPRC", PhotonTargets.OthersBuffered, parameters);
        }
    }

    public void setVar(int skin, bool haseye)
    {
        this.skin = skin;
        this.haseye = haseye;
    }
}