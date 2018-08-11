using ExitGames.Client.Photon;
using UnityEngine;

namespace Mod
{
    public class PlayerProperties : Hashtable
    {
        #region Fields
        public string Name { get; private set; }
        public string Guild { get; private set; }

        /**
         * Byte 255
         */
        public string FriendName { get; set; }
        public PlayerType PlayerType { get; private set; }
        public string Character { get; private set; } // TODO: Change to Mod.Hero
        public bool Cape { get; private set; }
        public string CurrentLevel { get; private set; }
        public int CostumeID { get; private set; }
        public int BeardTextureID { get; private set; }
        public string BodyTexture { get; private set; }
        public bool CustomBool { get; private set; }
        public float CustomFloat { get; private set; }
        public int CustomInt { get; private set; }
        public string CustomString { get; private set; }
        public bool IsAHSS { get; private set; }
        public bool IsDead { get; private set; }
        public int Deaths { get; private set; }
        public int Division { get; private set; }
        public int EyeTextureID { get; private set; }
        public int GlassTextureID { get; private set; }
        public Color HairColor { get; private set; }
        public int HairInfo { get; private set; }
        public int HeroCostumeID { get; private set; }
        public int Kills { get; private set; }
        public int MaxDamage { get; private set; }
        public float RCBombA { get; private set; }
        public float RCBombB { get; private set; }
        public float RCBombG { get; private set; }
        public float RCBombR { get; private set; }
        public float RCBombRadius { get; private set; }
        public int RCTeam { get; private set; }
        public int Sex { get; private set; }
        public int SkinColor { get; private set; }
        public int Acceleration { get; private set; }
        public int Blade { get; private set; }
        public int Gas { get; private set; }
        public int Speed { get; private set; }
        public string Skill { get; private set; }
        public int Team { get; private set; }
        public int TotalDamage { get; private set; }
#endregion

        public PlayerProperties()
        {
        }

        public PlayerProperties(Hashtable hashtable)
        {
            LoadHashtable();
        }

        public bool Contains(string key)
        {
            return this[key] != null;
        }

        private void LoadHashtable()
        {
            if (this[byte.MaxValue] is string friendName)
                FriendName = friendName;

            if (this["name"] is string name)
                Name = name;

            if (this["guildName"] is string guild)
                Guild = guild;

            if (this["isTitan"] is int num)
            {
                if (num == 2)
                    PlayerType = PlayerType.Titan;
                else
                    PlayerType = PlayerType.Human;
            }

            if (this["character"] is string character)
            {
                character = character.ToUpper();
                if (character == "AHSS")
                {
                    IsAHSS = true;
                    PlayerType = PlayerType.Human;
                }
                else
                    Character = character;
            }

            if (this["cape"] is bool cape)
                Cape = cape;

            if (this["currentLevel"] is string currentLevel)
                CurrentLevel = currentLevel;

            if (this["costumeId"] is int costumeId)
            {
                CostumeID = costumeId;
            }

            if (this["beard_texture_id"] is int beard_texture_id)
                BeardTextureID = beard_texture_id;

            if (this["body_texture"] is string body_texture)
                BodyTexture = body_texture;

            if (this["customBool"] is bool customBool)
                CustomBool = customBool;

            if (this["customFloat"] is float customFloat)
                CustomFloat = customFloat;

            if (this["customInt"] is int customInt)
                CustomInt = customInt;

            if (this["customString"] is string customString)
                CustomString = customString;

            if (this["dead"] is bool dead)
                IsDead = dead;

            if (this["deaths"] is int deaths)
                Deaths = deaths;

            if (this["division"] is int division)
                Division = division;

            if (this["eye_texture_id"] is int eye_texture_id)
                EyeTextureID = eye_texture_id;

            if (this["glass_texture_id"] is int glass_texture_id)
                GlassTextureID = glass_texture_id;
            
            if (this["hair_color1"] is float r && this["hair_color2"] is float g && this["hair_color3"] is float b)
                HairColor = new Color(r, g, b);

            if (this["hairInfo"] is int hairInfo)
                HairInfo = hairInfo;

            if (this["heroCostumeId"] is int heroCostumeId)
                HeroCostumeID = heroCostumeId;
    
            if (this["kills"] is int kills)
                Kills = kills;

            if (this["max_dmg"] is int max_dmg)
                MaxDamage = max_dmg;

            if (this["RCBombA"] is float bombA)
                RCBombA = bombA;

            if (this["RCBombB"] is float bombB)
                RCBombB = bombB;

            if (this["RCBombG"] is float bombG)
                RCBombG = bombG;

            if (this["RCBombR"] is float bombR)
                RCBombR = bombR;

            if (this["RCBombRadius"] is float bombRadius)
                RCBombRadius = bombRadius;

            if (this["RCteam"] is int RCteam)
                RCTeam = RCteam;

            if (this["sex"] is int sex)
                Sex = sex;

            if (this["skin_color"] is int skin_color)
                SkinColor = skin_color;

            if (this["statACL"] is int statACL)
                Acceleration = statACL;

            if (this["statBLA"] is int statBLA)
                Blade = statBLA;

            if (this["statGAS"] is int statGAS)
                Gas = statGAS;

            if (this["statSPD"] is int statSPD)
                Speed = statSPD;

            if (this["statSKILL"] is string statSKILL)
                Skill = statSKILL;

            if (this["team"] is int team)
                Team = team;

            if (this["total_dmg"] is int total_dmg)
                TotalDamage = total_dmg;

        }
    }
}
