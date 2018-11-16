using System;

namespace Mod.Exceptions
{
    [Serializable]
    public class PlayerNotFoundException : CustomException
    {
        public PlayerNotFoundException() : base("Player not found", 60F)
        {
        }

        public PlayerNotFoundException(object id) : base($"Player with ID({id}) not found", 60F)
        {
        }
    }
}