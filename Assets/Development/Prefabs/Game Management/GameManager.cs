using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Body;
using UnityEngine.Events;
using Selection;
using UnityEngine.SceneManagement;
using HotD.PostProcessing;
using MyBox;
using Yarn.Unity;
using UnityEngine.Assertions;

namespace HotD
{
    using static GameModes;
    using static HotD.CharacterModes;

    public class Game : BaseMonoBehaviour
    {
        public static Game main;

        // Properties
        [Foldout("Parts", true)]
        [Header("Parts")]
        public Party playerParty;
        public Selector selector;
        public SimpleController selectorController;
        public Targeter targeter;
        public GameSettings settings;
        [HideInInspector] public List<Cutouts> cardboardCutouts = new();
        [HideInInspector] public UserInterface userInterface;
        [HideInInspector] public VolumeManager volumeManager;
        [HideInInspector] public ProgressManager progressManager;
        [HideInInspector] public HUD hud;
        [HideInInspector] public List<ITimeScalable> timeScalables;
        [HideInInspector] public List<Interactable> interactables;
        [Foldout("Parts")][HideInInspector] public List<Character> allCharacters;
        private PlayerInput input;


        // Game InputMode
        [Foldout("Game Mode", true)]
        [Header("Game Mode")]
        [SerializeField] private InputMode initialInputMode = InputMode.None;
        [SerializeField] private Menu initialMenu = Menu.None;
        [SerializeField] private GameMode mode = new();
        public GameMode Mode { get => mode; set => SetMode(value); }
        public string ModeName { get => mode.name; set => SetMode(value); }
        public InputMode InputMode { get => mode.inputMode; set => SetMode(value); }
        public MoveMode MoveMode { get => mode.moveMode; }
        public LookMode LookMode { get => mode.lookMode; }
        public Menu ActiveMenu { get => mode.activeMenu; set => SetMode(value); }
        public bool swapModes = false;
        [Foldout("Game Mode")] public bool reactivateMode = false;

        // Current Character
        [Foldout("Currents", true)]
        [Header("Current Character")]
        [ReadOnly][SerializeField] private Character curCharacter;
        [ReadOnly] public SimpleController curController;
        [ReadOnly] public Selector curSelector;
        [ReadOnly] public ILooker curLooker;
        [HideInInspector] public Character CurCharacter { get => curCharacter; set => SetCharacter(value); }
        private int curCharIdx = 0;
        public int CurCharIdx { get => curCharIdx; }
        [Foldout("Currents")][ReadOnly][SerializeField] private ASelectable selectedTarget;

        // Events
        [Foldout("Events", true)]
        [Header("Events")]
        public UnityEvent onPlayerDied;
        public UnityEvent onRestartScene;
        [Foldout("Events")] public UnityEvent onRestartGame;

        // TimeScale
        [Header("TimeScale")]
        public float timeScale = 1.0f;
        public float TimeScale { get { return timeScale; } set { SetTimeScale(value); } }

        // Cheats / Shortcuts
        [Header("Cheats and Shortcuts")]
        public bool restartable = true;
        public bool debug = false;


        // Initialization

        private void Awake()
        {
            main = this;
            input = GetComponent<PlayerInput>();
            progressManager = GetComponent<ProgressManager>();
        }

        private void Start()
        {
            //if (session) session.Initialize();
            InitializePlayableCharacters();
            progressManager.RespawnToLastCheckpoint();
            if (initialMenu != Menu.None)
            {
                SetMode(initialMenu);
            }
            else if (initialInputMode != InputMode.None)
            {
                InputMode = initialInputMode;
            }
            else
            {
                InputMode = InputMode;
            }
        }

        public void InitializePlayableCharacters()
        {
            if (playerParty != null)
            {
                hud.MainCharacterSelect(playerParty.leader);
                foreach (var character in playerParty.members)
                {
                    if (character != playerParty.leader)
                        hud.AddAlly(character);
                }
                foreach (var character in playerParty.members)
                    character.onDeath.AddListener(OnCharacterDied);
            }
            hud.SetParty(playerParty);

            SetCharacterIdx(0);
            if (curCharacter == null)
                SetMode(InputMode.Character);
        }


        // Restart

        [ButtonMethod]
        public void RestartScene()
        {
            if (restartable)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        [ButtonMethod]
        public void RestartGame()
        {
            if (restartable)
                onRestartGame.Invoke();
        }
        [ButtonMethod]
        public void RestartLife()
        {
            Print("Restart Life", debug);
            if (playerParty != null)
            {
                SetCharacter(playerParty.Leader);
            }
            progressManager.RespawnToLastCheckpoint();
            if (mode.inputMode != InputMode.Character)
                SetMode(InputMode.Character);
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
            foreach (ITimeScalable timeScalable in timeScalables)
            {
                if (timeScalable != null)
                    timeScalable.SetTimeScale(this.timeScale);
                else
                    print("Invalid Object");
            }
        }

        // Game Mode

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
            if (inputMode == InputMode.LockOn && !targeter.HasTarget())
            {
                return;
            }
            else
            {
                Print($"Change InputMode to {inputMode} (in bank? {(InputBank.ContainsKey(inputMode) ? "yes" : "no")})", debug);
                if (InputBank.TryGetValue(inputMode, out GameMode mode))
                    SetMode(mode);
                else
                {
                    Debug.LogWarning($"Can't find game mode for \"{inputMode}\"");
                    SetMode(InputMode.Character);
                }
            }
        }

