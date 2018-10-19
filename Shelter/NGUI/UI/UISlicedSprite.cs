using UnityEngine;

[ExecuteInEditMode]
// ReSharper disable once CheckNamespace
public class UISlicedSprite : UISprite
{
    public override Type type
    {
        get
        {
            return Type.Sliced;
        }
    }
}

