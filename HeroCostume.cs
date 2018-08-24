using UnityEngine;

public class HeroCostume
{
    public static readonly HeroCostume[] costume;
    public static readonly HeroCostume[] costumeOption;
    
    public string _3dmg_texture = string.Empty;
    public string arm_l_mesh = string.Empty;
    public string arm_r_mesh = string.Empty;
    public string beard_mesh = string.Empty;
    public int beard_texture_id = -1;
    public string body_mesh = string.Empty;
    public string body_texture = string.Empty;
    public string brand_texture = string.Empty;
    public string brand1_mesh = string.Empty;
    public string brand2_mesh = string.Empty;
    public string brand3_mesh = string.Empty;
    public string brand4_mesh = string.Empty;
    public bool cape;
    public string cape_mesh = string.Empty;
    public string cape_texture = string.Empty;
    public int costumeId;
    public Division division;
    public string eye_mesh = string.Empty;
    public int eye_texture_id = -1;
    public string face_texture = string.Empty;
    public string glass_mesh = string.Empty;
    public int glass_texture_id = -1;
    public string hair_1_mesh = string.Empty;
    public Color hair_color = new Color(0.5f, 0.1f, 0f);
    public string hair_mesh = string.Empty;
    public CostumeHair hairInfo;
    public string hand_l_mesh = string.Empty;
    public string hand_r_mesh = string.Empty;
    public int id;
    public string mesh_3dmg = string.Empty;
    public string mesh_3dmg_belt = string.Empty;
    public string mesh_3dmg_gas_l = string.Empty;
    public string mesh_3dmg_gas_r = string.Empty;
    public string name = string.Empty;
    public string part_chest_1_object_mesh = string.Empty;
    public string part_chest_1_object_texture = string.Empty;
    public string part_chest_object_mesh = string.Empty;
    public string part_chest_object_texture = string.Empty;
    public string part_chest_skinned_cloth_mesh = string.Empty;
    public string part_chest_skinned_cloth_texture = string.Empty;
    public Sex sex;
    public int skin_color = 1;
    public string skin_texture = string.Empty;
    public HeroStat stat;
    public UniformType uniform_type = UniformType.CasualA;
    public string weapon_l_mesh = string.Empty;
    public string weapon_r_mesh = string.Empty;

    public void ValidateHeroStats()
    {
        if (stat.Speed + stat.Gas + stat.Blade + stat.Acceleration > 400)
        {
            stat.Acceleration = 100;
            stat.Blade = 100;
            stat.Gas = 100;
            stat.Speed = 100;
        }
    }