        public void SetMode(GameMode mode)
        {
            ActivateMode(mode, this.mode);
            this.mode = mode;
        }

        public void ActivateMode(GameMode mode, GameMode lastMode)
        {
            Print($"Activate inputMode {mode.name}.", debug);

            if (playerParty.leader != null && !playerParty.leader.Alive)
            {
                if (mode.name != lastMode.name && lastMode.playerRespawn == PlayerRespawn.OnLeave || mode.playerRespawn == PlayerRespawn.OnEnter)
                {
                    playerParty.leader.SetMode(LiveMode.Alive);
                }
            }

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
                volumeManager.SetDeath(mode.activeShader == Shader.Death);
            }
            else
            {
                Debug.LogWarning($"VolumeManager not found when changing Game Modes ({lastMode} -> {mode})");
            }

            foreach (Cutouts cutout in cardboardCutouts)
            {
                cutout.enabled = mode.cardboardMode;
            }

            // Swap Controllables
            bool canSpectate = mode.lookMode == LookMode.Character;
            IControllable newControllable = mode.Controllable;
            IControllable oldControllable = lastMode.Controllable;
            if (newControllable != null)
                SetControllable(newControllable, true, canSpectate);
            if (oldControllable != null && oldControllable != newControllable)
                SetControllable(oldControllable, false, newControllable == null && canSpectate);

            foreach (Character character in allCharacters)
            {
                Print($"{character.Name} is in mode {character.mode.name}. (GameManager)", debug, this);
                if (character != null && character.Alive && (character != curCharacter))
                {
                    Print($"Setting {character.Name} to control mode {ControlMode.Brain}.", debug, this);
                    character.SetMode(ControlMode.Brain);
                    // May require a "cardboard" Character Mode
                    //if (mode.cardboardMode)
                    //    SetDisplayable(character, false);
                }
            }

            // Set Inputs
            if (mode.inputMode != InputMode.None)
                input.SwitchCurrentActionMap(mode.inputMode.ToString());
            Cursor.lockState = mode.showMouse ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = false;

            // Set Time Scale
            TimeScale = mode.timeScale;

            // Swap Target Finders
            if (targeter != null)
            {
                TargetFinder newFinder = mode.Finder;
                TargetFinder oldFinder = lastMode.Finder;
                if (newFinder != null)
                    targeter.Finder = newFinder;
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
            if (controllable != null && controllable.Alive)
                controllable.Controllable = shouldControl;
            controllable?.SetSpectatable(shouldSpectate);
        }

        // Selectables

        public void OnTargetSelected(ASelectable selectable)
        {

            if (selectable == null)
            {
                Print($"Target deselected: {selectedTarget}", debug);
                hud.SetTarget(null);
            }
            else
            {
                Print($"Target selected: {selectable}", debug);
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
        public void StartDialogue(string nodeName, UnityAction<string> startListener = null, UnityAction completeListener = null)
        {
            prevMode = main.Mode;
            main.InputMode = InputMode.Dialogue;
            userInterface.dialogueRunner.Stop();
            if (startListener != null)
                userInterface.dialogueRunner.onNodeStart.AddListener(startListener);
            if (completeListener != null)
                userInterface.dialogueRunner.onDialogueComplete.AddListener(completeListener);
            userInterface.dialogueRunner.StartDialogue(nodeName);
        }

        public void EndDialogue()
        {
            main.Mode = prevMode;
            userInterface.dialogueRunner.Stop();
        }

        // Set Characters

        public void SetCharacterIdx(int idx)
        {
            List<Character> members = playerParty.members;
            if (members.Count > 0)
            {
                idx = idx < 0 ? members.Count + idx : idx;
                curCharIdx = idx % (members.Count);
                Character character = members[curCharIdx];
                SetCharacter(character);
                hud.CharacterSelect(character);
            }
        }

        public void SetCharacter(Character character)
        {
            if (character != null)
            {
                userInterface.SetCharacter(character);
                SetControllable(curCharacter, false, false);
                if (Mode.inputMode == InputMode.Character)
                    SetControllable(character, true, true);
                curCharacter = character;
                if (!character.Alive)
                    SetMode(InputMode.Spectate);
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
            if (character == playerParty.leader)
            {
                SetMode(Menu.Death);
                onPlayerDied.Invoke();
            }
            else if (character == curCharacter)
            {
                SetCharacter(playerParty.leader);
            }
        }
    }
}