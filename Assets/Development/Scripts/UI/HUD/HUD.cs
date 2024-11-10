using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Body;
using HotD.Castables;
using HotD.Body;
using MyBox;

public class HUD : BaseMonoBehaviour
{
    static public HUD main;

    [Foldout("Components", true)]
    public AbilityMenu abilityMenu;
    public PartySelectPanel partySelectPanel;
    public TargetStatusDisplay targetCharacterPanel;
    public bool useTargetPanel = true;
    public Transform crosshair;
    [Foldout("Components")]
    public CastMeter castMeter;

    [Space]
    [Foldout("Main Character", true)]
    [SerializeField] private PlayerStatusDisplay mainStatusPanel;
    [SerializeField] private AllyStatusPanel allyStatusPanel;
    [SerializeField] private SpellSlots spellSlots;
    [SerializeField] private bool useSpellSlots = false;
    [Foldout("Main Character")]
    [ReadOnly][SerializeField] private Character mainCharacter;

    [Space]
    [Foldout("Identifiables", true)]
    [ReadOnly][SerializeField] private Character controlledCharacter;
    [ReadOnly][SerializeField] private Character selectedCharacter;
    [ReadOnly][SerializeField] private IIdentifiable target;
    [Foldout("Identifiables")]
    [ReadOnly][SerializeField] private bool hasTarget = false;

    [SerializeField] private bool debug;

    enum CHAR { GOBKIN, ROTTA, OSSEUS }



    // Builtin

    private void OnDestroy()
    {
        if (HUD.main == this) HUD.main = null;
    }

    private void Awake()
    {
        HUD.main = this;

        if (crosshair != null)
            crosshair.gameObject.SetActive(false);

        if (targetCharacterPanel != null)
            SetTarget(null);

        if (spellSlots != null)
            spellSlots.gameObject.SetActive(useSpellSlots);
    }

    public void SetParty(Party party)
    {
        partySelectPanel.SetTarget(0, party.Leader);
        int idx = 1;
        foreach (Character member in party.members)
        {
            if (member != party.Leader)
            {
                partySelectPanel.SetTarget(idx, member);
                idx++;
                if (idx > 2) break;
            }
        }
    }

    //Selection

    public void CharacterSelect(Character character)
    {
        if (character != null)
        {
            controlledCharacter = character;
            int idx = character.StatBlock != null ? character.StatBlock.portraitIndex : 0;
            partySelectPanel.Select(idx);
            abilityMenu.Select(false);
            if (character.castables.Length > 1 && character.castables[0] != null)
            {
                Print("Attach Cast Meter", debug, this);
                castMeter.Castable = character.castables[0];
            }
        }
    }

    public void SetTarget(IIdentifiable target)
    {
        if (useTargetPanel)
        {
            this.target = target;
            hasTarget = target != null;
            targetCharacterPanel.Target = target;
        }
        else
        {
            this.target = null;
            hasTarget = target != null;
            targetCharacterPanel.Target = null;
        }
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

            character.PrimaryTargetingMethod(out var method);
            if (crosshair != null)
                crosshair.gameObject.SetActive(method == AimingMethod.OverTheShoulder);
        }
        else
        {
            if (crosshair != null)
                crosshair.gameObject.SetActive(false);
        }
    }
}
