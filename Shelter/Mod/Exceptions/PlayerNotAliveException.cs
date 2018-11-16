using System;

namespace Mod.Exceptions
{
    [Serializable]
    public class PlayerNotAliveException : CustomException
    {
        public PlayerNotAliveException() : base("Player is not alive!")
        {
        }

        public PlayerNotAliveException(object id) : base($"Player ID({id}) is not alive")
        {
        }

        public PlayerNotAliveException(Player player) : base($"Player {player} is not alive")
        {
        }
    }
}