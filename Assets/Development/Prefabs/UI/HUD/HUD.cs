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
    [SerializeField] private Canvas abilityMenu;
    [SerializeField] private bool abilityMenuActive = false;
    [SerializeField] private Animator abilityMenuAnimator;
    [SerializeField] private Material shimmerMat;
    [SerializeField] private float shimmerSpeed = .01f;
    private bool isShimmering = false;
    private int currentCharacter;

    [SerializeField] private Material defaultSpriteMat;
    private GameObject mainCamera;
    private Canvas hudCanvas;


    enum CHAR { GOBKIN, ROTTA, OSSEUS }
    
    public Image selectedAbility;

    private void Start() 
    {
        abilityMenuAnimator = abilityMenu.GetComponent<Animator>();
        mainCamera = GameObject.Find("Game Controller").transform.Find("Main Camera").gameObject;
        hudCanvas = this.GetComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        hudCanvas.worldCamera = mainCamera.GetComponent<Camera>();                
    }

    private void FixedUpdate() 
    {
        if(isShimmering)
        {
            Shimmer(currentCharacter);
        }    
    }

    public void CharacterSelect(int idx)
    {
        
        if (selectedCharacter == characterImages[idx])
            return;
   
        prevSelectedCharacter = selectedCharacter;        
        selectedCharacter = characterImages[idx];
        print(selectedCharacter);
        selectedCharacter.GetComponent<SpriteRenderer>().sortingOrder = 3;
        prevSelectedCharacter.GetComponent<SpriteRenderer>().sortingOrder = 1;
        prevSelectedCharacter.GetComponent<SpriteRenderer>().material = defaultSpriteMat;
        shimmerMat.SetFloat("_SheenPosition", 0f);
        selectedCharacter.GetComponent<SpriteRenderer>().material = shimmerMat;
        isShimmering = true;        
        characterSelectAnimator.SetTrigger("SelectCharacter" + idx);
        currentCharacter = idx;

        AbilitySelect(false);
    }

    public void AbilityToggle()
    {
        AbilitySelect(!abilityMenuActive);
    }

    public void Shimmer(int idx)
    {
        float endingPos = -.7f;
        float currentPos = shimmerMat.GetFloat("_SheenPosition");
        if(currentPos >= endingPos)                
            shimmerMat.SetFloat("_SheenPosition", currentPos - shimmerSpeed);
        else
            isShimmering = false;
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
