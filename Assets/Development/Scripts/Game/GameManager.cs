using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Selection;
using UnityEngine.SceneManagement;
using HotD.PostProcessing;
using MyBox;
using Yarn.Unity;
using HotD.Body;
using HotD.UI;

namespace HotD
{
    using static GameModes;
    using static HotD.CharacterModes;

    /*
     * The Game Manager is responsible for all of the high-level game behaviours.
     * 
     * It does this by coordinating different parts of the game using Game Modes.
     */

    public class Game : BaseMonoBehaviour
    {
        public static Game main;
        public static Game Main
        {
            get
            {
                if (main == null)
                    main = FindObjectOfType<Game>();
                return main;
            }
            set => main = value;
        }
        private void OnDestroy()
        {
            if (Game.main == this) Game.main = null;
        }

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
        [HideInInspector] public List<Interactable> interactables;
        [Foldout("Parts")][HideInInspector] public List<Character> allCharacters;
        private PlayerInput input;


        // Game InputMode
        [Foldout("Game Mode", true)]
        [Header("Game Mode")]
        public string dialogueMode = "Dialogue";
        public string cutSceneMode = "Cutscene";
        [SerializeField] private InputMode initialInputMode = InputMode.None;
        [SerializeField] private Menu initialMenu = Menu.None;
        [SerializeField] private GameMode mode = new();
        public GameMode Mode { get => mode; set => SetMode(value); }
        public string ModeName { get => mode.name; set => SetMode(value); }
        public InputMode InputMode { get => mode.inputMode; set => SetMode(value); }
        public MoveMode MoveMode { get => mode.moveMode; }
        public LookMode LookMode { get => mode.lookMode; }
        public Menu ActiveMenu { get => mode.activeMenu; set => SetMode(value); }
        [SerializeField] private string swapToMode = "";
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
        [SerializeField] protected bool reloadScene = false;
        public UnityEvent onRestartScene;
        public UnityEvent onRestartLife;
        [Foldout("Events")] public UnityEvent onRestartGame;

        // Cheats / Shortcuts
        [Header("Cheats and Shortcuts")]
        public bool restartable = true;
        public bool debug = false;
        [SerializeField] private bool canSwitchCharacters;


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
            bool spawnedMainParty = progressManager.RespawnToLastCheckpoint();
            if (!spawnedMainParty) Party.mainParty.Respawn();
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
            Warning("No HUD found!", HUD.main, this);

            if (playerParty != null)
            {
                Character leader = playerParty.leader;
                if (HUD.main)
                {
                    HUD.main.MainCharacterSelect(leader);
                    foreach (var character in playerParty.members)
                    {
                        if (character != playerParty.leader)
                            HUD.main.AddAlly(character);
                    }
                }
                playerParty.onMemberDeath = OnCharacterDied;
            }
            if (HUD.main) HUD.main.SetParty(playerParty);

            SetCharacterIdx(0);
            if (curCharacter == null)
                SetMode(InputMode.Selection);
        }


        // Restart

