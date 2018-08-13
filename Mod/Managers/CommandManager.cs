using System;
using Mod.Commands;
using System.Collections.Generic;
using Mod.Exceptions;
using Mod.Interface;

namespace Mod.Managers
{
    public class CommandManager : List<Command>, IDisposable
    {
        public CommandManager()
        {
            AddRange(new Command[]
            {
                new CommandBan(), 
                new CommandClear(), 
                new CommandCloth(), 
                new CommandIgnore(), 
                new CommandKick(), 
                new CommandKill(), 
                new CommandMasterClient(), 
                new CommandPause(), 
                new CommandPrivateMessage(), 
                new CommandProp(), 
                new CommandReply(), 
                new CommandResetkd(), 
                new CommandRestart(), 
                new CommandRevive(), 
                new CommandRoom(), 
                new CommandRules(), 
                new CommandSpectate(), 
                new CommandSpectateMode(), 
                new CommandTeleport(), 
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
