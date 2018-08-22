using Mod;
using Mod.Interface;
using Mod.Keybinds;
using UnityEngine;

public class TITAN_CONTROLLER : MonoBehaviour
{
    public bool bite;
    public bool bitel;
    public bool biter;
    public bool chopl;
    public bool chopr;
    public bool choptl;
    public bool choptr;
    public bool cover;
    public Camera currentCamera;
    public float currentDirection;
    public bool grabbackl;
    public bool grabbackr;
    public bool grabfrontl;
    public bool grabfrontr;
    public bool grabnapel;
    public bool grabnaper;
    public bool isAttackDown;
    public bool isAttackIIDown;
    public bool isHorse;
    public bool isJumpDown;
    public bool isSuicide;
    public bool isWALKDown;
    public bool sit;
    public float targetDirection;

    private void Start()
    {
        this.currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            enabled = false;
    }
    
    private void Update()
    {
        if (this.isHorse)
        {
            int x = 0;
            if (Shelter.InputManager.IsKeyPressed(InputAction.Forward))
                x = 1;
            else if (Shelter.InputManager.IsKeyPressed(InputAction.Back))
                x = -1;

            int y = 0;
            if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
                y = -1;
            else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
                y = 1;
            
            if (y != 0 || x != 0)
            {
                float rotY = this.currentCamera.transform.rotation.eulerAngles.y;
                float num4 = -Mathf.Atan2(x, y) * 57.29578f + 90f;
                targetDirection = rotY + num4;
                currentDirection = targetDirection;
            }
            else
            {
                targetDirection = -874f;
            }
            
            this.isAttackDown = false;
            this.isAttackIIDown = false;
            
            if (Shelter.InputManager.IsKeyPressed(InputAction.HorseJump))
                this.isAttackDown = true;
            this.isWALKDown = Shelter.InputManager.IsKeyPressed(InputAction.Forward);
        }
        else
        {
            int x = 0;
            if (Shelter.InputManager.IsKeyPressed(InputAction.Forward))
                x = 1;
            else if (Shelter.InputManager.IsKeyPressed(InputAction.Back))
                x = -1;

            int y = 0;
            if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
                y = -1;
            else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
                y = 1;
            
            if (y != 0 || x != 0)
            {
                float rotY = this.currentCamera.transform.rotation.eulerAngles.y;
                float num4 = -Mathf.Atan2(x, y) * 57.29578f + 90;
                targetDirection = rotY + num4;
                currentDirection = targetDirection;
            }
            else
            {
                this.targetDirection = -874f;
            }
            
            float num6 = this.currentCamera.transform.rotation.eulerAngles.y - this.currentDirection;
            if (num6 >= 180f)
                num6 -= 360f;
            
            this.isAttackDown = Shelter.InputManager.IsKeyPressed(InputAction.TitanDoublePunch);
            this.isAttackIIDown = Shelter.InputManager.IsDown(InputAction.TitanSlam);
            this.isJumpDown = Shelter.InputManager.IsDown(InputAction.TitanJump);
            this.isSuicide = Shelter.InputManager.IsDown(InputAction.Suicide);
            this.cover = Shelter.InputManager.IsKeyPressed(InputAction.TitanCover);
            this.grabfrontr = Shelter.InputManager.IsDown(InputAction.TitanGrabFront) && num6 >= 0;
            this.grabfrontl = Shelter.InputManager.IsDown(InputAction.TitanGrabFront) && num6 < 0;
            this.grabbackr = Shelter.InputManager.IsDown(InputAction.TitanGrabBack) && num6 >= 0;
            this.grabbackl = Shelter.InputManager.IsDown(InputAction.TitanGrabBack) && num6 < 0; 
            this.grabnaper = Shelter.InputManager.IsDown(InputAction.TitanGrabNape) && num6 >= 0;
            this.grabnapel = Shelter.InputManager.IsDown(InputAction.TitanGrabNape) && num6 < 0;
            this.choptr = Shelter.InputManager.IsDown(InputAction.TitanAntiAE) && num6 >= 0f;
            this.choptl = Shelter.InputManager.IsDown(InputAction.TitanAntiAE) && num6 < 0f;
            this.biter = Shelter.InputManager.IsDown(InputAction.TitanBite) && num6 > 7.5f;
            this.bitel = Shelter.InputManager.IsDown(InputAction.TitanBite) && num6 < -7.5f;
            this.bite = Shelter.InputManager.IsDown(InputAction.TitanBite) && num6 >= -7.5f && num6 <= 7.5f;

            this.isWALKDown = Shelter.InputManager.IsKeyPressed(InputAction.TitanWalk);
        }
    }
}

