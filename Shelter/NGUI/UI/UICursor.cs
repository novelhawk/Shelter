using NGUI.Internal;
using UnityEngine;

[RequireComponent(typeof(UISprite)), AddComponentMenu("NGUI/Examples/UI Cursor")]
// ReSharper disable once CheckNamespace
public class UICursor : MonoBehaviour
{
    private UIAtlas mAtlas;
    private static UICursor mInstance;
    private UISprite mSprite;
    private string mSpriteName;
    private Transform mTrans;
    public Camera uiCamera;

    private void Awake()
    {
        mInstance = this;
    }

    public static void Clear()
    {
        Set(mInstance.mAtlas, mInstance.mSpriteName);
    }

    private void OnDestroy()
    {
        mInstance = null;
    }

    public static void Set(UIAtlas atlas, string sprite)
    {
        if (mInstance != null)
        {
            mInstance.mSprite.atlas = atlas;
            mInstance.mSprite.spriteName = sprite;
            mInstance.mSprite.MakePixelPerfect();
            mInstance.Update();
        }
    }

    private void Start()
    {
        this.mTrans = transform;
        this.mSprite = GetComponentInChildren<UISprite>();
        this.mAtlas = this.mSprite.atlas;
        this.mSpriteName = this.mSprite.spriteName;
        this.mSprite.depth = 100;
        if (this.uiCamera == null)
        {
            this.uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
        }
    }

    private void Update()
    {
        if (this.mSprite.atlas != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            if (this.uiCamera != null)
            {
                mousePosition.x = Mathf.Clamp01(mousePosition.x / Screen.width);
                mousePosition.y = Mathf.Clamp01(mousePosition.y / Screen.height);
                this.mTrans.position = this.uiCamera.ViewportToWorldPoint(mousePosition);
                if (this.uiCamera.isOrthoGraphic)
                {
                    this.mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(this.mTrans.localPosition, this.mTrans.localScale);
                }
            }
            else
            {
                mousePosition.x -= Screen.width * 0.5f;
                mousePosition.y -= Screen.height * 0.5f;
                this.mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(mousePosition, this.mTrans.localScale);
            }
        }
    }
}

