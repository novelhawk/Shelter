using UnityEngine;

public class AttractionForceAffector : Affector
{
    private AnimationCurve AttractionCurve;
    private float Magnitude;
    protected Vector3 Position;
    private bool UseCurve;

    public AttractionForceAffector(float magnitude, Vector3 pos, EffectNode node) : base(node)
    {
        this.Magnitude = magnitude;
        this.Position = pos;
        this.UseCurve = false;
    }

    public AttractionForceAffector(AnimationCurve curve, Vector3 pos, EffectNode node) : base(node)
    {
        this.AttractionCurve = curve;
        this.Position = pos;
        this.UseCurve = true;
    }

    public override void Update()
    {
        Vector3 vector;
        float magnitude;
        if (Node.SyncClient)
        {
            vector = this.Position - Node.GetLocalPosition();
        }
        else
        {
            vector = Node.ClientTrans.position + this.Position - Node.GetLocalPosition();
        }
        float elapsedTime = Node.GetElapsedTime();
        if (this.UseCurve)
        {
            magnitude = this.AttractionCurve.Evaluate(elapsedTime);
        }
        else
        {
            magnitude = this.Magnitude;
        }
        float num3 = magnitude;
        Node.Velocity += vector.normalized * num3 * Time.deltaTime;
    }
}

