﻿using ExitGames.Client.Photon;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Commands
{
    public class CommandResetkd : Command
    {
        public override string CommandName => "resetkd";

        public override void Execute(string[] args)
        {
            Hashtable hashtable = new Hashtable
            {
                {PhotonPlayerProperty.kills, 0},
                {PhotonPlayerProperty.deaths, 0},
                {PhotonPlayerProperty.max_dmg, 0},
                {PhotonPlayerProperty.total_dmg, 0}
            };

            if (args.Length > 0 && args[0].EqualsIgnoreCase("all"))
            {
                foreach (PhotonPlayer p in PhotonNetwork.playerList)
                    p.SetCustomProperties(hashtable);
                Chat.System("You resetted the kds of everyone.");
            }
            else
            {
                if (args.Length < 1 || args[0] == PhotonPlayer.Self.ID.ToString())
                {
                    PhotonPlayer.Self.SetCustomProperties(hashtable);
                    Chat.System("You resetted your kd.");
                }
                else
                {
                    if (!PhotonPlayer.TryParse(args[0], out PhotonPlayer player))
                        throw new PlayerNotFoundException(args[0]);

                    player.SetCustomProperties(hashtable);
                    Chat.System($"You resetted {player}'s kd");
                }
            }
        }
    }
}