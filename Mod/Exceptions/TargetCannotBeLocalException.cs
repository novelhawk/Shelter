using System;
using System.Runtime.Serialization;

namespace Mod.Exceptions
{
    [Serializable]
    public class TargetCannotBeLocalException : CustomException
    {
        public TargetCannotBeLocalException()
        {
        }

        public TargetCannotBeLocalException(string message) : base("message", 60F)
        {
            
        }
    }
}