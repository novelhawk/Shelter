using System;
using Mod.Commands;
using System.Collections.Generic;
using System.Linq;
using Mod.Interface;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mod.Managers
{
    public class CommandManager : List<Command>, IDisposable
    {
        public CommandManager()
        {
            AddRange(new Command[]
            {
                new CommandRevive(), 
            });
        }

        public Command GetCommand(string commandName)
        {
            for (var i = 0; i < this.Count; i++)
                if (this[i].CommandName.EqualsIgnoreCase(commandName) || this[i].Aliases.ContainsIgnoreCase(commandName))
                    return this[i];
            return null;
        }

        public Exception Execute(string commandName, string[] commandArgs)
            => Execute(GetCommand(commandName), commandArgs);

        public Exception Execute(Command command, string[] commandArgs)
        {
            try
            {
                command.Execute(commandArgs);
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public void Dispose()
        {
//            Object.Destroy(commandGameObject);
            this.Clear();
        }
    }
}
