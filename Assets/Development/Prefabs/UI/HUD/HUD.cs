using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Body;

public class HUD : BaseMonoBehaviour
{
    [Header("Components")]
    public AbilityMenu abilityMenu;
    public CharacterSelectPanel characterPanel;
    public TargetCharacterPanel targetCharacterPanel;
    private GameObject mainCamera;
    private Canvas hudCanvas;

    [Space]
    [Header("Main Character")]
    [SerializeField] private PlayerHealthUI healthUI;
    [SerializeField] private SpellSlots spellSlots;
    [SerializeField] private bool useSpellSlots = false;
    [ReadOnly][SerializeField] private Character mainCharacter;

    [Space]
    [Header("Identifiables")]
    [ReadOnly][SerializeField] private Character controlledCharacter;
    [ReadOnly][SerializeField] private Character selectedCharacter;
    [ReadOnly][SerializeField] private IIdentifiable target;

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
    }


    //Selection

    public void CharacterSelect(Character character)
    {
        if (character != null)
        {
            controlledCharacter = character;
            int idx = character.characterUIElements != null ? character.characterUIElements.portraitIndex : 0;
            characterPanel.Select(idx);
            abilityMenu.Select(false);
        }
    }

    public void SetTarget(IIdentifiable identifiable)
    {
        target = identifiable;
        targetCharacterPanel.gameObject.SetActive(identifiable != null);
        targetCharacterPanel.Target = identifiable;
    }

    public void MainCharacterSelect(Character character)
    {
        if (character != null)
        {
            mainCharacter = character;
            if (healthUI != null)
                healthUI.ConnectCharacter(character);
        }
    }
}
