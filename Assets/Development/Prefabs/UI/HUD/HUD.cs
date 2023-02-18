using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HUD : MonoBehaviour
{
    [SerializeField] private List<Image> characterImages;
    [SerializeField] private Image selectedCharacter;
    [SerializeField] private Image prevSelectedCharacter;
    [SerializeField] private Animator characterSelectAnimator;
    [SerializeField] private Image characterSelectFrame;
    [SerializeField] private Image abilityMenu;
    [SerializeField] private bool abilityMenuActive = false;
    [SerializeField] private Animator abilityMenuAnimator;
    
    public Image selectedAbility;

    private void Start() 
    {
        abilityMenuAnimator = abilityMenu.GetComponent<Animator>();
    }
  
    public void CharacterSelect(InputAction.CallbackContext context)
    {
        //Debug.Log("Target is: " + context.ReadValue<Vector2>());
        if(context.performed)
        {
            
            prevSelectedCharacter = selectedCharacter;
            if(context.ReadValue<Vector2>().x < 0) // select Osseus
            {
                if(selectedCharacter == characterImages[2])
                    return;

                selectedCharacter = characterImages[2]; 
                selectedCharacter.transform.SetAsLastSibling();
                prevSelectedCharacter.transform.SetSiblingIndex(2);
                abilityMenu.transform.SetAsFirstSibling();            
                characterSelectAnimator.SetTrigger("SelectCharacter2");
                
                if(abilityMenuActive == true)
                {
                    abilityMenuAnimator.SetBool("AbilityMenuActive", false);
                    abilityMenuActive = false;
                    abilityMenu.transform.SetAsFirstSibling();    
                }                
            }
            else if(context.ReadValue<Vector2>().x > 0) // select Rotta
            {
                if(selectedCharacter == characterImages[1])
                    return;

                selectedCharacter = characterImages[1]; 
                selectedCharacter.transform.SetAsLastSibling();
                prevSelectedCharacter.transform.SetSiblingIndex(2);
                abilityMenu.transform.SetAsFirstSibling();
                characterSelectAnimator.SetTrigger("SelectCharacter1");
                
                if(abilityMenuActive == true)
                {
                    abilityMenuAnimator.SetBool("AbilityMenuActive", false);
                    abilityMenuActive = false;
                    abilityMenu.transform.SetAsFirstSibling();    
                }                
            }
            else if(context.ReadValue<Vector2>().y < 0) // select Gobkin
            {
                if(selectedCharacter == characterImages[0])
                    return;
                
                selectedCharacter = characterImages[0]; 
                selectedCharacter.transform.SetAsLastSibling();
                prevSelectedCharacter.transform.SetSiblingIndex(2);
                abilityMenu.transform.SetAsFirstSibling(); 
                characterSelectAnimator.SetTrigger("SelectCharacter0");
                
                if(abilityMenuActive == true)
                {
                    abilityMenuAnimator.SetBool("AbilityMenuActive", false);
                    abilityMenuActive = false;
                    abilityMenu.transform.SetAsFirstSibling();    
                }                        
            }
            else if(context.ReadValue<Vector2>().y > 0) // select dungeonAbilities
            {
                if(abilityMenuActive == true)
                {
                    abilityMenuAnimator.SetBool("AbilityMenuActive", false);
                    abilityMenuActive = false;
                    abilityMenu.transform.SetAsFirstSibling();
                }    
                else
                {
                    abilityMenuAnimator.SetBool("AbilityMenuActive", true); 
                    abilityMenu.transform.SetAsLastSibling();
                    abilityMenuActive = true;                   
                }
            }
        }
        
        
    }


}
