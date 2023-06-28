using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Body;

public class HUD : BaseMonoBehaviour
{
    [Header("Ability Menu")]
    [SerializeField] private Canvas abilityMenu;
    [SerializeField] private Animator abilityMenuAnimator;
    [SerializeField] private bool abilityMenuActive = false;
    public Image selectedAbility;

    [Header("Materials")]
    [SerializeField] private Material defaultSpriteMat;
    [SerializeField] private Material shimmerMat;
    [SerializeField] private float shimmerSpeed = .02f;
    private bool isShimmering = false;

    [Header("Main Character")]
    [SerializeField] private PlayerHealthUI healthUI;
    [SerializeField] private Character mainCharacter;

    [Header("Portrait Images")]
    [SerializeField] private List<GameObject> characterImages;
    [SerializeField] private Animator portraitAnimator;
    private int portraitIndex;
    [SerializeField] private GameObject currentPortrait;
    [SerializeField] private GameObject prevPortrait;

    [Header("Controlled Character")]
    [SerializeField] private Character controlledCharacter;

    [Header("Selected Character")]
    [SerializeField] private Character selectedCharacter;

    private GameObject mainCamera;
    private Canvas hudCanvas;

    enum CHAR { GOBKIN, ROTTA, OSSEUS }



    // Builtin

    private void Awake()
    {
        abilityMenuAnimator = abilityMenu.GetComponent<Animator>();
        hudCanvas = GetComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        
        mainCamera = Camera.main.gameObject;
        hudCanvas.worldCamera = mainCamera.GetComponent<Camera>();
        
        CharacterSelectByIdx(0);
    }

    private void FixedUpdate() 
    {
        if(isShimmering)
        {
            Shimmer(portraitIndex);
        }    
    }


    // Selection

    public void CharacterSelect(Character character)
    {
        if (character == null) return;

        controlledCharacter = character;

        int idx = character.characterUIElements != null ? character.characterUIElements.portraitIndex : 0;
        CharacterSelectByIdx(idx % characterImages.Count);
    }

    public void CharacterSelectByIdx(int idx)
    {
        
        currentPortrait = characterImages[idx];
        currentPortrait.GetComponent<SpriteRenderer>().sortingOrder = 3;
        currentPortrait.GetComponent<SpriteRenderer>().material = shimmerMat;
        portraitAnimator.SetTrigger($"SelectCharacter{idx}");
        portraitIndex = idx;
        
        if (prevPortrait != null)
        {
            prevPortrait.GetComponent<SpriteRenderer>().sortingOrder = 1;
            prevPortrait.GetComponent<SpriteRenderer>().material = defaultSpriteMat;
        }
        prevPortrait = currentPortrait;
        
        shimmerMat.SetFloat("_SheenPosition", 0f);
        isShimmering = true;
        
        AbilitySelect(false);
    }

    public void MainCharacterSelect(Character character)
    {
        if (character == null) return;

        mainCharacter = character;

        if (healthUI != null)
            healthUI.ConnectCharacter(character);
    }


    // Shimmer

    public void Shimmer(int idx)
    {
        float endingPos = -.9f;
        float currentPos = shimmerMat.GetFloat("_SheenPosition");
        if(currentPos >= endingPos)                
            shimmerMat.SetFloat("_SheenPosition", currentPos - shimmerSpeed);
        else
            isShimmering = false;
    }
    

    // Abilities

    public void AbilityToggle()
    {
        AbilitySelect(!abilityMenuActive);
    }

    
    public void AbilitySelect(bool activate)
    {
        if (abilityMenuActive != activate)
        {
            if (abilityMenuAnimator.HasParameter("AbilityMenuActive"))
            {
                abilityMenuAnimator.SetBool("AbilityMenuActive", activate);
                abilityMenuActive = activate;            
            }
            else
            {
                Debug.LogWarning("HUD cannot find Animator parameter: AbilityMenuActive");
            }
        }
    }
}
