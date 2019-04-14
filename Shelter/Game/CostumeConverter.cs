using ExitGames.Client.Photon;
using Game.Enums;
using Mod;
using UnityEngine;

namespace Game
{
    public static class CostumeConverter
    {
        public static void HeroCostumeToLocalData(HeroCostume costume, string slot)
        {
            slot = slot.ToUpper();
            PlayerPrefs.SetInt(slot + PlayerProperty.Sex, (int) costume.sex);
            PlayerPrefs.SetInt(slot + PlayerProperty.CostumeID, costume.costumeId);
            PlayerPrefs.SetInt(slot + PlayerProperty.HeroCostumeID, costume.id);
            PlayerPrefs.SetInt(slot + PlayerProperty.HasCape, !costume.cape ? 0 : 1);
            PlayerPrefs.SetInt(slot + PlayerProperty.HairInfo, costume.hairInfo.ID);
            PlayerPrefs.SetInt(slot + PlayerProperty.EyeTextureID, costume.eye_texture_id);
            PlayerPrefs.SetInt(slot + PlayerProperty.BeardTextureId, costume.beard_texture_id);
            PlayerPrefs.SetInt(slot + PlayerProperty.GlassTextureID, costume.glass_texture_id);
            PlayerPrefs.SetInt(slot + PlayerProperty.SkinColor, costume.skin_color);
            PlayerPrefs.SetFloat(slot + PlayerProperty.HairColorR, costume.hair_color.r);
            PlayerPrefs.SetFloat(slot + PlayerProperty.HairColorG, costume.hair_color.g);
            PlayerPrefs.SetFloat(slot + PlayerProperty.HairColorB, costume.hair_color.b);
            PlayerPrefs.SetInt(slot + PlayerProperty.Division, (int) costume.division);
            PlayerPrefs.SetInt(slot + PlayerProperty.Speed, costume.stat.Speed);
            PlayerPrefs.SetInt(slot + PlayerProperty.Gas, costume.stat.Gas);
            PlayerPrefs.SetInt(slot + PlayerProperty.Blade, costume.stat.Blade);
            PlayerPrefs.SetInt(slot + PlayerProperty.Acceleration, costume.stat.Acceleration);
            PlayerPrefs.SetString(slot + PlayerProperty.Skill, costume.stat.SkillName);
        }

        public static void HeroCostumeToPhotonData(HeroCostume costume, Player player)
        {
            int costumeId = costume.costumeId;
            if (costumeId == 26)
                costumeId = 25;

            player.SetCustomProperties(new Hashtable
            {
                {PlayerProperty.Sex, (int) costume.sex},
                {PlayerProperty.HeroCostumeID, costume.id},
                {PlayerProperty.HasCape, costume.cape},
                {PlayerProperty.HairInfo, costume.hairInfo.ID},
                {PlayerProperty.EyeTextureID, costume.eye_texture_id},
                {PlayerProperty.BeardTextureId, costume.beard_texture_id},
                {PlayerProperty.GlassTextureID, costume.glass_texture_id},
                {PlayerProperty.SkinColor, costume.skin_color},
                {PlayerProperty.HairColorR, costume.hair_color.r},
                {PlayerProperty.HairColorG, costume.hair_color.g},
                {PlayerProperty.HairColorB, costume.hair_color.b},
                {PlayerProperty.Division, (int) costume.division},
                {PlayerProperty.Speed, costume.stat.Speed},
                {PlayerProperty.Gas, costume.stat.Gas},
                {PlayerProperty.Blade, costume.stat.Blade},
                {PlayerProperty.Acceleration, costume.stat.Acceleration},
                {PlayerProperty.Skill, costume.stat.SkillName},
                {PlayerProperty.CostumeID, costumeId}
            });
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
                sex = (Sex) PlayerPrefs.GetInt(slot + PlayerProperty.Sex),
                id = PlayerPrefs.GetInt(slot + PlayerProperty.HeroCostumeID),
                costumeId = PlayerPrefs.GetInt(slot + PlayerProperty.CostumeID),
                cape = PlayerPrefs.GetInt(slot + PlayerProperty.HasCape) == 1,
                hairInfo = costume.sex != Sex.Male ? CostumeHair.FemaleHairs[PlayerPrefs.GetInt(slot + PlayerProperty.HairInfo)] : CostumeHair.MaleHairs[PlayerPrefs.GetInt(slot + PlayerProperty.HairInfo)],
                eye_texture_id = PlayerPrefs.GetInt(slot + PlayerProperty.EyeTextureID),
                beard_texture_id = PlayerPrefs.GetInt(slot + PlayerProperty.BeardTextureId),
                glass_texture_id = PlayerPrefs.GetInt(slot + PlayerProperty.GlassTextureID),
                skin_color = PlayerPrefs.GetInt(slot + PlayerProperty.SkinColor),
                hair_color = new Color(PlayerPrefs.GetFloat(slot + PlayerProperty.HairColorR), PlayerPrefs.GetFloat(slot + PlayerProperty.HairColorG), PlayerPrefs.GetFloat(slot + PlayerProperty.HairColorB)),
                division = (Division) PlayerPrefs.GetInt(slot + PlayerProperty.Division),
                stat = new HeroStat()
            };
            costume.stat.Speed = PlayerPrefs.GetInt(slot + PlayerProperty.Speed);
            costume.stat.Gas = PlayerPrefs.GetInt(slot + PlayerProperty.Gas);
            costume.stat.Blade = PlayerPrefs.GetInt(slot + PlayerProperty.Blade);
            costume.stat.Acceleration = PlayerPrefs.GetInt(slot + PlayerProperty.Acceleration);
            costume.stat.SkillName = PlayerPrefs.GetString(slot + PlayerProperty.Skill);
            costume.setBodyByCostumeId();
            costume.setMesh2();
            costume.setTexture();
            return costume;
        }

        public static HeroCostume PhotonDataToHeroCostume(Player player)
        {
            var costume = new HeroCostume
            {
                sex = player.Properties.Sex,
                costumeId = player.Properties.CostumeID,
                id = player.Properties.HeroCostumeID,
                cape = player.Properties.HasCape,
                hairInfo = player.Properties.Sex != Sex.Male
                    ? CostumeHair.FemaleHairs[player.Properties.HairInfo]
                    : CostumeHair.MaleHairs[player.Properties.HairInfo],
                eye_texture_id = player.Properties.EyeTextureID,
                beard_texture_id = player.Properties.BeardTextureID,
                glass_texture_id = player.Properties.GlassTextureID,
                skin_color = player.Properties.SkinColor,
                hair_color = player.Properties.HairColor,
                division = player.Properties.Division,
                stat = player.Properties.HeroStat
            };
            if (costume.costumeId == 25 && costume.sex == Sex.Female)
                costume.costumeId = 26;
            costume.setBodyByCostumeId();
            costume.setMesh2();
            costume.setTexture();
            return costume;
        }
    }
}

