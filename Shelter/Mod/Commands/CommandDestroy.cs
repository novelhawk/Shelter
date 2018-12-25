using Mod.Exceptions;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandDestroy : Command
    {
        public override string CommandName => "destroy";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/destroy [GameObject]");

            if (!Shelter.TryFind(args[0], out GameObject obj))
            {
                Shelter.Chat.System("Couldn't find GameObject '{0}'.", args[0]);
                return;
            }

            Object.Destroy(obj);
            Shelter.Chat.System("{0} destroyed.", obj.name);
        }
    }
}
