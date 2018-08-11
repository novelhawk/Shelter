using ExitGames.Client.Photon;
using UnityEngine;

public class CostumeConeveter
{
    private static int DivisionToInt(DIVISION id)
    {
        if (id == DIVISION.TheGarrison)
        {
            return 0;
        }
        if (id == DIVISION.TheMilitaryPolice)
        {
            return 1;
        }
        if (id != DIVISION.TheSurveryCorps && id == DIVISION.TraineesSquad)
        {
            return 3;
        }
        return 2;
    }

    public static void HeroCostumeToLocalData(HeroCostume costume, string slot)
    {
        slot = slot.ToUpper();
        PlayerPrefs.SetInt(slot + PlayerProperty.Sex, SexToInt(costume.sex));
        PlayerPrefs.SetInt(slot + PlayerProperty.CostumeId, costume.costumeId);
        PlayerPrefs.SetInt(slot + PlayerProperty.HeroCostumeId, costume.id);
        PlayerPrefs.SetInt(slot + PlayerProperty.HasCape, !costume.cape ? 0 : 1);
        PlayerPrefs.SetInt(slot + PlayerProperty.HairInfo, costume.hairInfo.id);
        PlayerPrefs.SetInt(slot + PlayerProperty.EyeTextureId, costume.eye_texture_id);
        PlayerPrefs.SetInt(slot + PlayerProperty.BeardTextureId, costume.beard_texture_id);
        PlayerPrefs.SetInt(slot + PlayerProperty.GlassTextureId, costume.glass_texture_id);
        PlayerPrefs.SetInt(slot + PlayerProperty.SkinColor, costume.skin_color);
        PlayerPrefs.SetFloat(slot + PlayerProperty.HairColorR, costume.hair_color.r);
        PlayerPrefs.SetFloat(slot + PlayerProperty.HairColorG, costume.hair_color.g);
        PlayerPrefs.SetFloat(slot + PlayerProperty.HairColorB, costume.hair_color.b);
        PlayerPrefs.SetInt(slot + PlayerProperty.Division, DivisionToInt(costume.division));
        PlayerPrefs.SetInt(slot + PlayerProperty.Speed, costume.stat.Speed);
        PlayerPrefs.SetInt(slot + PlayerProperty.Gas, costume.stat.Gas);
        PlayerPrefs.SetInt(slot + PlayerProperty.Blade, costume.stat.Blade);
        PlayerPrefs.SetInt(slot + PlayerProperty.Acceleration, costume.stat.Acceleration);
        PlayerPrefs.SetString(slot + PlayerProperty.Skill, costume.stat.SkillName);
    }

