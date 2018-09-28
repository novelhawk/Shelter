using System.Text.RegularExpressions;
using Mod.Animation;
using Mod.Exceptions;
using Mod.Interface;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandKill : Command
    {
        public override string CommandName => "kill";

        public override void Execute(string[] args)
        {
            var match = Regex.Match(Chat.LastMessage, @"[\\\/]\w+\s(?:\d+|\w+)\s(.*)");
            if (!match.Success)
                throw new CommandArgumentException(CommandName, "/kill [id|all|titans] [msg]");
            
            string msg = match.Groups[1].Value;
            if (string.IsNullOrEmpty(msg))
                msg = "Server";

            switch (args[0].ToLower())
            {
                case "all":
                {
                    foreach (Player player in PhotonNetwork.PlayerList)
                    {
                        if (player.Hero == null)
                            continue;
                        
                        player.Hero.markDie();
                        player.Hero.photonView.RPC("netDie2", PhotonTargets.All, -1, $"[{AnimationColor.Random}]{msg}[-]  ");
                    }
                    return;
                }

                case "titans":
                {
                    foreach (GameObject obj in GameObject.FindGameObjectsWithTag("titan"))
                        obj.GetComponent<TITAN>()?.photonView.RPC("netDie", PhotonTargets.All);
                    return;
                }

                default:
                {
                    if (!Player.TryParse(args[0], out Player player))
                        throw new PlayerNotFoundException(args[0]);
                    if (player.IsLocal)
                        throw new TargetCannotBeLocalException("You cannot kill yourself"); // Do I want this?

                    if (player.Hero != null)
                    {
                        player.Hero.markDie();
                        player.Hero.photonView.RPC("netDie2", PhotonTargets.All, -1, $"[{AnimationColor.Random}]{msg}[-]  ");
                    }
                    if (player.Titan != null)
                        player.Titan.photonView.RPC("netDie", PhotonTargets.All);
                    break;
                }
            }
        }
    }
}
