using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Body;

public class HUD : BaseMonoBehaviour
{
    [Header("Components")]
    public AbilityMenu abilityMenu;
    public CharacterSelectPanel characterPanel;

    [Header("Main Character")]
    [SerializeField] private PlayerHealthUI healthUI;
    [SerializeField] private SpellSlots spellSlots;
    [SerializeField] private Character mainCharacter;
    [SerializeField] private bool useSpellSlots = false;

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
