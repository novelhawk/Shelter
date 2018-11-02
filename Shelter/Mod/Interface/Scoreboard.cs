using System;
using System.Linq;
using System.Text;
using Photon;
using UnityEngine;
using UnityEngine.UI;
using Animator = Mod.Animation.Animator;

namespace Mod.Interface
{
    public class Scoreboard : Gui
    {
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

        private static string ModToString(CustomMod mod, string color)
        {
            switch (mod)
            {
                case CustomMod.None:
                    return string.Empty;
                case CustomMod.RC:
                    return "|<b><color=#00FF11>RC</color></b>| ";
                case CustomMod.HawkMain:
                    return "|<b><color=#0089FF>Hawk</color></b>| ";
                case CustomMod.HawkUser:
                    return "|<b><color=#00B7FF>HawkUser</color></b>| ";
                case CustomMod.Shelter:
                    return $"|<b><color=#{color}>Shelter</color></b>| ";
                case CustomMod.AlphaX:
                    return "|<b><color=#00D5FF>AlphaX</color></b>| ";
                case CustomMod.Alpha:
                    return "|<b><color=#00FFF3>Alpha</color></b>| ";
                case CustomMod.Universe:
                    return "|<b><color=#00FFD5>Universe</color></b>| ";
                case CustomMod.Cyan:
                    return "|<b><color=#00FFFF>Cyan</color></b>| ";
                case CustomMod.Pedobear:
                    return "|<b><color=#00FFAF>PedoBear</color></b>| ";
                case CustomMod.SRC:
                    return "|<b><color=#00FF6F>SRC</color></b>| ";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string HumanTypeToString(Player player)
        {
            if (player.IsIgnored)
                return "[<b><color=#890000>IGNORED</color></b>] ";
            if (player.Properties.Alive == false)
                return "[<b><color=#FF3A3A>DEAD</color></b>] ";
            switch (player.Properties.PlayerType)
            {
                case PlayerType.Human:
                    if (player.Properties.IsAHSS == true)
                        return "[<b><color=#C42057>A</color></b>] ";
                    return string.Empty;
                case PlayerType.Titan:
                    return "[<b><color=#FD5079>T</color></b>] ";
                default:
                    return "[<i><color=#670018>NULL</color></i>] ";
            }
        }

        private static string ReadableDamage(int damage)
        {
            if (damage > 10000)
                return $"{damage / 10000f:F1} M";
            if (damage > 1000)
                return $"{damage / 1000f:F1} k";
            return damage.ToString();
        }

        private static string Entry(Player player, string color)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("<color=#672A42>[<b><color=#DC3052>{0}</color></b>] ", player.ID);
            
            // Player info
            if (player.IsMasterClient)
                builder.Append("|<b><color=#8DFF00>MC</color></b>| ");
            builder.Append(HumanTypeToString(player));
            builder.Append(ModToString(player.Mod, color));
            builder.AppendFormat("<b><color=#6E8EEB>{0}</color></b> ", player.Properties.HexName ?? "Unknown");
            if (!string.IsNullOrEmpty(player.FriendName))
                builder.AppendFormat("| {0} ", player.FriendName);
            
            // Average compute
            var average = 0;
            if (player.Properties.Kills > 0)
                average = player.Properties.TotalDamage / player.Properties.Kills;
            
            // KDA            
            builder.AppendFormat("<color=#31A5E4>{0}</color>|", player.Properties.Kills);
            builder.AppendFormat("<color=#31A5E4>{0}</color>|", player.Properties.Deaths);
            builder.AppendFormat("<color=#31A5E4>{0}</color>|", ReadableDamage(player.Properties.MaxDamage));
            builder.AppendFormat("<color=#31A5E4>{0}</color>|", ReadableDamage(average));
            builder.AppendFormat("<color=#31A5E4>{0}</color></color>", ReadableDamage(player.Properties.TotalDamage));
            return builder.ToString();
        }
    }
}
