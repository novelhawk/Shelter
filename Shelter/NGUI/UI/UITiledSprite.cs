using UnityEngine;

[ExecuteInEditMode]
// ReSharper disable once CheckNamespace
public class UITiledSprite : UISlicedSprite
{
    public override Type type
    {
        get
        {
            return Type.Tiled;
        }
    }
}

