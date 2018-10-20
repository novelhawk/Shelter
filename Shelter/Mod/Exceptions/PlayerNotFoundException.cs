using System;

namespace Mod.Exceptions
{
    [Serializable]
    public class PlayerNotFoundException : CustomException
    {
        public PlayerNotFoundException() : base("Player not found", 60F)
        {
        }

        public PlayerNotFoundException(int id) : base($"Player with ID({id}) not found", 60F)
        {
        }

        public PlayerNotFoundException(string id) : base($"Player with ID({id}) not found", 60F)
        {
        }
    }
}