using System.Text.RegularExpressions;
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
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/kill [id|all|titans] [msg]");
            string msg = Regex.Match(Chat.Message, @"[\\\/]\w+\s(?:\d+|\w+)\s(.*)").Groups[1].Value;
            if (string.IsNullOrEmpty(msg))
                msg = "<color=#ffffff>Server</color> ";

            if (args[0].EqualsIgnoreCase("all"))
            {
                foreach (var obj in GameObject.FindGameObjectsWithTag("Player"))
                {
                    obj.GetComponent<HERO>()?.markDie();
                    obj.GetComponent<HERO>()?.photonView.RPC("netDie2", PhotonTargets.All, -1, "[FF0000]" + msg + "[-]  ");
                }
                return;
            }

            if (args[0].EqualsIgnoreCase("titans"))
            {
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("titan"))
                    obj.GetComponent<TITAN>()?.photonView.RPC("netDie", PhotonTargets.All);
                return;
            }

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (obj.GetComponent<HERO>() != null && obj.GetComponent<HERO>().photonView.owner.ID == args[0].ToInt())
                {
                    obj.GetComponent<HERO>().markDie();
                    obj.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, -1, "[FF0000]" + msg + "[-]  ");
                }
            }
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("titan"))
            {
                if (obj.GetComponent<TITAN>() != null && obj.GetComponent<TITAN>().photonView.owner.ID == args[0].ToInt())
                {
                    obj.GetComponent<TITAN>().photonView.RPC("netDie", PhotonTargets.All);
                }
            }
        }
    }
}
