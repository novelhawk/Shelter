using System;
using System.Text;
using Photon;
using UnityEngine;
using Animator = Mod.Animation.Animator;

namespace Mod.Interface
{
    public class Scoreboard : Gui
    {
        protected override void Render()
        {
            if (!PhotonNetwork.inRoom) return;

            _maxWidth = 0;
            
            // Painfully deoptimised, TODO: Improve Scoreboard 
            float y = 15;
            foreach (var player in PhotonNetwork.PlayerList) // O(3n)
            {
                DrawPlayerInfo(player, y);
                y += 15;
            }

            DrawLegenda();
            y = 15;
            foreach (var player in PhotonNetwork.PlayerList) // O(n + nx)
            {
                DrawPlayerScores(player, y);
                y += 15;
            }
        }

        private void DrawLegenda()
        {
            GUI.Label(new Rect(40, 0, _maxWidth, 20), "Player Info", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter});
            GUI.Label(new Rect(_maxWidth + 20, 0, Screen.width, 20), " K/D       max    avg    tot");
        }

        private void DrawPlayerInfo(in Player player, in float y)
        {
            // Compiler computes all const string concatenation.
            // It looks ugly tho; If we want performance we can switch to it
            const string SymbolsColor = "#A97AFF";
            const string IDColor = "#94DC79";
            const string MCColor = "#8DFF00";
            const string GuestColor = "#A0A0A0BB";
            
            
            StringBuilder builder = new StringBuilder(120);
            builder.AppendFormat("<color={0}>[<b><color={1}>{2:00}</color></b>] ", SymbolsColor, IDColor, player.ID);
            
            builder.AppendFormat("<b><color={0}>{1}</color></b> ", GuestColor, player.Properties.HexName ?? "Unknown");
            if (!string.IsNullOrEmpty(player.FriendName))
                builder.AppendFormat("| {0} ", player.FriendName);

            if (player.IsMasterClient)
                builder.Append("|<b><color=" + MCColor + ">MC</color></b>| ");
            builder.Append(HumanTypeToString(player));
            builder.Append(ModToString(player.Mod));
            builder.Append("</color>");

            var playerInfo = builder.ToString();

            var content = new GUIContent(playerInfo);
            GUI.Label(new Rect(0, y, Screen.width * 0.25f, Screen.height * 0.45f), content);
            
            GUI.skin.label.CalcMinMaxWidth(content, out _, out float max);
            _maxWidth = Mathf.Max(max, _maxWidth);
        }

        private float _maxWidth;
        private void DrawPlayerScores(in Player player, in float y)
        {
            const string NumbersColor = "#5BC0FF";
            const string TextColor = "#A97AFF";
            
            StringBuilder builder = new StringBuilder();
            builder.Append("<color=" + TextColor + ">");
            {
                builder.AppendFormat("<color={0}>{1,2}</color>/<color={0}>{2,-2}</color>", NumbersColor, player.Properties.Kills, 
                                                                                                    player.Properties.Deaths);
                builder.Append(' ', 3);

                if (player.Properties.MaxDamage > 0)
                {
                    builder.AppendFormat("<color={0}>{1,6}</color>", NumbersColor, ReadableDamage(player.Properties.MaxDamage));
                    builder.Append(' ');
                }

                if (player.Properties.Kills > 1 && player.Properties.TotalDamage > 0)
                {
                    builder.AppendFormat("<color={0}>{1,6}</color>", NumbersColor, ReadableDamage(player.Properties.TotalDamage / player.Properties.Kills));
                    builder.Append(' ');

                    builder.AppendFormat("<color={0}>{1,6}</color>", NumbersColor, ReadableDamage(player.Properties.TotalDamage));
                    builder.Append(' ');
                }
            }
            builder.Append("</color>");
            
            GUI.Label(new Rect(_maxWidth + 20, y, Screen.width * 0.25f, 20), builder.ToString());
        }

        private static string ModToString(in CustomMod mod)
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
                    return $"|<b><color=#{Shelter.Animation.ToHex()}>Shelter</color></b>| ";
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

        private static string HumanTypeToString(in Player player)
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
    }
}
