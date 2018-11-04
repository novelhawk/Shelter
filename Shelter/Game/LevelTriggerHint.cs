using Game;
using Mod;
using Mod.Interface;
using Mod.Keybinds;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class LevelTriggerHint : MonoBehaviour
{
    private static int? _messageId;
    
    public string content;
    public HintType myhint;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_messageId == null)
                _messageId = Chat.System("");            
            Chat.EditMessage(_messageId, content, true);
        }
    }

    private void Start()
    {
        if (!LevelInfoManager.Get(FengGameManagerMKII.Level).Hint)
            enabled = false;

        if (this.content != string.Empty) 
            return;
        
        switch (this.myhint)
        {
            case HintType.Move:
            {
                this.content = string.Format("Welcome to Shelter Mod tutorial.\nUse keys {0}{1}{2}{3} to move",
                        Shelter.InputManager[InputAction.Forward], Shelter.InputManager[InputAction.Back],
                        Shelter.InputManager[InputAction.Left], Shelter.InputManager[InputAction.Right]);
                        
                break;
            }
            case HintType.GreenWarp:
                this.content = "Move to <color=#82FA58>green warp point</color> to proceed.";
                break;

            case HintType.ChangeCamera:
            {
                this.content = string.Format("Press <color=#F7D358>{0}</color> to change camera mode",
                        Shelter.InputManager[InputAction.ChangeCamera]);
                break;
            }
            case HintType.Jump:
                this.content = string.Format("Press <color=#F7D358>{0}</color> to Jump.",
                    Shelter.InputManager[InputAction.Jump]);
                break;

            case HintType.WallJump:
                this.content = string.Format("Press <color=#F7D358>{0}</color> towards a wall to perform a wall-run.",
                    Shelter.InputManager[InputAction.Forward]);
                break;

            case HintType.Hook:
            {
                this.content = string.Format(
                    "Press and Hold <color=#F7D358>{0}</color> or <color=#F7D358>{1}</color> to launch your hook.\nNow try hooking to the [>3<] box.",
                    Shelter.InputManager[InputAction.LeftHook],
                    Shelter.InputManager[InputAction.RightHook]);
                break;
            }
            case HintType.BothHooks:
            {
                this.content = string.Format(
                    "Press and Hold <color=#F7D358>{0}</color> to launch both of your hooks at the same time.\n\nNow aim between the two black blocks. \nYou will see the mark \'<\' and \'>\' appearing on the blocks. \nThen press again {0} to hook the blocks.",
                    Shelter.InputManager[InputAction.BothHooks]);
                break;
            }
            case HintType.Supply:
                this.content = string.Format("Press <color=#F7D358>{0}</color> to reload your blades.\n" +
                                             "Move to the supply station to refill your gas and blades.",
                    Shelter.InputManager[InputAction.Reload]);
                break;

            case HintType.Dodge:
                this.content = string.Format("Press <color=#F7D358>{0}</color> to Dodge.",
                    Shelter.InputManager[InputAction.Dodge]);
                break;

            case HintType.Attack:
            {
                this.content = string.Format(
                    "Press <color=#F7D358>{0}</color> to use your normal attack or use <color=#F7D358>{1}</color> to use your special.\n" +
                    "You can only kill a titan by slashing his <color=#FA5858>neck</color>.",
                    Shelter.InputManager[InputAction.Attack], Shelter.InputManager[InputAction.Special]);
                break;
            }
        }
    }
}

