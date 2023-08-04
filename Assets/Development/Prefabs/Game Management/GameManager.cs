using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Body;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using static GameModes;
using Selection;

public class Game : BaseMonoBehaviour
{
    public static Game game;

    // Properties
    [Header("Parts")]
    public Character playerCharacter;
    public List<Character> playableCharacters;
    public Selector selector;
    public SimpleController selectorController;
    public Targeter targeter;
    public GameSettings settings;
    [HideInInspector] public UserInterface userInterface;
    [HideInInspector] public HUD hud;
    [HideInInspector] public List<ITimeScalable> timeScalables;
    [HideInInspector] public List<Interactable> interactables;
    private PlayerInput input;

    // Current Character
    [Header("Current Character")]
    [ReadOnly][SerializeField] private Character curCharacter;
    [ReadOnly] public SimpleController curController;
    [ReadOnly] public Selector curSelector;
    [ReadOnly] public ILooker curLooker;
    [HideInInspector] public Character CurCharacter { get { return curCharacter; } set { SetCharacter(value); } }
    private int curCharIdx = 0;
    public int CurCharIdx { get => curCharIdx; }

    // TimeScale
    [Header("TimeScale")]
    public float timeScale = 1.0f;
    public float TimeScale { get { return timeScale; } set { SetTimeScale(value); } }

    // Cheats / Shortcuts
    [Header("Cheats and Shortcuts")]
    public bool restartable = true;
    public bool debug = false;

    // Events
    [Header("Events")]
    public UnityEvent onPlayerDied;


    // Initialization

    private void Awake()
    {
        game = this;
        input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        InitializePlayableCharacters();
        Mode = Mode;
    }

    public void InitializePlayableCharacters()
    {
        bool hasPlayer = false;
        if (playerCharacter != null)
        {
            hud.MainCharacterSelect(playerCharacter);
            if (!hasPlayer)
                playableCharacters.Insert(0, playerCharacter);
        }

        SetCharacterIdx(0);
        if (curCharacter == null)
            SetMode(GameMode.Selection);

        foreach (var character in playableCharacters)
            character.onDeath.AddListener(OnCharacterDied);
    }


    // Setters

    // Time Scale
    public void SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
        foreach(ITimeScalable timeScalable in timeScalables)
        {
            if (timeScalable != null)
                timeScalable.SetTimeScale(this.timeScale);
            else
                print("Invalid Object");
        }
    }

    // Game Mode
    private GameMode mode = GameMode.Character;
    public GameMode Mode { get { return mode; } set { SetMode(value); } }
    
    public void SetMode(GameMode mode)
    {
        if (debug) print($"Change Mode to {mode} (in bank? {(ModeBank.ContainsKey(mode) ? "yes" : "no")})");
        ActivateMode(ModeBank[mode], ModeBank[this.mode]);
        if (mode == GameMode.Selection)
        {
            if (curController != null && curCharacter != null)
                curController.transform.position = curCharacter.body.position;
        }
        this.mode = mode;
    }

    public void ActivateMode(ModeParameters mode, ModeParameters lastMode)
    {
        if (debug) print($"Activate mode {mode}.");
        userInterface.SetHudActive(mode.hudActive);
        userInterface.SetDialogueActive(mode.dialogueActive);
        userInterface.SetControlScreenActive(mode.controlScreenActive);
        input.SwitchCurrentActionMap(mode.inputMap);
        TimeScale = mode.timeScale;

        // Swap Controllables
        IControllable newControllable = mode.Controllable;
        IControllable oldControllable = lastMode.Controllable;
        if (newControllable != null)
            SetControllable(newControllable, true);
        if (oldControllable != null && oldControllable != newControllable)
            SetControllable(oldControllable, false);

        // Swap Finders
        TargetFinder newFinder = mode.Finder;
        TargetFinder oldFinder = lastMode.Finder;
        if (mode.Finder != null)
            targeter.Finder = mode.Finder;
        if (oldFinder != null && oldFinder != newFinder)
            targeter.Finder = null;
        targeter.SetTargetLock(mode.targetLock);
    }

    // Controllables

    public void SetControllable(IControllable controllable, bool shouldControl)
    {
        controllable?.SetControllable(shouldControl);
    }

    // Set Characters

    public void SetCharacterIdx(int idx)
    {
        if (playableCharacters.Count > 0)
        {
            idx = idx < 0 ? playableCharacters.Count + idx : idx;
            curCharIdx = idx % (playableCharacters.Count);
            Character character = playableCharacters[curCharIdx];
            SetCharacter(character);
            hud.CharacterSelect(character);
        }
    }

    public void SetCharacter(Character character)
    {
        if (character != null)
        {
            SetControllable(curCharacter, false);
            curCharacter = character;
            SetControllable(curCharacter, true);
            SetMode(GameMode.Character);
        }
    }
    
    public void SwitchToCompanion(int idx)
    {
        if (curCharIdx == idx)
            SetCharacterIdx(0);
        else
            SetCharacterIdx(idx);
    }


    // Checks
    public bool CanUseCharacter() { return curCharacter != null && Mode == GameMode.Character; }
    public bool CanUseSelector() { return curController != null && Mode == GameMode.Selection; }



    // Events

    public void OnCharacterDied(Character character)
    {
        if (character == playerCharacter)
        {
            onPlayerDied.Invoke();
        }
        else if (character == curCharacter)
        {
            SetCharacter(playerCharacter);
        }
    }
}
