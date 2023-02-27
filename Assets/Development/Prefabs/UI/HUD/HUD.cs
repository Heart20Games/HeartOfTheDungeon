using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HUD : MonoBehaviour
{
    [SerializeField] private List<GameObject> characterImages;
    [SerializeField] private GameObject selectedCharacter;
    [SerializeField] private GameObject prevSelectedCharacter;
    [SerializeField] private Animator characterSelectAnimator;
    [SerializeField] private GameObject characterSelectFrame;
    [SerializeField] private Canvas abilityMenu;
    [SerializeField] private bool abilityMenuActive = false;
    [SerializeField] private Animator abilityMenuAnimator;
    [SerializeField] private Material shimmerMat;
    [SerializeField] private float shimmerTime;

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
        selectedCharacter.transform.parent.SetAsLastSibling();
        prevSelectedCharacter.transform.parent.SetSiblingIndex(5);
        abilityMenu.transform.SetAsFirstSibling();
        characterSelectAnimator.SetTrigger("SelectCharacter" + idx);
        StartCoroutine(Shimmer(idx));

        AbilitySelect(false);
    }

    public void AbilityToggle()
    {
        AbilitySelect(!abilityMenuActive);
    }

    IEnumerator Shimmer(int idx)
    {
        GameObject character = characterImages[idx];
        character.GetComponent<SpriteRenderer>().material = shimmerMat;
        yield return new WaitForSeconds(shimmerTime);
        character.GetComponent<SpriteRenderer>().material = null;
        Debug.Log("Finished Coroutine for" + idx);

    }
    
    public void AbilitySelect(bool activate)
    {
        if (!abilityMenuActive && activate || abilityMenuActive && !activate)
        {
            abilityMenuAnimator.SetBool("AbilityMenuActive", activate);
            abilityMenuActive = activate;
            //if (activate)
            //{
            //    abilityMenu.transform.SetAsLastSibling();
            //}
            //else
            //{
            //    abilityMenu.transform.SetAsFirstSibling();
            //}
        }
    }
}
