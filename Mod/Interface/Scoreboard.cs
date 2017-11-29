using System;
using System.Linq;
using UnityEngine;

namespace Mod.Interface
{
    public class Scoreboard : Gui
    {
        private const string EntryLayout = "<color=#672A42>[<b><color=#DC3052>{0}</color></b>] {10}{1}{2} <b><color=#6E8EEB>{3}</color></b>{9}  <color=#31A5E4>{4}</color>|<color=#31A5E4>{5}</color>|<color=#31A5E4>{6}</color>|<color=#31A5E4>{6}</color>|<color=#31A5E4>{7}</color></color>";

        protected override void Render()
        {
            if (!PhotonNetwork.inRoom) return;
            Rect rect = new Rect(0, -14, Screen.width * 0.35f, 20);
            foreach (var player in PhotonNetwork.playerList.OrderBy(x => x.ID).ToList())
                GUI.Label(rect = new Rect(0, rect.y + 15, rect.width, rect.height), Entry(player));
        }

        private static string Entry(PhotonPlayer player)
        {
            object temp;
            string playerName = player.HexName.Trim() == string.Empty ? "Unknown" : (player.HexName ?? "Unknown"), humanType;
            var type = !FengGameManagerMKII.ignoreList.Contains(player.ID) ? ((temp = player.CustomProperties[PhotonPlayerProperty.dead]) != null ? ((bool)temp ? 4 : (temp = player.CustomProperties[PhotonPlayerProperty.team]) != null ? ((int)temp == 2 ? 2 : ((int)temp == 1 ? 1 : 3)) : 0) : 0) : 5;
            var kills = (temp = player.CustomProperties[PhotonPlayerProperty.kills]) != null && temp is int ? (int)temp : 0;
            var deaths = (temp = player.CustomProperties[PhotonPlayerProperty.deaths]) != null && temp is int ? (int)temp : 0;
            var maxDmg = (temp = player.CustomProperties[PhotonPlayerProperty.max_dmg]) != null && temp is int ? (int)temp : 0;
            var totDmg = (temp = player.CustomProperties[PhotonPlayerProperty.total_dmg]) != null && temp is int ? (int)temp : 0;
            var averangeDmg = totDmg > 0 && kills > 0 ? Convert.ToInt32(Math.Floor((decimal)totDmg / kills)) : 0;

            switch (type)
            {
                case 1:
                    humanType = string.Empty;
                    break;
                case 2:
                    humanType = "[<b><color=#C42057>A</color></b>]";
                    break;
                case 3:
                    humanType = "[<b><color=#FD5079>T</color></b>]";
                    break;
                case 4:
                    humanType = "[<b><color=#FF3A3A>DEAD</color></b>]";
                    break;
                case 5:
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

            return string.Format(EntryLayout,
                player.ID, 
                mod,
                humanType,
                playerName,
                kills,
                deaths,
                maxDmg,
                totDmg,
                averangeDmg,
                player.name != string.Empty ? " | " + player.name : "",
                player.IsMasterClient ? "|<b><color=#8DFF00>MC</color></b>| " : ""
            );
        }
    }
}
