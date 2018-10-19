using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Photon;
using UnityEngine;
using Animator = Mod.Animation.Animator;

namespace Mod.Interface
{
    public class Scoreboard : Gui
    {
        private const string EntryLayout = "<color=#672A42>[<b><color=#DC3052>{0}</color></b>] {10}{1}{2} <b><color=#6E8EEB>{3}</color></b>{9}  <color=#31A5E4>{4}k</color>|<color=#31A5E4>{5}d</color>|<color=#31A5E4>max {6}</color>|<color=#31A5E4>tot {7}</color>|<color=#31A5E4>avg {8}</color></color>";
        private Animator _animator;
        private float _lastAnimationUpdate;

        protected override void OnShow()
        {
            _animator = new Animator(Shelter.Animation, 20);
        }

        protected override void Render()
        {
            if (!PhotonNetwork.inRoom) return;
            SmartRect rect = new SmartRect(0, -14, Screen.width * 0.35f, 20);
            foreach (var player in PhotonNetwork.PlayerList.OrderBy(x => x.ID).ToList())
                GUI.Label(rect.OY(15), Entry(player, _animator.LastColor.ToHex()));
            
            if (Time.time - _lastAnimationUpdate < 0.05f)
                return;
            _animator.ComputeNext();
            _lastAnimationUpdate = Time.time;
        }

        protected override void OnHide()
        {
            _animator = null;
        }

        private static string Entry(Player player, string color)
        {
            string playerName = player.Properties.Name.Trim() == string.Empty ? "Unknown" : player.Properties.HexName;
            string humanType;
            
            if (player.IsIgnored)
                humanType = "[<b><color=#890000>IGNORED</color></b>]";
            else if (player.Properties.Alive == false)
                humanType = "[<b><color=#FF3A3A>DEAD</color></b>]";
            else if (player.Properties.IsAHSS == true)
                humanType = "[<b><color=#C42057>A</color></b>]";
            else switch (player.Properties.PlayerType)
            {
                case PlayerType.Human:
                    humanType = string.Empty;
                    break;
                case PlayerType.Titan:
                    humanType = "[<b><color=#FD5079>T</color></b>]";
                    break;
                default:
                    humanType = "[<i><color=#670018>NULL</color></i>]";
                    break;
            }
            
            var mod = string.Empty;
            switch (player.Mod)
            {
                case CustomMod.None:
                    break;
                case CustomMod.RC:
                    mod = "|<b><color=#00FF11>RC</color></b>| ";
                    break;
                case CustomMod.HawkMain:
                    mod = "|<b><color=#0089FF>Hawk</color></b>| ";
                    break;
                case CustomMod.HawkUser:
                    mod = "|<b><color=#00B7FF>HawkUser</color></b>| ";
                    break;
                case CustomMod.Shelter:
                    mod = $"|<b><color=#{color}>Shelter</color></b>| "; 
                    break;
                case CustomMod.AlphaX:
                    mod = "|<b><color=#00D5FF>AlphaX</color></b>| ";
                    break;
                case CustomMod.Alpha:
                    mod = "|<b><color=#00FFF3>Alpha</color></b>| ";
                    break;
                case CustomMod.Universe:
                    mod = "|<b><color=#00FFD5>Universe</color></b>| ";
                    break;
                case CustomMod.Cyan:
                    mod = "|<b><color=#00FFFF>Cyan</color></b>| ";
                    break;
                case CustomMod.Pedobear:
                    mod = "|<b><color=#00FFAF>PedoBear</color></b>| ";
                    break;
                case CustomMod.SRC:
                    mod = "|<b><color=#00FF6F>SRC</color></b>| ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
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
                player.FriendName != string.Empty ? " | " + player.FriendName : "",
                player.IsMasterClient ? "|<b><color=#8DFF00>MC</color></b>| " : ""
            );
        }
    }
}
