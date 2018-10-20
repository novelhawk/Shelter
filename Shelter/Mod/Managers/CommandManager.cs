using System;
using System.Collections.Generic;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Managers
{
    public sealed class CommandManager : List<Command>, IDisposable
    {
        public CommandManager()
        {
            // Slower method but it's easier to add commands
            foreach (var type in Shelter.Assembly.GetTypes())
            {
                if (type.Namespace != $"{nameof(Mod)}.{nameof(Commands)}")
                    continue;

                if (type.IsSubclassOf(typeof(Command)))
                    Add(type.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0]) as Command);
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
