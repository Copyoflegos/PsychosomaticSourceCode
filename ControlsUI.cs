using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class ControlsUI : MonoBehaviour
{
    public Sprite KEYBOARD;
    public Sprite GAMEPAD;

    public PlayerInput PI;
    SpriteRenderer SR;

    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (PI.user.controlScheme.ToString() == "Gamepad(<Gamepad>)")
        {
            SR.sprite = GAMEPAD;
        }
        if(PI.user.controlScheme.ToString() == "Keyboard&Mouse(<Keyboard>,<Mouse>)")
        {
            SR.sprite = KEYBOARD;
        }
        
    }
}
