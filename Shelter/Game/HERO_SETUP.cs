using System.Collections.Generic;
using Game;
using UnityEngine;
using Xft;

// ReSharper disable once CheckNamespace
public class HERO_SETUP : MonoBehaviour
{
    public string aniname;
    public float anitime;
    private List<BoneWeight> boneWeightsList = new List<BoneWeight>();
    public bool change;
    public GameObject chest_info;
    private byte[] config = new byte[4];
    public int currentOne;
    public SkinnedMeshRenderer[][] elements;
    public bool isDeadBody;
    private List<Material> materialList;
    private GameObject mount_3dmg;
    private GameObject mount_3dmg_gas_l;
    private GameObject mount_3dmg_gas_r;
    private GameObject mount_3dmg_gun_mag_l;
    private GameObject mount_3dmg_gun_mag_r;
    private GameObject mount_weapon_l;
    private GameObject mount_weapon_r;
    public HeroCostume myCostume;
    public GameObject part_3dmg;
    public GameObject part_3dmg_belt;
    public GameObject part_3dmg_gas_l;
    public GameObject part_3dmg_gas_r;
    public GameObject part_arm_l;
    public GameObject part_arm_r;
    public GameObject part_asset_1;
    public GameObject part_asset_2;
    public GameObject part_blade_l;
    public GameObject part_blade_r;
    public GameObject part_brand_1;
    public GameObject part_brand_2;
    public GameObject part_brand_3;
    public GameObject part_brand_4;
    public GameObject part_cape;
    public GameObject part_chest;
    public GameObject part_chest_1;
    public GameObject part_chest_2;
    public GameObject part_chest_3;
    public GameObject part_eye;
    public GameObject part_face;
    public GameObject part_glass;
    public GameObject part_hair;
    public GameObject part_hair_1;
    public GameObject part_hair_2;
    public GameObject part_hand_l;
    public GameObject part_hand_r;
    public GameObject part_head;
    public GameObject part_leg;
    public GameObject part_upper_body;
    public GameObject reference;
    public float timer;

    private void Awake()
    {
        this.part_head.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
        this.mount_3dmg = new GameObject();
        this.mount_3dmg_gas_l = new GameObject();
        this.mount_3dmg_gas_r = new GameObject();
        this.mount_3dmg_gun_mag_l = new GameObject();
        this.mount_3dmg_gun_mag_r = new GameObject();
        this.mount_weapon_l = new GameObject();
        this.mount_weapon_r = new GameObject();
        this.mount_3dmg.transform.position = transform.position;
        this.mount_3dmg.transform.rotation = Quaternion.Euler(270f, transform.rotation.eulerAngles.y, 0f);
        this.mount_3dmg.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest").transform;
        this.mount_3dmg_gas_l.transform.position = transform.position;
        this.mount_3dmg_gas_l.transform.rotation = Quaternion.Euler(270f, transform.rotation.eulerAngles.y, 0f);
        this.mount_3dmg_gas_l.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine").transform;
        this.mount_3dmg_gas_r.transform.position = transform.position;
        this.mount_3dmg_gas_r.transform.rotation = Quaternion.Euler(270f, transform.rotation.eulerAngles.y, 0f);
        this.mount_3dmg_gas_r.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine").transform;
        this.mount_3dmg_gun_mag_l.transform.position = transform.position;
        this.mount_3dmg_gun_mag_l.transform.rotation = Quaternion.Euler(270f, transform.rotation.eulerAngles.y, 0f);
        this.mount_3dmg_gun_mag_l.transform.parent = transform.Find("Amarture/Controller_Body/hip/thigh_L").transform;
        this.mount_3dmg_gun_mag_r.transform.position = transform.position;
        this.mount_3dmg_gun_mag_r.transform.rotation = Quaternion.Euler(270f, transform.rotation.eulerAngles.y, 0f);
        this.mount_3dmg_gun_mag_r.transform.parent = transform.Find("Amarture/Controller_Body/hip/thigh_R").transform;
        this.mount_weapon_l.transform.position = transform.position;
        this.mount_weapon_l.transform.rotation = Quaternion.Euler(270f, transform.rotation.eulerAngles.y, 0f);
        this.mount_weapon_l.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L").transform;
        this.mount_weapon_r.transform.position = transform.position;
        this.mount_weapon_r.transform.rotation = Quaternion.Euler(270f, transform.rotation.eulerAngles.y, 0f);
        this.mount_weapon_r.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R").transform;
    }

