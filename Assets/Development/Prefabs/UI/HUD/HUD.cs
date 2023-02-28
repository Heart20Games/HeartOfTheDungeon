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
    [SerializeField] private Material defaultSpriteMat;
    private GameObject mainCamera;
    private Canvas hudCanvas;



    enum CHAR { GOBKIN, ROTTA, OSSEUS }
    
    public Image selectedAbility;

    private void Start() 
    {
        abilityMenuAnimator = abilityMenu.GetComponent<Animator>();
        mainCamera = GameObject.FindGameObjectWithTag("Player").transform.Find("Main Camera").gameObject;
        hudCanvas = this.GetComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        hudCanvas.worldCamera = mainCamera.GetComponent<Camera>();
    }

    public void CharacterSelect(int idx)
    {
        if (selectedCharacter == characterImages[idx])
            return;

        prevSelectedCharacter = selectedCharacter;

        selectedCharacter = characterImages[idx];
        selectedCharacter.GetComponent<SpriteRenderer>().sortingOrder = 3;
        prevSelectedCharacter.GetComponent<SpriteRenderer>().sortingOrder = 1;
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
        character.GetComponent<SpriteRenderer>().material = defaultSpriteMat;
    }
    
    public void AbilitySelect(bool activate)
    {
        if (!abilityMenuActive && activate || abilityMenuActive && !activate)
        {
            abilityMenuAnimator.SetBool("AbilityMenuActive", activate);
            abilityMenuActive = activate;            
        }
    }
}
