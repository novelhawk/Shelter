using Mod;
using Mod.Keybinds;
using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
    public bool disable;
    public FengCustomInputs inputManager;
    private float speed = 100f;

    private void Start()
    {
        this.inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
    }

    private void Update()
    {
        if (!this.disable)
        {
            float num2;
            float num3;
            float speed = this.speed;
            if (Shelter.InputManager.IsKeyPressed(InputAction.Jump))
            {
                speed *= 3f;
            }
            if (Shelter.InputManager.IsKeyPressed(InputAction.Forward))
            {
                num2 = 1f;
            }
            else if (Shelter.InputManager.IsKeyPressed(InputAction.Back))
            {
                num2 = -1f;
            }
            else
            {
                num2 = 0f;
            }
            if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
            {
                num3 = -1f;
            }
            else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
            {
                num3 = 1f;
            }
            else
            {
                num3 = 0f;
            }
            Transform transform = this.transform;
            if (num2 > 0f)
            {
                transform.position += this.transform.forward * speed * Time.deltaTime;
            }
            else if (num2 < 0f)
            {
                transform.position -= this.transform.forward * speed * Time.deltaTime;
            }
            if (num3 > 0f)
            {
                transform.position += this.transform.right * speed * Time.deltaTime;
            }
            else if (num3 < 0f)
            {
                transform.position -= this.transform.right * speed * Time.deltaTime;
            }
            if (Shelter.InputManager.IsKeyPressed(InputAction.LeftHook))
            {
                transform.position -= this.transform.up * speed * Time.deltaTime;
            }
            else if (Shelter.InputManager.IsKeyPressed(InputAction.RightHook))
            {
                transform.position += this.transform.up * speed * Time.deltaTime;
            }
        }
    }
}

