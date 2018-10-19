using UnityEngine;

namespace Xft
{
    public class JetAffector : Affector
    {
        protected float MaxAcceleration;
        protected float MinAcceleration;

        public JetAffector(float min, float max, EffectNode node) : base(node)
        {
            this.MinAcceleration = min;
            this.MaxAcceleration = max;
        }

        public override void Update()
        {
            if (Mathf.Abs(Node.Acceleration) < 1E-06)
            {
                float num = Random.Range(this.MinAcceleration, this.MaxAcceleration);
                Node.Acceleration = num;
            }
        }
    }
}

