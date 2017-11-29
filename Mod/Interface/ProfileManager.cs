using UnityEngine;

namespace Mod.Interface
{
    public class ProfileManager : Gui
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
                normal = {background = _textBackground},
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
        }

        private Vector2 pos = new Vector2(0, 0);
        protected override void Render()
        {
            GUI.DrawTexture(windowRect = new Rect(Screen.width / 2f - width/2, Screen.height / 2f - height/2, width, height), background);
            if (Alpha < 255f)
                Animation();

            Rect rect;
            GUI.TextField(rect = new Rect(windowRect.x + 20, windowRect.y + 20, 400, 40), "{PlayerName}", _textStyle);
            GUI.TextField(rect = new Rect(rect.x, rect.y + 50, rect.width, rect.height), "{Guild}", _textStyle);
            GUI.TextField(rect = new Rect(rect.x, rect.y + 50, rect.width, rect.height), "{Chatname}", _textStyle);
            GUI.TextField(rect = new Rect(rect.x, rect.y + 50, rect.width, rect.height), "{Friendname}", _textStyle);
            // Use GUI.ScrollView()
            GUILayout.BeginArea(new Rect(rect.x, rect.y + 70, rect.width, windowRect.height - rect.y));
            pos = GUILayout.BeginScrollView(pos, false, false, GUIStyle.none, GUIStyle.none);
            GUILayout.TextField("{HERO}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{Sex}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{Uniform}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{HasCape}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{BodyTexture}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{Hair}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{EyeTextureID}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{BeardTextureID}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{SkinColor}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{HairColor}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{Division}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{CostumeID}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{Skill}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(20f);
            GUILayout.TextField("{Speed}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{Gas}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{Acceleration}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.Space(10f);
            GUILayout.TextField("{BladeDuration}", _textStyle, GUILayout.Width(400), GUILayout.Height(40f));
            GUILayout.EndScrollView();
            GUILayout.EndArea();
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
