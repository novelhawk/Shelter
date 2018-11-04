namespace Photon.Enums
{
    /// <summary>Defines Photon event-codes as used by PUN.</summary>
    public static class PunEvent
    {
        /// <summary>(200)</summary>
        public const byte RPC = 200;
        
        /// <summary>(201)</summary>
        public const byte SendSerialize = 201;
        
        /// <summary>(202)</summary>
        public const byte Instantiation = 202;
        
        /// <summary>(203)</summary>
        public const byte CloseConnection = 203;
        
        /// <summary>(204)</summary>
        public const byte Destroy = 204;
        
        /// <summary>(205)</summary>
        public const byte RemoveCachedRPCs = 205;
        
        /// <summary>(206)</summary>
        public const byte SendSerializeReliable = 206;  // TS: added this but it's not really needed anymore
        
        /// <summary>(207)</summary>
        public const byte DestroyPlayer = 207;  // TS: added to make others remove all GOs of a player
        
        /// <summary>(208)</summary>
        public const byte SetMasterClient = 208;  // TS: added to assign someone master client (overriding the current)
        
        /// <summary>(209)</summary>
        public const byte OwnershipRequest = 209;
        
        /// <summary>(210)</summary>
        public const byte OwnershipTransfer = 210;
        
        /// <summary>(211)</summary>
        public const byte VacantViewIds = 211;
    }
}