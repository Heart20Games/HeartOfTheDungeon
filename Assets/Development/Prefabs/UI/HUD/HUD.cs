using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Body;

public class HUD : BaseMonoBehaviour
{
    [Header("Components")]
    public AbilityMenu abilityMenu;
    public CharacterSelectPanel characterPanel;
    public TargetStatusDisplay targetCharacterPanel;
    private GameObject mainCamera;
    private Canvas hudCanvas;

    [Space]
    [Header("Main Character")]
    [SerializeField] private PlayerStatusDisplay mainStatusPanel;
    [SerializeField] private AllyStatusPanel allyStatusPanel;
    [SerializeField] private SpellSlots spellSlots;
    [SerializeField] private bool useSpellSlots = false;
    [ReadOnly][SerializeField] private Character mainCharacter;

    [Space]
    [Header("Identifiables")]
    [ReadOnly][SerializeField] private Character controlledCharacter;
    [ReadOnly][SerializeField] private Character selectedCharacter;
    [ReadOnly][SerializeField] private IIdentifiable target;
    [ReadOnly][SerializeField] private bool hasTarget = false;

    enum CHAR { GOBKIN, ROTTA, OSSEUS }



    // Builtin

    private void Awake()
    {
        if (targetCharacterPanel != null)
            SetTarget(null);

        hudCanvas = GetComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        mainCamera = Camera.main.gameObject;
        hudCanvas.worldCamera = mainCamera.GetComponent<Camera>();

        if (spellSlots != null)
            spellSlots.gameObject.SetActive(useSpellSlots);
    }


    //Selection

    public void CharacterSelect(Character character)
    {
        if (character != null)
        {
            controlledCharacter = character;
            int idx = character.statBlock != null ? character.statBlock.portraitIndex : 0;
            characterPanel.Select(idx);
            abilityMenu.Select(false);
        }
    }

    public void SetTarget(IIdentifiable target)
    {
        this.target = target;
        hasTarget = target != null;
        targetCharacterPanel.Target = target;
    }

    public void AddAlly(IIdentifiable ally)
    {
        allyStatusPanel.AddTarget(ally);
    }

    public void MainCharacterSelect(Character character)
    {
        if (character != null)
        {
            mainCharacter = character;
            if (mainStatusPanel != null)
                mainStatusPanel.ConnectCharacter(character);
        }
    }
}
