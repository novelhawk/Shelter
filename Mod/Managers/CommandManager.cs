using System;
using Mod.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Mod.Exceptions;
using Mod.Interface;
using UnityEngine;

namespace Mod.Managers
{
    public sealed class CommandManager : List<Command>, IDisposable
    {
        public CommandManager()
        {
            // Slower method but it's easier to add commands
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.FullName.Contains("Assembly-CSharp, ")) //Assembly-CSharp, Version=3.6.2.0, Culture=neutral, PublicKeyToken=null
                    continue;
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Namespace != "Mod.Commands")
                        continue;

                    if (type.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0]) is Command cmd)
                        Add(cmd);
                }
            }
        }

        public Command GetCommand(string commandName)
        {
            for (var i = 0; i < this.Count; i++)
                if (this[i].CommandName.EqualsIgnoreCase(commandName) || this[i].Aliases.AnyEqualsIgnoreCase(commandName))
                    return this[i];
            return null;
        }

        public Exception Execute(string commandName, string[] commandArgs)
            => Execute(GetCommand(commandName), commandArgs);

        public static Exception Execute(Command command, string[] commandArgs)
        {
            try
            {
                command.Execute(commandArgs);
                return null;
            }
            catch (ArgumentException)
            {
                Chat.System("Errore degli argomenti del comando " + command.CommandName);
                return null;
            }
            catch (CustomException)
            {
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
