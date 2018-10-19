using UnityEngine;

namespace Xft
{
    public class LinearForceAffector : Affector
    {
        protected Vector3 Force;

        public LinearForceAffector(Vector3 force, EffectNode node) : base(node)
        {
            this.Force = force;
        }

        public override void Update()
        {
            Node.Velocity += this.Force * Time.deltaTime;
        }
    }
}

