using System;
using System.IO;
using System.Text;
using ExitGames.Client.Photon;
using Mod.Exceptions;
using Mod.Interface;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandTest : Command
    {
        public override string CommandName => "test";

        
        public override void Execute(string[] args)
        {
//            Chat.System($"Enabled: {Shelter.AnimationManager.Enabled}");
//            foreach (var a in Shelter.AnimationManager.Animations)
//            {
//                Chat.System(a.Name);
//                Chat.System(a.Type);
//                foreach (var color in a.Colors)
//                    Chat.System($"Color({color.R}, {color.G}, {color.B}, {color.A})");
//            }
        }
    }
}
