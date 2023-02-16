using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public List<Character> playableCharacters;
    public Character curCharacter;
    public UserInterface userInterface;

    private void Start()
    {
        SetCharacter(0);
    }

    public void SetCharacter(int idx)
    {
        curCharacter = playableCharacters[idx];
        userInterface.SetCharacter(curCharacter);
    }

    public void ReceiveInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 inputVector = context.ReadValue<Vector2>();
            Movement movement = curCharacter.movement;
            movement.SetInputVector(inputVector);
        }
    }
}
