using System;
using Mod;
using Mod.Keybinds;
using UnityEngine;

public class LevelTriggerHint : MonoBehaviour
{
    public string content;
    public HintType myhint;
    private bool on;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.on = true;
        }
    }

    private void Start()
    {
        if (!LevelInfoManager.GetInfo(FengGameManagerMKII.Level).Hint)
        {
            enabled = false;
        }
        if (this.content == string.Empty)
        {
            switch (this.myhint)
            {
                case HintType.MOVE:
                {
                    this.content =
                        string.Format(
                            "Hello soldier!\nWelcome to Attack On Titan Tribute Game!\n Press [F7D358]{0}{1}{2}{3}[-] to Move.",
                            Shelter.InputManager[Mod.Keybinds.InputAction.Forward], Shelter.InputManager[Mod.Keybinds.InputAction.Left],
                            Shelter.InputManager[Mod.Keybinds.InputAction.Back], Shelter.InputManager[Mod.Keybinds.InputAction.Right]);
                    break;
                }
                case HintType.TELE:
                    this.content = "Move to [82FA58]green warp point[-] to proceed.";
                    break;

                case HintType.CAMA:
                {
                    this.content = $"Press [F7D358]{Shelter.InputManager[Mod.Keybinds.InputAction.ChangeCamera]}[-] to change camera mode";
                    break;
                }
                case HintType.JUMP:
                    this.content = $"Press [F7D358]{Shelter.InputManager[Mod.Keybinds.InputAction.Jump]}[-] to Jump.";
                    break;

                case HintType.JUMP2:
                    this.content = $"Press [F7D358]{Shelter.InputManager[Mod.Keybinds.InputAction.Forward]}[-] towards a wall to perform a wall-run.";
                    break;

                case HintType.HOOK:
                {
                    this.content = string.Format(
                        "Press and Hold[F7D358] {0}[-] or [F7D358]{1}[-] to launch your grapple.\nNow Try hooking to the [>3<] box.",
                        Shelter.InputManager[Mod.Keybinds.InputAction.LeftHook],
                        Shelter.InputManager[Mod.Keybinds.InputAction.RightHook]);
                    break;
                }
                case HintType.HOOK2:
                {
                    this.content = string.Format(
                        "Press and Hold[F7D358] {0}[-] to launch both of your grapples at the same Time.\n\nNow aim between the two black blocks. \nYou will see the mark \'<\' and \'>\' appearing on the blocks. \nThen press again {0} to hook the blocks.",
                        Shelter.InputManager[Mod.Keybinds.InputAction.BothHooks]);
                    break;
                }
                case HintType.SUPPLY:
                    this.content = $"Press [F7D358]{Shelter.InputManager[Mod.Keybinds.InputAction.Reload]}[-] to reload your blades.\n Move to the supply station to refill your gas and blades.";
                    break;

                case HintType.DODGE:
                    this.content = $"Press [F7D358]{Shelter.InputManager[Mod.Keybinds.InputAction.Dodge]}[-] to Dodge.";
                    break;

                case HintType.ATTACK:
                {
                    this.content = string.Format(
                        "Press [F7D358]{0}[-] to Attack. \nPress [F7D358]{1}[-] to use special attack.\nYou can only kill a titan by slashing his [FA5858]neck[-].\n\n",
                        Shelter.InputManager[Mod.Keybinds.InputAction.Attack], Shelter.InputManager[Mod.Keybinds.InputAction.Special]);
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (this.on)
        {
            this.on = false;
        }
    }
}

