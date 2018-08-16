using System;
using System.Linq;
using UnityEngine;

namespace Mod.Interface
{
    public class Scoreboard : Gui
    {
        private const string EntryLayout = "<color=#672A42>[<b><color=#DC3052>{0}</color></b>] {10}{1}{2} <b><color=#6E8EEB>{3}</color></b>{9}  <color=#31A5E4>{4}k</color>|<color=#31A5E4>{5}d</color>|<color=#31A5E4>max {6}</color>|<color=#31A5E4>tot {7}</color>|<color=#31A5E4>avg {8}</color></color>";

        protected override void Render()
        {
            if (!PhotonNetwork.inRoom) return;
            SmartRect rect = new SmartRect(0, -14, Screen.width * 0.35f, 20);
            foreach (var player in PhotonNetwork.playerList.OrderBy(x => x.ID).ToList())
                GUI.Label(rect.OY(15), Entry(player));
        }

        private static string Entry(Player player)
        {
            string playerName = player.Properties.Name.Trim() == string.Empty ? "Unknown" : player.HexName, humanType;
            int type;
            if (FengGameManagerMKII.ignoreList.Contains(player.ID))
                type = 4;
            else if (player.Properties.Alive == false)
                type = 3;
            else if (player.Properties.IsAHSS == true)
                type = 1;
            else if (player.Properties.PlayerType == PlayerType.Titan)
                type = 2;
            else
                type = 0;
            

            switch (type)
            {
                case 0:
                    humanType = string.Empty;
                    break;
                case 1:
                    humanType = "[<b><color=#C42057>A</color></b>]";
                    break;
                case 2:
                    humanType = "[<b><color=#FD5079>T</color></b>]";
                    break;
                case 3:
                    humanType = "[<b><color=#FF3A3A>DEAD</color></b>]";
                    break;
                case 4:
                    humanType = "[<b><color=#890000>IGNORED</color></b>]";
                    break;
                default:
                    humanType = "[<i><color=#670018>NULL</color></i>]";
                    break;
            }
            var mod = string.Empty;
            if (player.Has("AoTTG_Mod"))
                mod = "|<b><color=#0089FF>Hawk</color></b>| ";
            else if (player.Has("HawkUser"))
                mod = "|<b><color=#00B7FF>HawkUser</color></b>| ";
            else if (player.Has("AlphaX"))
                mod = "|<b><color=#00D5FF>AlphaX</color></b>| ";
            else if (player.Has("Alpha"))
                mod = "|<b><color=#00FFF3>Alpha</color></b>| ";
            else if (player.Has("coin") || player.Has("UPublica2") || player.Has("Hats"))
                mod = "|<b><color=#00FFD5>Universe</color></b>| ";
            else if (player.Has("PBCheater"))
                mod = "|<b><color=#00FFAF>PedoBear</color></b>| ";
            else if (player.Has("SRC"))
                mod = "|<b><color=#00FF6F>SRC</color></b>| ";
            else if (player.Has("RCteam"))
                mod = "|<b><color=#00FF11>RC</color></b>| ";

            int averangeDmg = player.Properties.Kills > 0 ? (int) Math.Floor((decimal)player.Properties.TotalDamage / player.Properties.Kills) : 0;

            return string.Format(EntryLayout,
                player.ID, 
                mod,
                humanType,
                playerName,
                player.Properties.Kills,
                player.Properties.Deaths,
                player.Properties.MaxDamage,
                player.Properties.TotalDamage,
                averangeDmg,
                player.name != string.Empty ? " | " + player.name : "",
                player.IsMasterClient ? "|<b><color=#8DFF00>MC</color></b>| " : ""
            );
        }
    }
}
