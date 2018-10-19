using UnityEngine;

[ExecuteInEditMode]
// ReSharper disable once CheckNamespace
public class UIFilledSprite : UISprite
{
    public override Type type
    {
        get
        {
            return Type.Filled;
        }
    }
}