    private void combineSMR(GameObject go, GameObject go2)
    {
        if (go.GetComponent<SkinnedMeshRenderer>() == null)
        {
            go.AddComponent<SkinnedMeshRenderer>();
        }
        SkinnedMeshRenderer component = go.GetComponent<SkinnedMeshRenderer>();
        List<CombineInstance> list = new List<CombineInstance>();
        this.materialList = new List<Material>
        {
            component.material
        };
        this.boneWeightsList = new List<BoneWeight>();
        Transform[] bones = component.bones;
        SkinnedMeshRenderer renderer2 = go2.GetComponent<SkinnedMeshRenderer>();
        for (int i = 0; i < renderer2.sharedMesh.subMeshCount; i++)
        {
            CombineInstance item = new CombineInstance {
                mesh = renderer2.sharedMesh,
                transform = renderer2.transform.localToWorldMatrix,
                subMeshIndex = i
            };
            list.Add(item);
            for (int k = 0; k < this.materialList.Count; k++)
            {
                Material material = this.materialList[k];
                if (material.name != renderer2.material.name)
                {
                    goto Label_00DA;
                }
            }
            continue;
        Label_00DA:
            this.materialList.Add(renderer2.material);
        }
        Destroy(renderer2.gameObject);
        component.sharedMesh = new Mesh();
        component.sharedMesh.CombineMeshes(list.ToArray(), true, false);
        component.bones = bones;
        component.materials = this.materialList.ToArray();
        List<Matrix4x4> list2 = new List<Matrix4x4>();
        for (int j = 0; j < bones.Length; j++)
        {
            if (bones[j] != null)
            {
                list2.Add(bones[j].worldToLocalMatrix * transform.localToWorldMatrix);
            }
        }
        component.sharedMesh.bindposes = list2.ToArray();
    }