        [ButtonMethod]
        public void RestartScene()
        {
            if (restartable && reloadScene)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                onRestartScene.Invoke();
            }
        }
        [ButtonMethod]
        public void RestartGame()
        {
            if (restartable)
                onRestartGame.Invoke();
        }

        private bool restartingLife = false;
        [ButtonMethod]
        public void RestartLife()
        {
            restartingLife = true;
            Print("Restart Life", debug);
            if (playerParty != null)
            {
                SetCharacter(playerParty.Leader);
            }
            if (!progressManager.RespawnToLastCheckpoint())
            {
                playerParty.Respawn();
            }
            if (mode.inputMode != InputMode.Character)
            {
                SetMode(InputMode.Character);
            }
            onRestartLife.Invoke();
            restartingLife = false;
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
                if (swapToMode == "")
                    InputMode = InputMode;
                else
                    SetMode(swapToMode);
            }
        }


        // Boss Stuff

        public void ShowBossHud()
        {
            userInterface.CanDismissBossHUD = false;
            SetMode(Menu.BossHud);
        }

        public void HideBossHud()
        {
            userInterface.CanDismissBossHUD = true;
            userInterface.SetBossHudActive(false);
        }

        // End Demo
        
        public void ShowEndDemoScreen()
        {
            SetMode(Menu.EndDemo);
        }

        // Start Game

        [Foldout("Events")]
        [SerializeField] protected UnityEvent onStartGame = new();
        [ButtonMethod]
        public void StartGame()
        {
            SetCharacter(Party.mainParty.Leader);
            SetMode("Character");
            //SetMode(InputMode.Character);
            onStartGame.Invoke();
        }


        // Setters

        // Game Mode

        public void SetMode(string name)
        {
            if (debug) print($"Change InputMode to {name} (in bank? {(ModeBank.ContainsKey(name) ? "yes" : "no")})");
            if (ModeBank.TryGetValue(name, out GameMode mode))
            {
                if (mode.active)
                    SetMode(mode);
            }
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
                if (mode.active) SetMode(mode);
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
                if (debug) print($"Change InputMode to {inputMode} (in bank? {(InputBank.ContainsKey(inputMode) ? "yes" : "no")})");
                if (InputBank.TryGetValue(inputMode, out GameMode mode))
                {
                    if (mode.active) SetMode(mode);
                }
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
            Print($"Activate inputMode {mode}.", debug);

            // Configure User Interface
            if (userInterface != null)
            {
                userInterface.SetHudActive(mode.hudActive);
                userInterface.SetDialogueActive(mode.dialogueActive);
                userInterface.SetBossHudActive(mode.activeMenu == Menu.BossHud);
                userInterface.SetControlScreenActive(mode.activeMenu == Menu.ControlSheet);
                userInterface.SetCharacterSheetActive(mode.activeMenu == Menu.CharacterSheet);
                userInterface.SetSimpleDialogueActive(mode.activeMenu == Menu.EndDemo);
                userInterface.SetMenuInputsActive(mode.activeMenu != Menu.None);
                userInterface.SetDeathScreenActive(mode.activeMenu == Menu.Death);
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
                SetControllable(newControllable, true, canSpectate, restartingLife);
            if (oldControllable != null && oldControllable != newControllable)
                SetControllable(oldControllable, false, newControllable == null && canSpectate, restartingLife);

            foreach (Character character in allCharacters)
            {
                if (character != null && character.Alive && !character.PlayerControlled)
                {
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
            TimeScaler.TimeScale = mode.timeScale;

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
                    curController.transform.position = curCharacter.Body.position;
            }
        }

        // Controllables

        public void SetControllable(IControllable controllable, bool shouldControl, bool shouldSpectate, bool ignoreDeath = false)
        {
            controllable?.SetPlayerControlled(shouldControl, ignoreDeath);
            controllable?.SetSpectatable(shouldSpectate);
        }

        // Selectables

        public void OnTargetSelected(ASelectable selectable)
        {

            if (selectable == null)
            {
                Print($"Target deselected: {selectedTarget}", debug);
                HUD.main.SetTarget(null);
            }
            else
            {
                Print($"Target selected: {selectable}", debug);
                if (selectable.source.TryGetComponent<AIdentifiable>(out var identifiable))
                {
                    HUD.main.SetTarget(identifiable);
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
            DialogueRunner dialogueRunner = userInterface.dialogueRunner;

            prevMode = main.Mode;
            main.InputMode = InputMode.Dialogue;
            userInterface.dialogueRunner.Stop();
            if (startListener != null)
                dialogueRunner.onNodeStart.AddListener(startListener);
            if (completeListener != null)
                dialogueRunner.onDialogueComplete.AddListener(completeListener);

            YarnProject yarnProject = dialogueRunner.yarnProject;
            if (yarnProject.NodeNames.Length <= 0)
            {
                Debug.LogWarning($"{yarnProject.name} has no node names listed.", this);
            }
            else
            {
                string nameLog = $"{yarnProject.name} has node names:";
                foreach (string name in userInterface.dialogueRunner.yarnProject.NodeNames)
                {
                    nameLog += $"\n{name}";
                }
                Debug.Log(nameLog, this);
            }

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
                if (members[idx % (members.Count)].mode.Alive)
                {
                    idx = idx < 0 ? members.Count + idx : idx;
                    curCharIdx = idx % (members.Count);
                    Character character = members[curCharIdx];
                    SetCharacter(character);
                    HUD.main.CharacterSelect(character);
                }
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
            if (!canSwitchCharacters) return;

            if (curCharIdx == idx)
                SetCharacterIdx(0);
            else
                SetCharacterIdx(idx);
        }


        // Events

        public void OnCharacterDied(Character character)
        {
            Print($"Character died: {character.Name} ({character.mode.liveMode})", debug);
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