    static HeroCostume()
    {
        var bodyUniformMaTexture = new[] {"aottg_hero_uniform_ma_1", "aottg_hero_uniform_ma_2", "aottg_hero_uniform_ma_3"};
        var bodyUniformFaTexture = new[] {"aottg_hero_uniform_fa_1", "aottg_hero_uniform_fa_2", "aottg_hero_uniform_fa_3"};
        var bodyUniformMbTexture = new[] { "aottg_hero_uniform_mb_1", "aottg_hero_uniform_mb_2", "aottg_hero_uniform_mb_3", "aottg_hero_uniform_mb_4" };
        var bodyUniformFbTexture = new[] {"aottg_hero_uniform_fb_1", "aottg_hero_uniform_fb_2"};
        var bodyCasualMaTexture = new[] {"aottg_hero_casual_ma_1", "aottg_hero_casual_ma_2", "aottg_hero_casual_ma_3"};
        var bodyCasualFaTexture = new[] {"aottg_hero_casual_fa_1", "aottg_hero_casual_fa_2", "aottg_hero_casual_fa_3"};
        var bodyCasualMbTexture = new[] {"aottg_hero_casual_mb_1", "aottg_hero_casual_mb_2", "aottg_hero_casual_mb_3", "aottg_hero_casual_mb_4"};
        var bodyCasualFbTexture = new[] {"aottg_hero_casual_fb_1", "aottg_hero_casual_fb_2"};


        costume = new[]
        {
            new HeroCostume
            {
                name = "annie",
                sex = Sex.Female,
                uniform_type = UniformType.UniformB,
                part_chest_object_mesh = "character_cap_uniform",
                part_chest_object_texture = "aottg_hero_annie_cap_uniform",
                cape = true,
                body_texture = bodyUniformFbTexture[0],
                hairInfo = CostumeHair.FemaleHairs[5],
                eye_texture_id = 0,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(1f, 0.9f, 0.5f),
                division = Division.TheMilitaryPolice,
                costumeId = 0
            },
            new HeroCostume
            {
                name = "annie",
                sex = Sex.Female,
                uniform_type = UniformType.UniformB,
                part_chest_object_mesh = "character_cap_uniform",
                part_chest_object_texture = "aottg_hero_annie_cap_uniform",
                body_texture = bodyUniformFbTexture[0],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[5],
                eye_texture_id = 0,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(1f, 0.9f, 0.5f),
                division = Division.TraineesSquad,
                costumeId = 0
            },
            new HeroCostume
            {
                name = "annie",
                sex = Sex.Female,
                uniform_type = UniformType.CasualB,
                part_chest_object_mesh = "character_cap_casual",
                part_chest_object_texture = "aottg_hero_annie_cap_causal",
                part_chest_1_object_mesh = "character_body_blade_keeper_f",
                part_chest_1_object_texture = bodyCasualFbTexture[0],
                body_texture = bodyCasualFbTexture[0],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[5],
                eye_texture_id = 0,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(1f, 0.9f, 0.5f),
                costumeId = 1
            },
            new HeroCostume
            {
                name = "mikasa",
                sex = Sex.Female,
                uniform_type = UniformType.UniformB,
                body_texture = bodyUniformFbTexture[1],
                cape = true,
                hairInfo = CostumeHair.FemaleHairs[7],
                eye_texture_id = 2,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.15f, 0.15f, 0.145f),
                division = Division.TheSurveryCorps,
                costumeId = 2
            },
            new HeroCostume
            {
                name = "mikasa",
                sex = Sex.Female,
                uniform_type = UniformType.UniformB,
                part_chest_skinned_cloth_mesh = "mikasa_asset_uni",
                part_chest_skinned_cloth_texture = bodyUniformFbTexture[1],
                body_texture = bodyUniformFbTexture[1],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[7],
                eye_texture_id = 2,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.15f, 0.15f, 0.145f),
                division = Division.TraineesSquad,
                costumeId = 3
            },
            new HeroCostume
            {
                name = "mikasa",
                sex = Sex.Female,
                uniform_type = UniformType.CasualB,
                part_chest_skinned_cloth_mesh = "mikasa_asset_cas",
                part_chest_skinned_cloth_texture = bodyCasualFbTexture[1],
                part_chest_1_object_mesh = "character_body_blade_keeper_f",
                part_chest_1_object_texture = bodyCasualFbTexture[1],
                body_texture = bodyCasualFbTexture[1],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[7],
                eye_texture_id = 2,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.15f, 0.15f, 0.145f),
                costumeId = 4
            },
            new HeroCostume
            {
                name = "levi",
                sex = Sex.Male,
                uniform_type = UniformType.UniformB,
                body_texture = bodyUniformMbTexture[1],
                cape = true,
                hairInfo = CostumeHair.MaleHairs[7],
                eye_texture_id = 1,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.295f, 0.295f, 0.275f),
                division = Division.TheSurveryCorps,
                costumeId = 11
            },
            new HeroCostume
            {
                name = "levi",
                sex = Sex.Male,
                uniform_type = UniformType.CasualB,
                body_texture = bodyCasualMbTexture[1],
                part_chest_1_object_mesh = "character_body_blade_keeper_m",
                part_chest_1_object_texture = bodyCasualMbTexture[1],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[7],
                eye_texture_id = 1,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.295f, 0.295f, 0.275f),
                costumeId = 12
            },
            new HeroCostume
            {
                name = "eren",
                sex = Sex.Male,
                uniform_type = UniformType.UniformB,
                body_texture = bodyUniformMbTexture[0],
                cape = true,
                hairInfo = CostumeHair.MaleHairs[4],
                eye_texture_id = 3,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.295f, 0.295f, 0.275f),
                division = Division.TheSurveryCorps,
                costumeId = 13
            },
            new HeroCostume
            {
                name = "eren",
                sex = Sex.Male,
                uniform_type = UniformType.UniformB,
                body_texture = bodyUniformMbTexture[0],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[4],
                eye_texture_id = 3,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.295f, 0.295f, 0.275f),
                division = Division.TraineesSquad,
                costumeId = 13
            },
            new HeroCostume
            {
                name = "eren",
                sex = Sex.Male,
                uniform_type = UniformType.CasualB,
                body_texture = bodyCasualMbTexture[0],
                part_chest_1_object_mesh = "character_body_blade_keeper_m",
                part_chest_1_object_texture = bodyCasualMbTexture[0],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[4],
                eye_texture_id = 3,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.295f, 0.295f, 0.275f),
                costumeId = 14
            },
            new HeroCostume
            {
                name = "sasha",
                sex = Sex.Female,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformFaTexture[1],
                cape = true,
                hairInfo = CostumeHair.FemaleHairs[10],
                eye_texture_id = 4,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.45f, 0.33f, 0.255f),
                division = Division.TheSurveryCorps,
                costumeId = 5
            },
            new HeroCostume
            {
                name = "sasha",
                sex = Sex.Female,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformFaTexture[1],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[10],
                eye_texture_id = 4,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.45f, 0.33f, 0.255f),
                division = Division.TraineesSquad,
                costumeId = 5
            },
            new HeroCostume
            {
                name = "sasha",
                sex = Sex.Female,
                uniform_type = UniformType.CasualA,
                body_texture = bodyCasualFaTexture[1],
                part_chest_1_object_mesh = "character_body_blade_keeper_f",
                part_chest_1_object_texture = bodyCasualFaTexture[1],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[10],
                eye_texture_id = 4,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.45f, 0.33f, 0.255f),
                costumeId = 6
            },
            new HeroCostume
            {
                name = "hanji",
                sex = Sex.Female,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformFaTexture[2],
                cape = true,
                hairInfo = CostumeHair.FemaleHairs[6],
                eye_texture_id = 5,
                beard_texture_id = 33,
                glass_texture_id = 49,
                skin_color = 1,
                hair_color = new Color(0.45f, 0.33f, 0.255f),
                division = Division.TheSurveryCorps,
                costumeId = 7
            },
            new HeroCostume
            {
                name = "hanji",
                sex = Sex.Female,
                uniform_type = UniformType.CasualA,
                body_texture = bodyCasualFaTexture[2],
                part_chest_1_object_mesh = "character_body_blade_keeper_f",
                part_chest_1_object_texture = bodyCasualFaTexture[2],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[6],
                eye_texture_id = 5,
                beard_texture_id = 33,
                glass_texture_id = 49,
                skin_color = 1,
                hair_color = new Color(0.295f, 0.23f, 0.17f),
                costumeId = 8
            },
            new HeroCostume
            {
                name = "rico",
                sex = Sex.Female,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformFaTexture[0],
                cape = true,
                hairInfo = CostumeHair.FemaleHairs[9],
                eye_texture_id = 6,
                beard_texture_id = 33,
                glass_texture_id = 48,
                skin_color = 1,
                hair_color = new Color(1f, 1f, 1f),
                division = Division.TheGarrison,
                costumeId = 9
            },
            new HeroCostume
            {
                name = "rico",
                sex = Sex.Female,
                uniform_type = UniformType.CasualA,
                body_texture = bodyCasualFaTexture[0],
                part_chest_1_object_mesh = "character_body_blade_keeper_f",
                part_chest_1_object_texture = bodyCasualFaTexture[0],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[9],
                eye_texture_id = 6,
                beard_texture_id = 33,
                glass_texture_id = 48,
                skin_color = 1,
                hair_color = new Color(1f, 1f, 1f),
                costumeId = 10
            },
            new HeroCostume
            {
                name = "jean",
                sex = Sex.Male,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformMaTexture[1],
                cape = true,
                hairInfo = CostumeHair.MaleHairs[6],
                eye_texture_id = 7,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.94f, 0.84f, 0.6f),
                division = Division.TheSurveryCorps,
                costumeId = 15
            },
            new HeroCostume
            {
                name = "jean",
                sex = Sex.Male,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformMaTexture[1],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[6],
                eye_texture_id = 7,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.94f, 0.84f, 0.6f),
                division = Division.TraineesSquad,
                costumeId = 15
            },
            new HeroCostume
            {
                name = "jean",
                sex = Sex.Male,
                uniform_type = UniformType.CasualA,
                body_texture = bodyCasualMaTexture[1],
                part_chest_1_object_mesh = "character_body_blade_keeper_m",
                part_chest_1_object_texture = bodyCasualMaTexture[1],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[6],
                eye_texture_id = 7,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.94f, 0.84f, 0.6f),
                costumeId = 16
            },
            new HeroCostume
            {
                name = "marco",
                sex = Sex.Male,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformMaTexture[2],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[8],
                eye_texture_id = 8,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.295f, 0.295f, 0.275f),
                division = Division.TraineesSquad,
                costumeId = 17
            },
            new HeroCostume
            {
                name = "marco",
                sex = Sex.Male,
                uniform_type = UniformType.CasualA,
                body_texture = bodyCasualMaTexture[2],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[8],
                eye_texture_id = 8,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.295f, 0.295f, 0.275f),
                costumeId = 18
            },
            new HeroCostume
            {
                name = "mike",
                sex = Sex.Male,
                uniform_type = UniformType.UniformB,
                body_texture = bodyUniformMbTexture[3],
                cape = true,
                hairInfo = CostumeHair.MaleHairs[9],
                eye_texture_id = 9,
                beard_texture_id = 32,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.94f, 0.84f, 0.6f),
                division = Division.TheSurveryCorps,
                costumeId = 19
            },
            new HeroCostume
            {
                name = "mike",
                sex = Sex.Male,
                uniform_type = UniformType.CasualB,
                body_texture = bodyCasualMbTexture[3],
                part_chest_1_object_mesh = "character_body_blade_keeper_m",
                part_chest_1_object_texture = bodyCasualMbTexture[3],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[9],
                eye_texture_id = 9,
                beard_texture_id = 32,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.94f, 0.84f, 0.6f),
                division = Division.TheSurveryCorps,
                costumeId = 20
            },
            new HeroCostume
            {
                name = "connie",
                sex = Sex.Male,
                uniform_type = UniformType.UniformB,
                body_texture = bodyUniformMbTexture[2],
                cape = true,
                hairInfo = CostumeHair.MaleHairs[10],
                eye_texture_id = 10,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                division = Division.TheSurveryCorps,
                costumeId = 21
            },
            new HeroCostume
            {
                name = "connie",
                sex = Sex.Male,
                uniform_type = UniformType.UniformB,
                body_texture = bodyUniformMbTexture[2],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[10],
                eye_texture_id = 10,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                division = Division.TraineesSquad,
                costumeId = 21
            },
            new HeroCostume
            {
                name = "connie",
                sex = Sex.Male,
                uniform_type = UniformType.CasualB,
                body_texture = bodyCasualMbTexture[2],
                part_chest_1_object_mesh = "character_body_blade_keeper_m",
                part_chest_1_object_texture = bodyCasualMbTexture[2],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[10],
                eye_texture_id = 10,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                costumeId = 22
            },
            new HeroCostume
            {
                name = "armin",
                sex = Sex.Male,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformMaTexture[0],
                cape = true,
                hairInfo = CostumeHair.MaleHairs[5],
                eye_texture_id = 11,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.95f, 0.8f, 0.5f),
                division = Division.TheSurveryCorps,
                costumeId = 23
            },
            new HeroCostume
            {
                name = "armin",
                sex = Sex.Male,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformMaTexture[0],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[5],
                eye_texture_id = 11,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.95f, 0.8f, 0.5f),
                division = Division.TraineesSquad,
                costumeId = 23
            },
            new HeroCostume
            {
                name = "armin",
                sex = Sex.Male,
                uniform_type = UniformType.CasualA,
                body_texture = bodyCasualMaTexture[0],
                part_chest_1_object_mesh = "character_body_blade_keeper_m",
                part_chest_1_object_texture = bodyCasualMaTexture[0],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[5],
                eye_texture_id = 11,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.95f, 0.8f, 0.5f),
                costumeId = 24
            },
            new HeroCostume
            {
                name = "petra",
                sex = Sex.Female,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformFaTexture[0],
                cape = true,
                hairInfo = CostumeHair.FemaleHairs[8],
                eye_texture_id = 27,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(1f, 0.725f, 0.376f),
                division = Division.TheSurveryCorps,
                costumeId = 9
            },
            new HeroCostume
            {
                name = "petra",
                sex = Sex.Female,
                uniform_type = UniformType.CasualA,
                body_texture = bodyCasualFaTexture[0],
                part_chest_1_object_mesh = "character_body_blade_keeper_f",
                part_chest_1_object_texture = bodyCasualFaTexture[0],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[8],
                eye_texture_id = 27,
                beard_texture_id = -1,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(1f, 0.725f, 0.376f),
                division = Division.TheSurveryCorps,
                costumeId = 10
            },
            new HeroCostume
            {
                name = "custom",
                sex = Sex.Female,
                uniform_type = UniformType.CasualB,
                part_chest_skinned_cloth_mesh = "mikasa_asset_cas",
                part_chest_skinned_cloth_texture = bodyCasualFbTexture[1],
                part_chest_1_object_mesh = "character_body_blade_keeper_f",
                part_chest_1_object_texture = bodyCasualFbTexture[1],
                body_texture = bodyCasualFbTexture[1],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[2],
                eye_texture_id = 12,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.15f, 0.15f, 0.145f),
                costumeId = 4
            },
            new HeroCostume
            {
                name = "custom",
                sex = Sex.Male,
                uniform_type = UniformType.CasualA,
                body_texture = bodyCasualMaTexture[0],
                part_chest_1_object_mesh = "character_body_blade_keeper_m",
                part_chest_1_object_texture = bodyCasualMaTexture[0],
                cape = false,
                hairInfo = CostumeHair.MaleHairs[3],
                eye_texture_id = 26,
                beard_texture_id = 44,
                glass_texture_id = -1,
                skin_color = 1,
                hair_color = new Color(0.41f, 1f, 0f),
                costumeId = 24
            },
            new HeroCostume
            {
                name = "custom",
                sex = Sex.Female,
                uniform_type = UniformType.UniformA,
                body_texture = bodyUniformFaTexture[1],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[4],
                eye_texture_id = 22,
                beard_texture_id = 33,
                glass_texture_id = 56,
                skin_color = 1,
                hair_color = new Color(0f, 1f, 0.874f),
                costumeId = 5
            },
            new HeroCostume
            {
                name = "feng",
                sex = Sex.Male,
                uniform_type = UniformType.CasualB,
                body_texture = bodyCasualMbTexture[3],
                part_chest_1_object_mesh = "character_body_blade_keeper_m",
                part_chest_1_object_texture = bodyCasualMbTexture[3],
                cape = true,
                hairInfo = CostumeHair.MaleHairs[10],
                eye_texture_id = 25,
                beard_texture_id = 39,
                glass_texture_id = 53,
                skin_color = 1,
                division = Division.TheSurveryCorps,
                costumeId = 20
            },
            new HeroCostume
            {
                name = "AHSS",
                sex = Sex.Male,
                uniform_type = UniformType.CasualAHSS,
                body_texture = bodyCasualMaTexture[0] + "_ahss",
                cape = false,
                hairInfo = CostumeHair.MaleHairs[6],
                eye_texture_id = 25,
                beard_texture_id = 39,
                glass_texture_id = 53,
                skin_color = 3,
                division = Division.TheMilitaryPolice,
                costumeId = 25
            },
            new HeroCostume
            {
                name = "AHSS (F)",
                sex = Sex.Female,
                uniform_type = UniformType.CasualAHSS,
                body_texture = bodyCasualFaTexture[0],
                cape = false,
                hairInfo = CostumeHair.FemaleHairs[6],
                eye_texture_id = 2,
                beard_texture_id = 33,
                glass_texture_id = -1,
                skin_color = 3,
                division = Division.TheMilitaryPolice,
                costumeId = 26
            },

        };
        for (int i = 0; i < costume.Length; i++)
        {
            costume[i].stat = HeroStat.CustomDefault;
            costume[i].id = i;
            costume[i].setMesh2();
            costume[i].setTexture();
        }

        costumeOption = new[]
        {
            costume[0], costume[2], costume[3], costume[4], costume[5], costume[11], costume[13], costume[14],
            costume[15], costume[16], costume[17], costume[6], costume[7], costume[8], costume[10], costume[18],
            costume[19], costume[21], costume[22], costume[23], costume[24], costume[25], costume[27], costume[28],
            costume[30], costume[37], costume[38]
        };
    }

    public void setBodyByCostumeId(int id = -1)
    {
        if (id == -1)
        {
            id = this.costumeId;
        }
        this.costumeId = id;
        this.arm_l_mesh = costumeOption[id].arm_l_mesh;
        this.arm_r_mesh = costumeOption[id].arm_r_mesh;
        this.body_mesh = costumeOption[id].body_mesh;
        this.body_texture = costumeOption[id].body_texture;
        this.uniform_type = costumeOption[id].uniform_type;
        this.part_chest_1_object_mesh = costumeOption[id].part_chest_1_object_mesh;
        this.part_chest_1_object_texture = costumeOption[id].part_chest_1_object_texture;
        this.part_chest_object_mesh = costumeOption[id].part_chest_object_mesh;
        this.part_chest_object_texture = costumeOption[id].part_chest_object_texture;
        this.part_chest_skinned_cloth_mesh = costumeOption[id].part_chest_skinned_cloth_mesh;
        this.part_chest_skinned_cloth_texture = costumeOption[id].part_chest_skinned_cloth_texture;
    }

    public void setCape()
    {
        if (this.cape)
        {
            this.cape_mesh = "character_cape";
        }
        else
        {
            this.cape_mesh = string.Empty;
        }
    }
    
    public void setMesh2()
    {
        this.brand1_mesh = string.Empty;
        this.brand2_mesh = string.Empty;
        this.brand3_mesh = string.Empty;
        this.brand4_mesh = string.Empty;
        this.hand_l_mesh = "character_hand_l";
        this.hand_r_mesh = "character_hand_r";
        this.mesh_3dmg = "character_3dmg";
        this.mesh_3dmg_belt = "character_3dmg_belt";
        this.mesh_3dmg_gas_l = "character_3dmg_gas_l";
        this.mesh_3dmg_gas_r = "character_3dmg_gas_r";
        this.weapon_l_mesh = "character_blade_l";
        this.weapon_r_mesh = "character_blade_r";
        switch (this.uniform_type)
        {
            case UniformType.CasualAHSS:
                this.hand_l_mesh = "character_hand_l_ah";
                this.hand_r_mesh = "character_hand_r_ah";
                this.arm_l_mesh = "character_arm_casual_l_ah";
                this.arm_r_mesh = "character_arm_casual_r_ah";
                if (this.sex == Sex.Female)
                {
                    this.body_mesh = "character_body_casual_FA";
                }
                else
                {
                    this.body_mesh = "character_body_casual_MA";
                }
                this.mesh_3dmg = "character_3dmg_2";
                this.mesh_3dmg_belt = string.Empty;
                this.mesh_3dmg_gas_l = "character_gun_mag_l";
                this.mesh_3dmg_gas_r = "character_gun_mag_r";
                this.weapon_l_mesh = "character_gun_l";
                this.weapon_r_mesh = "character_gun_r";
                break;
            case UniformType.UniformA:
                this.arm_l_mesh = "character_arm_uniform_l";
                this.arm_r_mesh = "character_arm_uniform_r";
                this.brand1_mesh = "character_brand_arm_l";
                this.brand2_mesh = "character_brand_arm_r";
                if (this.sex == Sex.Female)
                {
                    this.body_mesh = "character_body_uniform_FA";
                    this.brand3_mesh = "character_brand_chest_f";
                    this.brand4_mesh = "character_brand_back_f";
                }
                else
                {
                    this.body_mesh = "character_body_uniform_MA";
                    this.brand3_mesh = "character_brand_chest_m";
                    this.brand4_mesh = "character_brand_back_m";
                }

                break;
            case UniformType.UniformB:
                this.arm_l_mesh = "character_arm_uniform_l";
                this.arm_r_mesh = "character_arm_uniform_r";
                this.brand1_mesh = "character_brand_arm_l";
                this.brand2_mesh = "character_brand_arm_r";
                if (this.sex == Sex.Female)
                {
                    this.body_mesh = "character_body_uniform_FB";
                    this.brand3_mesh = "character_brand_chest_f";
                    this.brand4_mesh = "character_brand_back_f";
                }
                else
                {
                    this.body_mesh = "character_body_uniform_MB";
                    this.brand3_mesh = "character_brand_chest_m";
                    this.brand4_mesh = "character_brand_back_m";
                }

                break;
            case UniformType.CasualA:
                this.arm_l_mesh = "character_arm_casual_l";
                this.arm_r_mesh = "character_arm_casual_r";
                if (this.sex == Sex.Female)
                {
                    this.body_mesh = "character_body_casual_FA";
                }
                else
                {
                    this.body_mesh = "character_body_casual_MA";
                }

                break;
            case UniformType.CasualB:
                this.arm_l_mesh = "character_arm_casual_l";
                this.arm_r_mesh = "character_arm_casual_r";
                if (this.sex == Sex.Female)
                {
                    this.body_mesh = "character_body_casual_FB";
                }
                else
                {
                    this.body_mesh = "character_body_casual_MB";
                }

                break;
        }
        if (this.hairInfo.Texture.Length > 0)
        {
            this.hair_mesh = this.hairInfo.Texture;
        }
        if (this.hairInfo.HasClothes)
        {
            this.hair_1_mesh = this.hairInfo.Clothes;
        }
        if (this.eye_texture_id >= 0)
        {
            this.eye_mesh = "character_eye";
        }
        if (this.beard_texture_id >= 0)
        {
            this.beard_mesh = "character_face";
        }
        else
        {
            this.beard_mesh = string.Empty;
        }
        if (this.glass_texture_id >= 0)
        {
            this.glass_mesh = "glass";
        }
        else
        {
            this.glass_mesh = string.Empty;
        }
        this.setCape();
    }

    public void setTexture()
    {
        if (this.uniform_type == UniformType.CasualAHSS)
            this._3dmg_texture = "aottg_hero_AHSS_3dmg";
        else
            this._3dmg_texture = "AOTTG_HERO_3DMG";
        
        this.face_texture = "aottg_hero_eyes";
        
        switch (this.division)
        {
            case Division.TheMilitaryPolice:
                this.brand_texture = "aottg_hero_brand_mp";
                break;
            case Division.TheGarrison:
                this.brand_texture = "aottg_hero_brand_g";
                break;
            case Division.TheSurveryCorps:
                this.brand_texture = "aottg_hero_brand_sc";
                break;
            case Division.TraineesSquad:
                this.brand_texture = "aottg_hero_brand_ts";
                break;
        }

        switch (this.skin_color)
        {
            case 1:
                this.skin_texture = "aottg_hero_skin_1";
                break;
            case 2:
                this.skin_texture = "aottg_hero_skin_2";
                break;
            case 3:
                this.skin_texture = "aottg_hero_skin_3";
                break;
        }
    }
}

