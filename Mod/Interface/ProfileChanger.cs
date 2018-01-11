using UnityEngine;

namespace Mod.Interface
{
    public class ProfileChanger : Gui
    {
        private Texture2D _textBackground;
        private GUIStyle _textStyle;
        private Texture2D background;

        protected override void OnShow()
        {
            background = Texture(255, 255, 255, 63);
            _textBackground = Texture(196, 196, 196, 150);
            _textStyle = new GUIStyle
            {
                normal = { background = _textBackground },
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
        }

        protected override void Render()
        {
            GUI.DrawTexture(windowRect = new Rect(Screen.width / 2f - width / 2, Screen.height / 2f - height / 2, width, height), background);
            if (Alpha < 255f)
                Animation();

        }

        private float width;
        private float height;
        private float Alpha;
        private void Animation()
        {
            if (width < 1280f || height < 720f)
            {
                if (width < 1280f)
                    width += 1280f / 100f * 0.1f + width / 100 * .5f * Time.deltaTime * 500;
                if (height < 720f)
                    height += 720f / 100f * 0.1f + height / 100 * .5f * Time.deltaTime * 500;
                if (width < 1280f || height < 720f)
                    return;
                width = 1280f;
                height = 720f;
            }
            else if (PhotonNetwork.connectionStatesDetailed == PeerStates.JoinedLobby)
            {
                Alpha += Time.deltaTime * 100;
                if (Alpha < 255) return;
                Alpha = Mathf.Clamp(Alpha, 0, 255);
            }
        }

        protected override void OnHide()
        {
            width = 0f;
            height = 0f;
            Alpha = 0f;
            Destroy(background);
        }
    }
}