    public void create3DMG()
    {
        Destroy(this.part_3dmg);
        Destroy(this.part_3dmg_belt);
        Destroy(this.part_3dmg_gas_l);
        Destroy(this.part_3dmg_gas_r);
        Destroy(this.part_blade_l);
        Destroy(this.part_blade_r);
        if (this.myCostume.mesh_3dmg.Length > 0)
        {
            this.part_3dmg = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.mesh_3dmg));
            this.part_3dmg.transform.position = this.mount_3dmg.transform.position;
            this.part_3dmg.transform.rotation = this.mount_3dmg.transform.rotation;
            this.part_3dmg.transform.parent = this.mount_3dmg.transform.parent;
            this.part_3dmg.renderer.material = CharacterMaterials.materials[this.myCostume._3dmg_texture];
        }
        if (this.myCostume.mesh_3dmg_belt.Length > 0)
        {
            this.part_3dmg_belt = this.GenerateCloth(this.reference, "Character/" + this.myCostume.mesh_3dmg_belt);
            this.part_3dmg_belt.renderer.material = CharacterMaterials.materials[this.myCostume._3dmg_texture];
        }
        if (this.myCostume.mesh_3dmg_gas_l.Length > 0)
        {
            this.part_3dmg_gas_l = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.mesh_3dmg_gas_l));
            if (this.myCostume.uniform_type != UniformType.CasualAHSS)
            {
                this.part_3dmg_gas_l.transform.position = this.mount_3dmg_gas_l.transform.position;
                this.part_3dmg_gas_l.transform.rotation = this.mount_3dmg_gas_l.transform.rotation;
                this.part_3dmg_gas_l.transform.parent = this.mount_3dmg_gas_l.transform.parent;
            }
            else
            {
                this.part_3dmg_gas_l.transform.position = this.mount_3dmg_gun_mag_l.transform.position;
                this.part_3dmg_gas_l.transform.rotation = this.mount_3dmg_gun_mag_l.transform.rotation;
                this.part_3dmg_gas_l.transform.parent = this.mount_3dmg_gun_mag_l.transform.parent;
            }
            this.part_3dmg_gas_l.renderer.material = CharacterMaterials.materials[this.myCostume._3dmg_texture];
        }
        if (this.myCostume.mesh_3dmg_gas_r.Length > 0)
        {
            this.part_3dmg_gas_r = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.mesh_3dmg_gas_r));
            if (this.myCostume.uniform_type != UniformType.CasualAHSS)
            {
                this.part_3dmg_gas_r.transform.position = this.mount_3dmg_gas_r.transform.position;
                this.part_3dmg_gas_r.transform.rotation = this.mount_3dmg_gas_r.transform.rotation;
                this.part_3dmg_gas_r.transform.parent = this.mount_3dmg_gas_r.transform.parent;
            }
            else
            {
                this.part_3dmg_gas_r.transform.position = this.mount_3dmg_gun_mag_r.transform.position;
                this.part_3dmg_gas_r.transform.rotation = this.mount_3dmg_gun_mag_r.transform.rotation;
                this.part_3dmg_gas_r.transform.parent = this.mount_3dmg_gun_mag_r.transform.parent;
            }
            this.part_3dmg_gas_r.renderer.material = CharacterMaterials.materials[this.myCostume._3dmg_texture];
        }
        if (this.myCostume.weapon_l_mesh.Length > 0)
        {
            this.part_blade_l = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.weapon_l_mesh));
            this.part_blade_l.transform.position = this.mount_weapon_l.transform.position;
            this.part_blade_l.transform.rotation = this.mount_weapon_l.transform.rotation;
            this.part_blade_l.transform.parent = this.mount_weapon_l.transform.parent;
            this.part_blade_l.renderer.material = CharacterMaterials.materials[this.myCostume._3dmg_texture];
            if (this.part_blade_l.transform.Find("X-WeaponTrailA") != null)
            {
                this.part_blade_l.transform.Find("X-WeaponTrailA").GetComponent<XWeaponTrail>().Deactivate();
                this.part_blade_l.transform.Find("X-WeaponTrailB").GetComponent<XWeaponTrail>().Deactivate();
                if (gameObject.GetComponent<HERO>() != null)
                {
                    gameObject.GetComponent<HERO>().leftbladetrail = this.part_blade_l.transform.Find("X-WeaponTrailA").GetComponent<XWeaponTrail>();
                    gameObject.GetComponent<HERO>().leftbladetrail2 = this.part_blade_l.transform.Find("X-WeaponTrailB").GetComponent<XWeaponTrail>();
                }
            }
        }
        if (this.myCostume.weapon_r_mesh.Length > 0)
        {
            this.part_blade_r = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.weapon_r_mesh));
            this.part_blade_r.transform.position = this.mount_weapon_r.transform.position;
            this.part_blade_r.transform.rotation = this.mount_weapon_r.transform.rotation;
            this.part_blade_r.transform.parent = this.mount_weapon_r.transform.parent;
            this.part_blade_r.renderer.material = CharacterMaterials.materials[this.myCostume._3dmg_texture];
            if (this.part_blade_r.transform.Find("X-WeaponTrailA") != null)
            {
                this.part_blade_r.transform.Find("X-WeaponTrailA").GetComponent<XWeaponTrail>().Deactivate();
                this.part_blade_r.transform.Find("X-WeaponTrailB").GetComponent<XWeaponTrail>().Deactivate();
                if (gameObject.GetComponent<HERO>() != null)
                {
                    gameObject.GetComponent<HERO>().rightbladetrail = this.part_blade_r.transform.Find("X-WeaponTrailA").GetComponent<XWeaponTrail>();
                    gameObject.GetComponent<HERO>().rightbladetrail2 = this.part_blade_r.transform.Find("X-WeaponTrailB").GetComponent<XWeaponTrail>();
                }
            }
        }
    }

    public void createCape()
    {
        Destroy(this.part_cape);
        if (this.myCostume.cape_mesh.Length > 0)
        {
            this.part_cape = this.GenerateCloth(this.reference, "Character/" + this.myCostume.cape_mesh);
            this.part_cape.renderer.material = CharacterMaterials.materials[this.myCostume.brand_texture];
        }
    }

    public void createCape2()
    {
        if (!this.isDeadBody)
        {
            ClothFactory.DisposeObject(this.part_cape);
            if (this.myCostume.cape_mesh.Length > 0)
            {
                this.part_cape = ClothFactory.GetCape(this.reference, "Character/" + this.myCostume.cape_mesh, CharacterMaterials.materials[this.myCostume.brand_texture]);
            }
        }
    }

    public void createFace()
    {
        this.part_face = (GameObject) Instantiate(Resources.Load("Character/character_face"));
        this.part_face.transform.position = this.part_head.transform.position;
        this.part_face.transform.rotation = this.part_head.transform.rotation;
        this.part_face.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
    }

    public void createGlass()
    {
        this.part_glass = (GameObject) Instantiate(Resources.Load("Character/glass"));
        this.part_glass.transform.position = this.part_head.transform.position;
        this.part_glass.transform.rotation = this.part_head.transform.rotation;
        this.part_glass.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
    }

    public void createHair()
    {
        Destroy(this.part_hair);
        Destroy(this.part_hair_1);
        if (this.myCostume.hair_mesh != string.Empty)
        {
            this.part_hair = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.hair_mesh));
            this.part_hair.transform.position = this.part_head.transform.position;
            this.part_hair.transform.rotation = this.part_head.transform.rotation;
            this.part_hair.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
            this.part_hair.renderer.material = CharacterMaterials.materials[this.myCostume.hairInfo.Texture];
            this.part_hair.renderer.material.color = this.myCostume.hair_color;
        }
        if (this.myCostume.hair_1_mesh.Length > 0)
        {
            this.part_hair_1 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.hair_1_mesh);
            this.part_hair_1.renderer.material = CharacterMaterials.materials[this.myCostume.hairInfo.Texture];
            this.part_hair_1.renderer.material.color = this.myCostume.hair_color;
        }
    }

    public void createHair2()
    {
        Destroy(this.part_hair);
        if (!this.isDeadBody)
        {
            ClothFactory.DisposeObject(this.part_hair_1);
        }
        if (this.myCostume.hair_mesh != string.Empty)
        {
            this.part_hair = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.hair_mesh));
            this.part_hair.transform.position = this.part_head.transform.position;
            this.part_hair.transform.rotation = this.part_head.transform.rotation;
            this.part_hair.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
            this.part_hair.renderer.material = CharacterMaterials.materials[this.myCostume.hairInfo.Texture];
            this.part_hair.renderer.material.color = this.myCostume.hair_color;
        }
        if (this.myCostume.hair_1_mesh.Length > 0 && !this.isDeadBody)
        {
            string name = "Character/" + this.myCostume.hair_1_mesh;
            Material material = CharacterMaterials.materials[this.myCostume.hairInfo.Texture];
            this.part_hair_1 = ClothFactory.GetHair(this.reference, name, material, this.myCostume.hair_color);
        }
    }

    public void createHead()
    {
        Destroy(this.part_eye);
        Destroy(this.part_face);
        Destroy(this.part_glass);
        Destroy(this.part_hair);
        Destroy(this.part_hair_1);
        this.createHair2();
        if (this.myCostume.eye_mesh.Length > 0)
        {
            this.part_eye = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.eye_mesh));
            this.part_eye.transform.position = this.part_head.transform.position;
            this.part_eye.transform.rotation = this.part_head.transform.rotation;
            this.part_eye.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
            this.setFacialTexture(this.part_eye, this.myCostume.eye_texture_id);
        }
        if (this.myCostume.beard_texture_id >= 0)
        {
            this.createFace();
            this.setFacialTexture(this.part_face, this.myCostume.beard_texture_id);
        }
        if (this.myCostume.glass_texture_id >= 0)
        {
            this.createGlass();
            this.setFacialTexture(this.part_glass, this.myCostume.glass_texture_id);
        }
        this.part_head.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
        this.part_chest.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
    }

    public void createHead2()
    {
        Destroy(this.part_eye);
        Destroy(this.part_face);
        Destroy(this.part_glass);
        Destroy(this.part_hair);
        if (!this.isDeadBody)
        {
            ClothFactory.DisposeObject(this.part_hair_1);
        }
        this.createHair2();
        if (this.myCostume.eye_mesh.Length > 0)
        {
            this.part_eye = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.eye_mesh));
            this.part_eye.transform.position = this.part_head.transform.position;
            this.part_eye.transform.rotation = this.part_head.transform.rotation;
            this.part_eye.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
            this.setFacialTexture(this.part_eye, this.myCostume.eye_texture_id);
        }
        if (this.myCostume.beard_texture_id >= 0)
        {
            this.createFace();
            this.setFacialTexture(this.part_face, this.myCostume.beard_texture_id);
        }
        if (this.myCostume.glass_texture_id >= 0)
        {
            this.createGlass();
            this.setFacialTexture(this.part_glass, this.myCostume.glass_texture_id);
        }
        this.part_head.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
        this.part_chest.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
    }

    public void createLeftArm()
    {
        Destroy(this.part_arm_l);
        if (this.myCostume.arm_l_mesh.Length > 0)
        {
            this.part_arm_l = this.GenerateCloth(this.reference, "Character/" + this.myCostume.arm_l_mesh);
            this.part_arm_l.renderer.material = CharacterMaterials.materials[this.myCostume.body_texture];
        }
        Destroy(this.part_hand_l);
        if (this.myCostume.hand_l_mesh.Length > 0)
        {
            this.part_hand_l = this.GenerateCloth(this.reference, "Character/" + this.myCostume.hand_l_mesh);
            this.part_hand_l.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
        }
    }

    public void createLowerBody()
    {
        this.part_leg.renderer.material = CharacterMaterials.materials[this.myCostume.body_texture];
    }

    public void createRightArm()
    {
        Destroy(this.part_arm_r);
        if (this.myCostume.arm_r_mesh.Length > 0)
        {
            this.part_arm_r = this.GenerateCloth(this.reference, "Character/" + this.myCostume.arm_r_mesh);
            this.part_arm_r.renderer.material = CharacterMaterials.materials[this.myCostume.body_texture];
        }
        Destroy(this.part_hand_r);
        if (this.myCostume.hand_r_mesh.Length > 0)
        {
            this.part_hand_r = this.GenerateCloth(this.reference, "Character/" + this.myCostume.hand_r_mesh);
            this.part_hand_r.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
        }
    }

    public void createUpperBody()
    {
        Destroy(this.part_upper_body);
        Destroy(this.part_brand_1);
        Destroy(this.part_brand_2);
        Destroy(this.part_brand_3);
        Destroy(this.part_brand_4);
        Destroy(this.part_chest_1);
        Destroy(this.part_chest_2);
        Destroy(this.part_chest_3);
        this.createCape2();
        if (this.myCostume.part_chest_object_mesh.Length > 0)
        {
            this.part_chest_1 = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.part_chest_object_mesh));
            this.part_chest_1.transform.position = this.chest_info.transform.position;
            this.part_chest_1.transform.rotation = this.chest_info.transform.rotation;
            this.part_chest_1.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest").transform;
            this.part_chest_1.renderer.material = CharacterMaterials.materials[this.myCostume.part_chest_object_texture];
        }
        if (this.myCostume.part_chest_1_object_mesh.Length > 0)
        {
            this.part_chest_2 = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.part_chest_1_object_mesh));
            this.part_chest_2.transform.position = this.chest_info.transform.position;
            this.part_chest_2.transform.rotation = this.chest_info.transform.rotation;
            this.part_chest_2.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest").transform;
            this.part_chest_2.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest").transform;
            this.part_chest_2.renderer.material = CharacterMaterials.materials[this.myCostume.part_chest_1_object_texture];
        }
        if (this.myCostume.part_chest_skinned_cloth_mesh.Length > 0)
        {
            this.part_chest_3 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.part_chest_skinned_cloth_mesh);
            this.part_chest_3.renderer.material = CharacterMaterials.materials[this.myCostume.part_chest_skinned_cloth_texture];
        }
        if (this.myCostume.body_mesh.Length > 0)
        {
            this.part_upper_body = this.GenerateCloth(this.reference, "Character/" + this.myCostume.body_mesh);
            this.part_upper_body.renderer.material = CharacterMaterials.materials[this.myCostume.body_texture];
        }
        if (this.myCostume.brand1_mesh.Length > 0)
        {
            this.part_brand_1 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand1_mesh);
            this.part_brand_1.renderer.material = CharacterMaterials.materials[this.myCostume.brand_texture];
        }
        if (this.myCostume.brand2_mesh.Length > 0)
        {
            this.part_brand_2 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand2_mesh);
            this.part_brand_2.renderer.material = CharacterMaterials.materials[this.myCostume.brand_texture];
        }
        if (this.myCostume.brand3_mesh.Length > 0)
        {
            this.part_brand_3 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand3_mesh);
            this.part_brand_3.renderer.material = CharacterMaterials.materials[this.myCostume.brand_texture];
        }
        if (this.myCostume.brand4_mesh.Length > 0)
        {
            this.part_brand_4 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand4_mesh);
            this.part_brand_4.renderer.material = CharacterMaterials.materials[this.myCostume.brand_texture];
        }
        this.part_head.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
        this.part_chest.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
    }

    public void createUpperBody2()
    {
        Destroy(this.part_upper_body);
        Destroy(this.part_brand_1);
        Destroy(this.part_brand_2);
        Destroy(this.part_brand_3);
        Destroy(this.part_brand_4);
        Destroy(this.part_chest_1);
        Destroy(this.part_chest_2);
        if (!this.isDeadBody)
        {
            ClothFactory.DisposeObject(this.part_chest_3);
        }
        this.createCape2();
        if (this.myCostume.part_chest_object_mesh.Length > 0)
        {
            this.part_chest_1 = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.part_chest_object_mesh));
            this.part_chest_1.transform.position = this.chest_info.transform.position;
            this.part_chest_1.transform.rotation = this.chest_info.transform.rotation;
            this.part_chest_1.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest").transform;
            this.part_chest_1.renderer.material = CharacterMaterials.materials[this.myCostume.part_chest_object_texture];
        }
        if (this.myCostume.part_chest_1_object_mesh.Length > 0)
        {
            this.part_chest_2 = (GameObject) Instantiate(Resources.Load("Character/" + this.myCostume.part_chest_1_object_mesh));
            this.part_chest_2.transform.position = this.chest_info.transform.position;
            this.part_chest_2.transform.rotation = this.chest_info.transform.rotation;
            this.part_chest_2.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest").transform;
            this.part_chest_2.transform.parent = transform.Find("Amarture/Controller_Body/hip/spine/chest").transform;
            this.part_chest_2.renderer.material = CharacterMaterials.materials[this.myCostume.part_chest_1_object_texture];
        }
        if (this.myCostume.part_chest_skinned_cloth_mesh.Length > 0 && !this.isDeadBody)
        {
            this.part_chest_3 = ClothFactory.GetCape(this.reference, "Character/" + this.myCostume.part_chest_skinned_cloth_mesh, CharacterMaterials.materials[this.myCostume.part_chest_skinned_cloth_texture]);
        }
        if (this.myCostume.body_mesh.Length > 0)
        {
            this.part_upper_body = this.GenerateCloth(this.reference, "Character/" + this.myCostume.body_mesh);
            this.part_upper_body.renderer.material = CharacterMaterials.materials[this.myCostume.body_texture];
        }
        if (this.myCostume.brand1_mesh.Length > 0)
        {
            this.part_brand_1 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand1_mesh);
            this.part_brand_1.renderer.material = CharacterMaterials.materials[this.myCostume.brand_texture];
        }
        if (this.myCostume.brand2_mesh.Length > 0)
        {
            this.part_brand_2 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand2_mesh);
            this.part_brand_2.renderer.material = CharacterMaterials.materials[this.myCostume.brand_texture];
        }
        if (this.myCostume.brand3_mesh.Length > 0)
        {
            this.part_brand_3 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand3_mesh);
            this.part_brand_3.renderer.material = CharacterMaterials.materials[this.myCostume.brand_texture];
        }
        if (this.myCostume.brand4_mesh.Length > 0)
        {
            this.part_brand_4 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand4_mesh);
            this.part_brand_4.renderer.material = CharacterMaterials.materials[this.myCostume.brand_texture];
        }
        this.part_head.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
        this.part_chest.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
    }

    public void deleteCharacterComponent()
    {
        Destroy(this.part_eye);
        Destroy(this.part_face);
        Destroy(this.part_glass);
        Destroy(this.part_hair);
        Destroy(this.part_hair_1);
        Destroy(this.part_upper_body);
        Destroy(this.part_arm_l);
        Destroy(this.part_arm_r);
        Destroy(this.part_hair_2);
        Destroy(this.part_cape);
        Destroy(this.part_brand_1);
        Destroy(this.part_brand_2);
        Destroy(this.part_brand_3);
        Destroy(this.part_brand_4);
        Destroy(this.part_chest_1);
        Destroy(this.part_chest_2);
        Destroy(this.part_chest_3);
        Destroy(this.part_3dmg);
        Destroy(this.part_3dmg_belt);
        Destroy(this.part_3dmg_gas_l);
        Destroy(this.part_3dmg_gas_r);
        Destroy(this.part_blade_l);
        Destroy(this.part_blade_r);
    }

    public void deleteCharacterComponent2()
    {
        Destroy(this.part_eye);
        Destroy(this.part_face);
        Destroy(this.part_glass);
        Destroy(this.part_hair);
        if (!this.isDeadBody)
        {
            ClothFactory.DisposeObject(this.part_hair_1);
        }
        Destroy(this.part_upper_body);
        Destroy(this.part_arm_l);
        Destroy(this.part_arm_r);
        if (!this.isDeadBody)
        {
            ClothFactory.DisposeObject(this.part_hair_2);
            ClothFactory.DisposeObject(this.part_cape);
        }
        Destroy(this.part_brand_1);
        Destroy(this.part_brand_2);
        Destroy(this.part_brand_3);
        Destroy(this.part_brand_4);
        Destroy(this.part_chest_1);
        Destroy(this.part_chest_2);
        Destroy(this.part_chest_3);
        Destroy(this.part_3dmg);
        Destroy(this.part_3dmg_belt);
        Destroy(this.part_3dmg_gas_l);
        Destroy(this.part_3dmg_gas_r);
        Destroy(this.part_blade_l);
        Destroy(this.part_blade_r);
    }

    private GameObject GenerateCloth(GameObject go, string res)
    {
        if (go.GetComponent<SkinnedMeshRenderer>() == null)
        {
            go.AddComponent<SkinnedMeshRenderer>();
        }
        SkinnedMeshRenderer component = go.GetComponent<SkinnedMeshRenderer>();
        Transform[] bones = component.bones;
        SkinnedMeshRenderer renderer2 = ((GameObject) Instantiate(Resources.Load(res))).GetComponent<SkinnedMeshRenderer>();
        renderer2.gameObject.transform.parent = component.gameObject.transform.parent;
        renderer2.transform.localPosition = Vector3.zero;
        renderer2.transform.localScale = Vector3.one;
        renderer2.bones = bones;
        renderer2.quality = SkinQuality.Bone4;
        return renderer2.gameObject;
    }

    private byte[] GetCurrentConfig()
    {
        return this.config;
    }

    public void setCharacterComponent()
    {
        this.createHead2();
        this.createUpperBody2();
        this.createLeftArm();
        this.createRightArm();
        this.createLowerBody();
        this.create3DMG();
    }

    public void setFacialTexture(GameObject go, int id)
    {
        if (id >= 0)
        {
            go.renderer.material = CharacterMaterials.materials[this.myCostume.face_texture];
            float num = 0.125f;
            float x = num * (int)(id / 8f);
            float y = -0.125f * (id % 8);
            go.renderer.material.mainTextureOffset = new Vector2(x, y);
        }
    }

    public void setSkin()
    {
        this.part_head.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
        this.part_chest.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
        this.part_hand_l.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
        this.part_hand_r.renderer.material = CharacterMaterials.materials[this.myCostume.skin_texture];
    }

    private void Update()
    {
    }
}

