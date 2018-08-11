using UnityEngine;

public class ScaleAffector : Affector
{
    protected float DeltaX;
    protected float DeltaY;
    protected AnimationCurve ScaleXCurve;
    protected AnimationCurve ScaleYCurve;
    protected RSTYPE Type;

    public ScaleAffector(float x, float y, EffectNode node) : base(node)
    {
        this.Type = RSTYPE.SIMPLE;
        this.DeltaX = x;
        this.DeltaY = y;
    }

    public ScaleAffector(AnimationCurve curveX, AnimationCurve curveY, EffectNode node) : base(node)
    {
        this.Type = RSTYPE.CURVE;
        this.ScaleXCurve = curveX;
        this.ScaleYCurve = curveY;
    }

    public override void Update()
    {
        float elapsedTime = Node.GetElapsedTime();
        if (this.Type == RSTYPE.CURVE)
        {
            if (this.ScaleXCurve != null)
            {
                Node.Scale.x = this.ScaleXCurve.Evaluate(elapsedTime);
            }
            if (this.ScaleYCurve != null)
            {
                Node.Scale.y = this.ScaleYCurve.Evaluate(elapsedTime);
            }
        }
        else if (this.Type == RSTYPE.SIMPLE)
        {
            float num2 = Node.Scale.x + this.DeltaX * Time.deltaTime;
            float num3 = Node.Scale.y + this.DeltaY * Time.deltaTime;
            if (num2 * Node.Scale.x > 0f)
            {
                Node.Scale.x = num2;
            }
            if (num3 * Node.Scale.y > 0f)
            {
                Node.Scale.y = num3;
            }
        }
    }
}

