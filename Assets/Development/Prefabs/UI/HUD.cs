using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HUD : MonoBehaviour
{
    [SerializeField] private List<Sprite> characterImages;

    [SerializeField] private Sprite selectedCharacter;
    [SerializeField] private Image characterSelector;
    
    public Image selectedAbility;
  
    public void CharacterSelect(InputAction.CallbackContext context)
    {
        
        Debug.Log("Target is: " + context.ReadValue<Vector2>());
        
        if(context.ReadValue<Vector2>().x < 0)
        {
            selectedCharacter = characterImages[2]; // select Osseus
        }else if(context.ReadValue<Vector2>().x > 0)
        {
            selectedCharacter = characterImages[1]; // select Rotta
        }else if(context.ReadValue<Vector2>().y < 0)
        {
            selectedCharacter = characterImages[0]; // select Gobkin
        }else if(context.ReadValue<Vector2>().y > 0)
        {
            selectedCharacter = characterImages[3]; // select dungeonAbilities
        }

        characterSelector.sprite = selectedCharacter;
        
    }


}
