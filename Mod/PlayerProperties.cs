using ExitGames.Client.Photon;
using UnityEngine;

namespace Mod
{
    // ReSharper disable MemberCanBePrivate.Global
    public class PlayerProperties : Hashtable
    {
        public string Name => this[PlayerProperty.Name] as string;
        public string Guild => this[PlayerProperty.Guild] as string;
        public string FriendName => this[(byte) 0xFF] as string;

        public PlayerType PlayerType
        {
            get
            {
                if (!(this[PlayerProperty.IsTitan] is bool isTitan))
                    return PlayerType.Unknown;
                if (isTitan)
                    return PlayerType.Titan;
                return PlayerType.Human;
            }
        }

        public string Character => this[PlayerProperty.Character] as string; // TODO: Change to Mod.Hero
        public bool? HasCape => this[PlayerProperty.HasCape] as bool?;
        public string CurrentLevel => this[PlayerProperty.CurrentLevel] as string;
        public int? CostumeID => this[PlayerProperty.CostumeId] as int?;
        public int? BeardTextureID => this[PlayerProperty.BeardTextureId] as int?;
        public string BodyTexture => this[PlayerProperty.BodyTexture] as string;
        public bool? CustomBool => this[PlayerProperty.RCBool] as bool?;
        public float? CustomFloat => this[PlayerProperty.RCFloat] as float?;
        public int? CustomInt => this[PlayerProperty.RCInt] as int?;
        public string CustomString => this[PlayerProperty.RCString] as string;
        public bool? IsAHSS => Character?.EqualsIgnoreCase("AHSS");
        public bool? IsDead => this[PlayerProperty.Dead] as bool?;
        public int? Deaths => this[PlayerProperty.Deaths] as int?;
        public int? Division => this[PlayerProperty.Division] as int?;
        public int? EyeTextureID => this[PlayerProperty.EyeTextureId] as int?;
        public int? GlassTextureID => this[PlayerProperty.GlassTextureId] as int?;

        public Color? HairColor
        {
            get
            {
                if (this[PlayerProperty.HairColorR] is float r &&
                    this[PlayerProperty.HairColorG] is float g &&
                    this[PlayerProperty.HairColorB] is float b)
                    return new Color(r, g, b);
                return null;
            }
        }
        
        public int? HairInfo => this[PlayerProperty.HairInfo] as int?;
        public int? HeroCostumeID => this[PlayerProperty.HeroCostumeId] as int?;
        public int? Kills => this[PlayerProperty.Kills] as int?;
        public int? MaxDamage => this[PlayerProperty.MaxDamage] as int?;
        public float? RCBombA => this[PlayerProperty.RCBombA] as float?;
        public float? RCBombB => this[PlayerProperty.RCBombB] as float?;
        public float? RCBombG => this[PlayerProperty.RCBombG] as float?;
        public float? RCBombR => this[PlayerProperty.RCBombR] as float?;
        public float? RCBombRadius => this[PlayerProperty.RCBombRadius] as float?;
        public int? RCTeam => this[PlayerProperty.RCTeam] as int?;
        public int? Sex => this[PlayerProperty.Sex] as int?;
        public int? SkinColor => this[PlayerProperty.SkinColor] as int?;
        public int? Acceleration => this[PlayerProperty.Acceleration] as int?;
        public int? Blade => this[PlayerProperty.Blade] as int?;
        public int? Gas => this[PlayerProperty.Gas] as int?;
        public int? Speed => this[PlayerProperty.Speed] as int?;
        public string Skill => this[PlayerProperty.Skill] as string;
        public int? Team => this[PlayerProperty.Team] as int?;
        public int? TotalDamage => this[PlayerProperty.TotalDamage] as int?;
    }
}
