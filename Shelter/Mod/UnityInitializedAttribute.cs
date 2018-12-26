using System;

namespace Mod
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class UnityInitializedAttribute : Attribute
    {}
}