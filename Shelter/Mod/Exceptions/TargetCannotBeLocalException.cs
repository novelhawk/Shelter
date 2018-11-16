using System;

namespace Mod.Exceptions
{
    [Serializable]
    public class TargetCannotBeLocalException : CustomException
    {
        public TargetCannotBeLocalException(string message) : base(message, 60F)
        {
            
        }
    }
}