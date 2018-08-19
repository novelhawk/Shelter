using System;
using System.IO;
using System.Text;
using ExitGames.Client.Photon;
using Mod.Exceptions;
using Mod.Interface;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mod.Commands
{
    public class CommandTest : Command
    {
        public override string CommandName => "test";

        
        public override void Execute(string[] args)
        {
            if (Shelter.TryFind(args[0], out GameObject obj))
            {
                var local = args[1] == "true";
                if (args.Length < 3)
                {
                    Chat.System("Position of " + obj.name);
                    Chat.System(obj.transform.localPosition);
                    Chat.System(obj.transform.position);
                    return;
                }
                
                if (local)
                    obj.transform.localPosition = new Vector3(float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]));
                else
                    obj.transform.position = new Vector3(float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]));
            }
            
            
            
//                Object.Destroy(obj);
            
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
