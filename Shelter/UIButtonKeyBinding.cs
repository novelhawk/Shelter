using UnityEngine;

[AddComponentMenu("Game/UI/Button Key Binding")]
public class UIButtonKeyBinding : MonoBehaviour
{
    public KeyCode keyCode;

    private void Update()
    {
        if (!UICamera.inputHasFocus && this.keyCode != KeyCode.None)
        {
            if (Input.GetKeyDown(this.keyCode))
            {
                SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
            }
            if (Input.GetKeyUp(this.keyCode))
            {
                SendMessage("OnPress", false, SendMessageOptions.DontRequireReceiver);
                SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}

