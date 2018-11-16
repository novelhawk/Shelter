using System;
using Mod.Interface;

namespace Mod.Exceptions
{
    [Serializable]
    public class CustomException : Exception
    {
        protected CustomException()
        {}

        protected CustomException(string message)
        {
            Notify.New(message, 3000);
        }

        protected CustomException(string message, float height)
        {
            Notify.New("<color=#ff0000>Error</color>", message, 3500, height);
        }
    }
}