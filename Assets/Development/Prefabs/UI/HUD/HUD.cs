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

    enum CHAR { GOBKIN, ROTTA, OSSEUS }
    
    public Image selectedAbility;

    private void Start() 
    {
        abilityMenuAnimator = abilityMenu.GetComponent<Animator>();
    }

    public void CharacterSelect(int idx)
    {
        if (selectedCharacter == characterImages[idx])
            return;

        prevSelectedCharacter = selectedCharacter;

        selectedCharacter = characterImages[idx];
        selectedCharacter.transform.SetAsLastSibling();
        prevSelectedCharacter.transform.SetSiblingIndex(idx);
        abilityMenu.transform.SetAsFirstSibling();
        characterSelectAnimator.SetTrigger("SelectCharacter" + idx);

        AbilitySelect(false);
    }

    public void AbilityToggle()
    {
        AbilitySelect(!abilityMenuActive);
    }

    public void AbilitySelect(bool activate)
    {
        if (!abilityMenuActive && activate || abilityMenuActive && !activate)
        {
            abilityMenuAnimator.SetBool("AbilityMenuActive", activate);
            abilityMenuActive = activate;
            if (activate)
            {
                abilityMenu.transform.SetAsLastSibling();
            }
            else
            {
                abilityMenu.transform.SetAsFirstSibling();
            }
        }
    }
}
