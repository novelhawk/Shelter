using System.Runtime.InteropServices;
using UnityEngine;

namespace Xft
{
    [StructLayout(LayoutKind.Sequential)]
    public struct STransform
    {
        public Vector3 position;
        public Quaternion rotation;

        public void LookAt(Vector3 target, Vector3 up)
        {
            Vector3 forward = target - this.position;
            this.rotation = Quaternion.LookRotation(forward, up);
        }
    
        public void Reset()
        {
            this.position = Vector3.zero;
            this.rotation = Quaternion.identity;
        }
    }
}

