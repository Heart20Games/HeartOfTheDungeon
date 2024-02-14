using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Body;
using UnityEngine.Events;
using static GameModes;
using Selection;
using UnityEngine.SceneManagement;

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
    [HideInInspector] public List<Cutouts> cardboardCutouts = new();
    [HideInInspector] public UserInterface userInterface;
    [HideInInspector] public GlobalVolumeManager volumeManager;
    [HideInInspector] public HUD hud;
    [HideInInspector] public List<ITimeScalable> timeScalables;
    [HideInInspector] public List<Interactable> interactables;
    [HideInInspector] public List<Character> allCharacters;
    private PlayerInput input;

    // Initialization
    [SerializeField] private InputMode initialInputMode = InputMode.None;
    [SerializeField] private Menu initialMenu = Menu.None;

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
    public UnityEvent onRestartScene;
    public UnityEvent onRestartGame;


    // Initialization

    private void Awake()
    {
        game = this;
        input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        InitializePlayableCharacters();
        if (initialMenu != Menu.None)
        {
            SetMode(initialMenu);
        }
        else if (initialInputMode != InputMode.None)
        {
            InputMode = InputMode.None;
        }
        else
        {
            InputMode = InputMode;
        }
    }

    public void InitializePlayableCharacters()
    {
        bool hasPlayer = false;
        if (playerCharacter != null)
        {
            hud.MainCharacterSelect(playerCharacter);
            foreach (var character in playableCharacters)
            {
                if (character != playerCharacter)
                    hud.AddAlly(character);
            }
            if (!hasPlayer)
                playableCharacters.Insert(0, playerCharacter);
        }

        SetCharacterIdx(0);
        if (curCharacter == null)
            SetMode(InputMode.Selection);

        foreach (var character in playableCharacters)
            character.onDeath.AddListener(OnCharacterDied);
    }


    // Restart

    public void RestartScene()
    {
        if (restartable)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartGame()
    {
        if (restartable)
            onRestartGame.Invoke();
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
    public string ModeName { get => mode.name; set => SetMode(value); }
    public InputMode InputMode { get => mode.inputMode; set => SetMode(value); }
    public MoveMode MoveMode { get => mode.moveMode; }
    public LookMode LookMode { get => mode.lookMode; }
    public Menu ActiveMenu { get => mode.activeMenu; set => SetMode(value); }
    public bool swapModes = false;
    public bool reactivateMode = false;
    
    public void SetMode(string name)
    {
        if (debug) print($"Change InputMode to {name} (in bank? {(ModeBank.ContainsKey(name) ? "yes" : "no")})");
        if (ModeBank.TryGetValue(name, out GameMode mode))
            SetMode(mode);
        else
        {
            Debug.LogWarning($"Can't find game mode for \"{name}\"");
            SetMode(InputMode.Character);
        }
    }

    public void SetMode(Menu menu)
    {
        if (debug) print($"Change InputMode to {menu} (in bank? {(MenuBank.ContainsKey(menu) ? "yes" : "no")})");
        if (MenuBank.TryGetValue(menu, out GameMode mode))
        {
            SetMode(mode);
        }
        else
        {
            if (debug) Debug.LogWarning($"Can't find game mode for \"{menu}\"");
            SetMode(InputMode.Character);
        }
    }

    public void SetMode(InputMode inputMode)
    {
        if (debug) print($"Change InputMode to {inputMode} (in bank? {(InputBank.ContainsKey(inputMode) ? "yes" : "no")})");
        if (InputBank.TryGetValue(inputMode, out GameMode mode))
            SetMode(mode);
        else
        {
            Debug.LogWarning($"Can't find game mode for \"{inputMode}\"");
            SetMode(InputMode.Character);
        }
    }

    public void SetMode(GameMode mode)
    {
        ActivateMode(mode, this.mode);
        this.mode = mode;
    }

    public void ActivateMode(GameMode mode, GameMode lastMode)
    {
        if (debug) print($"Activate inputMode {mode}.");
        
        // Configure User Interface
        if (userInterface != null)
        {
            userInterface.SetHudActive(mode.hudActive);
            userInterface.SetDialogueActive(mode.dialogueActive);
            userInterface.SetControlScreenActive(mode.activeMenu == Menu.ControlSheet);
            userInterface.SetCharacterSheetActive(mode.activeMenu == Menu.CharacterSheet);
            userInterface.SetMenuInputsActive(mode.activeMenu != Menu.None);
        }
        else
        {
            Debug.LogWarning($"UserInterface not found when changing Game Modes ({lastMode} -> {mode})");
        }

        if (volumeManager != null)
        {
            volumeManager.ToggleDeath(mode.activeShader == GameModes.Shader.Death);
        }
        else
        {
            Debug.LogWarning($"VolumeManager not found when changing Game Modes ({lastMode} -> {mode})");
        }

        foreach (Cutouts cutout in cardboardCutouts)
        {
            cutout.enabled = mode.cardboardMode;
        }

        foreach (Character character in allCharacters)
        {
            if (character != null)
            {
                SetDisplayable(character, mode.cardboardMode);
                SetBrainable(character, mode.shouldBrain);
            }
        }

        // Set Inputs
        if (mode.inputMode != InputMode.None)
            input.SwitchCurrentActionMap(mode.inputMode.ToString());
        Cursor.lockState = mode.showMouse ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = false;

        // Set Time Scale
        TimeScale = mode.timeScale;

        // Swap Controllables
        IControllable newControllable = mode.Controllable;
        IControllable oldControllable = lastMode.Controllable;
        if (newControllable != null)
            SetControllable(newControllable, true, true);
        if (oldControllable != null && oldControllable != newControllable)
            SetControllable(oldControllable, false, newControllable == null);

        // Swap Target Finders
        if (targeter != null)
        {
            TargetFinder newFinder = mode.Finder;
            TargetFinder oldFinder = lastMode.Finder;
            if (mode.Finder != null)
                targeter.Finder = mode.Finder;
            if (oldFinder != null && oldFinder != newFinder)
                targeter.Finder = null;
            targeter.SetTargetLock(mode.targetLock);
        }
        else
        {
            Debug.LogWarning($"Target not found when changing Game Modes ({lastMode} -> {mode})");
        }

        // Position Selector
        if (mode.inputMode == InputMode.Selection)
        {
            if (curController != null && curCharacter != null)
                curController.transform.position = curCharacter.body.position;
        }
    }

    // Controllables

    public void SetControllable(IControllable controllable, bool shouldControl, bool shouldSpectate)
    {
        controllable?.SetControllable(shouldControl);
        controllable?.SetSpectatable(shouldSpectate);
    }

    public void SetBrainable(IBrainable brainable, bool shouldBrain)
    {
        brainable?.SetBrainable(shouldBrain);
    }

    public void SetDisplayable(IDisplayable displayable, bool shouldDisplay)
    {
        displayable?.SetDisplayable(shouldDisplay);
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
            //if (selectable.source.TryGetComponent<Character>(out var character))
            //{
            //    hud.SetTarget(character);
            //}
            if (selectable.source.TryGetComponent<AIdentifiable>(out var identifiable))
            {
                hud.SetTarget(identifiable);
            }
        }
        selectedTarget = selectable;
    }

    // Start Dialogue

    private GameMode prevMode;
    public void StartDialogue(string nodeName)
    {
        StartDialogue(nodeName, null, null);
    }
    public void StartDialogue(string nodeName, UnityAction<string> startListener=null, UnityAction completeListener=null)
    {
        prevMode = game.Mode;
        game.InputMode = InputMode.Dialogue;
        userInterface.dialogueRunner.Stop();
        if (startListener != null)
            userInterface.dialogueRunner.onNodeStart.AddListener(startListener);
        if (completeListener != null)
            userInterface.dialogueRunner.onDialogueComplete.AddListener(completeListener);
        userInterface.dialogueRunner.StartDialogue(nodeName);
    }

    public void EndDialogue()
    {
        game.Mode = prevMode;
        userInterface.dialogueRunner.Stop();
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
            userInterface.SetCharacter(playerCharacter);
            SetControllable(curCharacter, false, false);
            SetControllable(character, true, true);
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
            SetMode(Menu.Death);
            onPlayerDied.Invoke();
        }
        else if (character == curCharacter)
        {
            SetCharacter(playerCharacter);
        }
    }
}
