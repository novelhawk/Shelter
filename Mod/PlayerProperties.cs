using ExitGames.Client.Photon;
using UnityEngine;

namespace Mod
{
    // ReSharper disable MemberCanBePrivate.Global
    public class PlayerProperties : Hashtable
    {
        public string Name => this[PlayerProperty.Name] as string ?? "Unknown";
        public string Guild => this[PlayerProperty.Guild] as string ?? "";
        public string FriendName => this[(byte) 0xFF] as string ?? "";

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

        public int Kills => this[PlayerProperty.Kills] as int? ?? 0;
        public int Deaths => this[PlayerProperty.Deaths] as int? ?? 0;
        public int MaxDamage => this[PlayerProperty.MaxDamage] as int? ?? 0;
        public int TotalDamage => this[PlayerProperty.TotalDamage] as int? ?? 0;
        
        public bool? Alive => !IsDead;
        public bool? IsDead => this[PlayerProperty.Dead] as bool?;
        
        public int? RCTeam => this[PlayerProperty.RCTeam] as int?;
        public int? Team => this[PlayerProperty.Team] as int?;
        
        public string Character => this[PlayerProperty.Character] as string; // TODO: Change to Mod.Hero
        public Sex Sex => this[PlayerProperty.Sex] as Sex? ?? Sex.Male;
        public bool? IsAHSS => Team == 2;

        public HeroStat HeroStat
        {
            get
            {
                if (Acceleration > 150 || Blade > 125 || Gas > 150 || Speed > 140)
                    return new HeroStat(Skill, 100, 100, 100, 100);                
                return new HeroStat(Skill, Speed, Gas, Blade, Acceleration);                
            }
        }
        public string Skill => this[PlayerProperty.Skill] as string;
        public int Speed => this[PlayerProperty.Speed] as int? ?? 100;
        public int Gas => this[PlayerProperty.Gas] as int? ?? 100;
        public int Blade => this[PlayerProperty.Blade] as int? ?? 100;
        public int Acceleration => this[PlayerProperty.Acceleration] as int? ?? 100;
        
        public string CurrentLevel => this[PlayerProperty.CurrentLevel] as string;
        
        public Division Division => this[PlayerProperty.Division] as Division? ?? 0;
        public bool HasCape => this[PlayerProperty.HasCape] as bool? ?? false;
        public int CostumeID => this[PlayerProperty.CostumeID] as int? ?? 0;
        public int BeardTextureID => this[PlayerProperty.BeardTextureId] as int? ?? 0;
        public string BodyTexture => this[PlayerProperty.BodyTexture] as string;
        public int EyeTextureID => this[PlayerProperty.EyeTextureID] as int? ?? 0;
        public int GlassTextureID => this[PlayerProperty.GlassTextureID] as int? ?? 0;
        public int HairInfo => this[PlayerProperty.HairInfo] as int? ?? 0;
        public int HeroCostumeID => this[PlayerProperty.HeroCostumeID] as int? ?? 0;
        public int SkinColor => this[PlayerProperty.SkinColor] as int? ?? 0;

        public bool? CustomBool => this[PlayerProperty.RCBool] as bool?;
        public float? CustomFloat => this[PlayerProperty.RCFloat] as float?;
        public int? CustomInt => this[PlayerProperty.RCInt] as int?;
        public string CustomString => this[PlayerProperty.RCString] as string;
        
        public Color HairColor
        {
            get
            {
                if (this[PlayerProperty.HairColorR] is float r &&
                    this[PlayerProperty.HairColorG] is float g &&
                    this[PlayerProperty.HairColorB] is float b)
                    return new Color(r, g, b);
                return new Color(1, 1, 1, 1);
            }
        }


        public Color RCBombColor
        {
            get
            {
                if (this[PlayerProperty.RCBombR] is float r &&
                    this[PlayerProperty.RCBombG] is float g &&
                    this[PlayerProperty.RCBombB] is float b &&
                    this[PlayerProperty.RCBombA] is float a)
                    return new Color(r, g, b, a);
                return new Color(1, 1, 1, 0.5f);
            }
        }

        public float RCBombRadius 
        {
            get
            {
                if (this[PlayerProperty.RCBombRadius] is float radius)
                    return Mathf.Clamp(radius, 20, 60);
                return 40;
            }
        }
    }
}
