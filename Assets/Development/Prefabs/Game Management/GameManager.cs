using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Body;
using UnityEngine.Events;
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
    [HideInInspector] public Character CurCharacter { get => curCharacter; set => SetCharacter(value); }
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
        InputMode = InputMode;
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
            SetMode(InputMode.Selection);

        foreach (var character in playableCharacters)
            character.onDeath.AddListener(OnCharacterDied);
    }


    // Updates

    private void Update()
    {
        if (reactivateMode)
        {
            reactivateMode = false;
            Mode = Mode;
        }
        if (swapModes)
        {
            swapModes = false;
            InputMode = InputMode;
        }
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

    // Game InputMode
    [SerializeField] private GameMode mode = new();
    public GameMode Mode { get => mode; set => SetMode(value); }
    public InputMode InputMode { get => mode.inputMode; set => SetMode(value); }
    public MoveMode MoveMode { get => mode.moveMode; }
    public LookMode LookMode { get => mode.lookMode; }
    public bool swapModes = false;
    public bool reactivateMode = false;
    
    public void SetMode(InputMode inputMode)
    {
        if (debug) print($"Change InputMode to {inputMode} (in bank? {(ModeBank.ContainsKey(inputMode) ? "yes" : "no")})");
        if (ModeBank.TryGetValue(inputMode, out GameMode mode))
            SetMode(mode);
        else
            Debug.LogWarning($"Can't find game mode for \"{inputMode}\"");
    }

    public void SetMode(GameMode mode)
    {
        ActivateMode(mode, this.mode);
        this.mode = mode;
    }

    public void ActivateMode(GameMode mode, GameMode lastMode)
    {
        if (debug) print($"Activate inputMode {mode}.");
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

        if (mode.inputMode == InputMode.Selection)
        {
            if (curController != null && curCharacter != null)
                curController.transform.position = curCharacter.body.position;
        }
    }

    // Controllables

    public void SetControllable(IControllable controllable, bool shouldControl)
    {
        controllable?.SetControllable(shouldControl);
    }

    // Selectables

    [ReadOnly][SerializeField] private ASelectable selectedTarget;
    public void OnTargetSelected(ASelectable selectable)
    {
        
        if (selectedTarget == selectable || selectable == null)
        {
            if (debug) print($"Target deselected: {selectable}");
            hud.SetTarget(null);
        }
        else
        {
            if (debug) print($"Target selected: {selectable}");
            if (selectable.source.TryGetComponent<Character>(out var character))
            {
                hud.SetTarget(character);
            }
            else if (selectable.source.TryGetComponent<Identifiable>(out var identifiable))
            {
                hud.SetTarget(identifiable);
            }
        }
        selectedTarget = selectable;
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
            SetControllable(character, true);
            curCharacter = character;
            SetMode(InputMode.Character);
        }
    }
    
    public void SwitchToCompanion(int idx)
    {
        if (curCharIdx == idx)
            SetCharacterIdx(0);
        else
            SetCharacterIdx(idx);
    }


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