    public static void HeroCostumeToPhotonData(HeroCostume costume, Player player)
    {
        Hashtable propertiesToSet = new Hashtable
        {
            { PlayerProperty.Sex, SexToInt(costume.sex) }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.CostumeId, costume.costumeId }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HeroCostumeId, costume.id }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HasCape, costume.cape }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HairInfo, costume.hairInfo.id }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.EyeTextureId, costume.eye_texture_id }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.BeardTextureId, costume.beard_texture_id }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.GlassTextureId, costume.glass_texture_id }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.SkinColor, costume.skin_color }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HairColorR, costume.hair_color.r }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HairColorG, costume.hair_color.g }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HairColorB, costume.hair_color.b }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Division, DivisionToInt(costume.division) }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Speed, costume.stat.Speed }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Gas, costume.stat.Gas }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Blade, costume.stat.Blade }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Acceleration, costume.stat.Acceleration }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Skill, costume.stat.SkillName }
        };
        player.SetCustomProperties(propertiesToSet);
    }

    public static void HeroCostumeToPhotonData2(HeroCostume costume, Player player)
    {
        Hashtable propertiesToSet = new Hashtable
        {
            { PlayerProperty.Sex, SexToInt(costume.sex) }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable();
        int costumeId = costume.costumeId;
        if (costumeId == 26)
        {
            costumeId = 25;
        }
        propertiesToSet.Add(PlayerProperty.CostumeId, costumeId);
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HeroCostumeId, costume.id }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HasCape, costume.cape }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HairInfo, costume.hairInfo.id }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.EyeTextureId, costume.eye_texture_id }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.BeardTextureId, costume.beard_texture_id }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.GlassTextureId, costume.glass_texture_id }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.SkinColor, costume.skin_color }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HairColorR, costume.hair_color.r }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HairColorG, costume.hair_color.g }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.HairColorB, costume.hair_color.b }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Division, DivisionToInt(costume.division) }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Speed, costume.stat.Speed }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Gas, costume.stat.Gas }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Blade, costume.stat.Blade }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Acceleration, costume.stat.Acceleration }
        };
        player.SetCustomProperties(propertiesToSet);
        propertiesToSet = new Hashtable
        {
            { PlayerProperty.Skill, costume.stat.SkillName }
        };
        player.SetCustomProperties(propertiesToSet);
    }

    private static DIVISION IntToDivision(int id)
    {
        if (id == 0)
        {
            return DIVISION.TheGarrison;
        }
        if (id == 1)
        {
            return DIVISION.TheMilitaryPolice;
        }
        if (id != 2 && id == 3)
        {
            return DIVISION.TraineesSquad;
        }
        return DIVISION.TheSurveryCorps;
    }

    private static SEX IntToSex(int id)
    {
        if (id == 0)
        {
            return SEX.FEMALE;
        }
        if (id == 1)
        {
            return SEX.MALE;
        }
        return SEX.MALE;
    }

    private static UNIFORM_TYPE IntToUniformType(int id)
    {
        if (id == 0)
        {
            return UNIFORM_TYPE.CasualA;
        }
        if (id == 1)
        {
            return UNIFORM_TYPE.CasualB;
        }
        if (id != 2)
        {
            if (id == 3)
            {
                return UNIFORM_TYPE.UniformB;
            }
            if (id == 4)
            {
                return UNIFORM_TYPE.CasualAHSS;
            }
        }
        return UNIFORM_TYPE.UniformA;
    }

    public static HeroCostume LocalDataToHeroCostume(string slot)
    {
        slot = slot.ToUpper();
        if (!PlayerPrefs.HasKey(slot + PlayerProperty.Sex))
        {
            return HeroCostume.costume[0];
        }
        HeroCostume costume = new HeroCostume();
        costume = new HeroCostume {
            sex = IntToSex(PlayerPrefs.GetInt(slot + PlayerProperty.Sex)),
            id = PlayerPrefs.GetInt(slot + PlayerProperty.HeroCostumeId),
            costumeId = PlayerPrefs.GetInt(slot + PlayerProperty.CostumeId),
            cape = PlayerPrefs.GetInt(slot + PlayerProperty.HasCape) != 1 ? false : true,
            hairInfo = costume.sex != SEX.MALE ? CostumeHair.hairsF[PlayerPrefs.GetInt(slot + PlayerProperty.HairInfo)] : CostumeHair.hairsM[PlayerPrefs.GetInt(slot + PlayerProperty.HairInfo)],
            eye_texture_id = PlayerPrefs.GetInt(slot + PlayerProperty.EyeTextureId),
            beard_texture_id = PlayerPrefs.GetInt(slot + PlayerProperty.BeardTextureId),
            glass_texture_id = PlayerPrefs.GetInt(slot + PlayerProperty.GlassTextureId),
            skin_color = PlayerPrefs.GetInt(slot + PlayerProperty.SkinColor),
            hair_color = new Color(PlayerPrefs.GetFloat(slot + PlayerProperty.HairColorR), PlayerPrefs.GetFloat(slot + PlayerProperty.HairColorG), PlayerPrefs.GetFloat(slot + PlayerProperty.HairColorB)),
            division = IntToDivision(PlayerPrefs.GetInt(slot + PlayerProperty.Division)),
            stat = new HeroStat()
        };
        costume.stat.Speed = PlayerPrefs.GetInt(slot + PlayerProperty.Speed);
        costume.stat.Gas = PlayerPrefs.GetInt(slot + PlayerProperty.Gas);
        costume.stat.Blade = PlayerPrefs.GetInt(slot + PlayerProperty.Blade);
        costume.stat.Acceleration = PlayerPrefs.GetInt(slot + PlayerProperty.Acceleration);
        costume.stat.SkillName = PlayerPrefs.GetString(slot + PlayerProperty.Skill);
        costume.setBodyByCostumeId(-1);
        costume.setMesh2();
        costume.setTexture();
        return costume;
    }

    public static HeroCostume PhotonDataToHeroCostume(Player player)
    {
        HeroCostume costume = new HeroCostume();
        costume = new HeroCostume {
            sex = IntToSex((int) player.Properties[PlayerProperty.Sex]),
            costumeId = (int) player.Properties[PlayerProperty.CostumeId],
            id = (int) player.Properties[PlayerProperty.HeroCostumeId],
            cape = (bool) player.Properties[PlayerProperty.HasCape],
            hairInfo = costume.sex != SEX.MALE ? CostumeHair.hairsF[(int) player.Properties[PlayerProperty.HairInfo]] : CostumeHair.hairsM[(int) player.Properties[PlayerProperty.HairInfo]],
            eye_texture_id = (int) player.Properties[PlayerProperty.EyeTextureId],
            beard_texture_id = (int) player.Properties[PlayerProperty.BeardTextureId],
            glass_texture_id = (int) player.Properties[PlayerProperty.GlassTextureId],
            skin_color = (int) player.Properties[PlayerProperty.SkinColor],
            hair_color = new Color((float) player.Properties[PlayerProperty.HairColorR], (float) player.Properties[PlayerProperty.HairColorG], (float) player.Properties[PlayerProperty.HairColorB]),
            division = IntToDivision((int) player.Properties[PlayerProperty.Division]),
            stat = new HeroStat()
        };
        costume.stat.Speed = (int) player.Properties[PlayerProperty.Speed];
        costume.stat.Gas = (int) player.Properties[PlayerProperty.Gas];
        costume.stat.Blade = (int) player.Properties[PlayerProperty.Blade];
        costume.stat.Acceleration = (int) player.Properties[PlayerProperty.Acceleration];
        costume.stat.SkillName = (string) player.Properties[PlayerProperty.Skill];
        costume.setBodyByCostumeId(-1);
        costume.setMesh2();
        costume.setTexture();
        return costume;
    }

    public static HeroCostume PhotonDataToHeroCostume2(Player player)
    {
        HeroCostume costume = new HeroCostume();
        SEX sex = IntToSex((int) player.Properties[PlayerProperty.Sex]);
        costume = new HeroCostume {
            sex = sex,
            costumeId = (int) player.Properties[PlayerProperty.CostumeId],
            id = (int) player.Properties[PlayerProperty.HeroCostumeId],
            cape = (bool) player.Properties[PlayerProperty.HasCape],
            hairInfo = sex != SEX.MALE ? CostumeHair.hairsF[(int) player.Properties[PlayerProperty.HairInfo]] : CostumeHair.hairsM[(int) player.Properties[PlayerProperty.HairInfo]],
            eye_texture_id = (int) player.Properties[PlayerProperty.EyeTextureId],
            beard_texture_id = (int) player.Properties[PlayerProperty.BeardTextureId],
            glass_texture_id = (int) player.Properties[PlayerProperty.GlassTextureId],
            skin_color = (int) player.Properties[PlayerProperty.SkinColor],
            hair_color = new Color((float) player.Properties[PlayerProperty.HairColorR], (float) player.Properties[PlayerProperty.HairColorG], (float) player.Properties[PlayerProperty.HairColorB]),
            division = IntToDivision((int) player.Properties[PlayerProperty.Division]),
            stat = new HeroStat()
        };
        costume.stat.Speed = (int) player.Properties[PlayerProperty.Speed];
        costume.stat.Gas = (int) player.Properties[PlayerProperty.Gas];
        costume.stat.Blade = (int) player.Properties[PlayerProperty.Blade];
        costume.stat.Acceleration = (int) player.Properties[PlayerProperty.Acceleration];
        costume.stat.SkillName = (string) player.Properties[PlayerProperty.Skill];
        if (costume.costumeId == 25 && costume.sex == SEX.FEMALE)
        {
            costume.costumeId = 26;
        }
        costume.setBodyByCostumeId(-1);
        costume.setMesh2();
        costume.setTexture();
        return costume;
    }

    private static int SexToInt(SEX id)
    {
        if (id == SEX.FEMALE)
        {
            return 0;
        }
        if (id == SEX.MALE)
        {
            return 1;
        }
        return 1;
    }

    private static int UniformTypeToInt(UNIFORM_TYPE id)
    {
        if (id == UNIFORM_TYPE.CasualA)
        {
            return 0;
        }
        if (id == UNIFORM_TYPE.CasualB)
        {
            return 1;
        }
        if (id != UNIFORM_TYPE.UniformA)
        {
            if (id == UNIFORM_TYPE.UniformB)
            {
                return 3;
            }
            if (id == UNIFORM_TYPE.CasualAHSS)
            {
                return 4;
            }
        }
        return 2;
    }
}

