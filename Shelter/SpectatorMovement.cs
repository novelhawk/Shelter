using Mod;
using Mod.Keybinds;
using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
    public bool disable;

    private void Update()
    {
        if (this.disable) 
            return;

        float speed = 100;
        if (Shelter.InputManager.IsKeyPressed(InputAction.Jump))
            speed *= 4f;
        if (Shelter.InputManager.IsKeyPressed(InputAction.SlowMovement))
            speed *= 0.3f;
            
        float x = 0;
        if (Shelter.InputManager.IsKeyPressed(InputAction.Forward))
            x = 1;
        else if (Shelter.InputManager.IsKeyPressed(InputAction.Back))
            x = -1;
            
        float y = 0;
        if (Shelter.InputManager.IsKeyPressed(InputAction.Left))
            y = -1f;
        else if (Shelter.InputManager.IsKeyPressed(InputAction.Right))
            y = 1f;
            
        if      (x > 0f) transform.position += transform.forward * speed * Time.deltaTime;
        else if (x < 0f) transform.position -= transform.forward * speed * Time.deltaTime;
        
        if      (y > 0f) transform.position += transform.right * speed * Time.deltaTime;
        else if (y < 0f) transform.position -= transform.right * speed * Time.deltaTime;
        
        if (Shelter.InputManager.IsKeyPressed(InputAction.LeftHook))
            transform.position -= transform.up * speed * Time.deltaTime;
        else if (Shelter.InputManager.IsKeyPressed(InputAction.RightHook))
            transform.position += transform.up * speed * Time.deltaTime;
    }
}

