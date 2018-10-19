using UnityEngine;

namespace Photon
{
    public static class GameObjectExtensions
    {
        public static bool IsActive(this GameObject target)
        {
            return target.activeInHierarchy;
        }
    }
}

