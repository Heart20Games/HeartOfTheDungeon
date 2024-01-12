using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Body;
using static GameModes;
using Selection;
using System.Collections;
using System.Diagnostics.Tracing;
using MyBox;

[RequireComponent(typeof(Game))]
[RequireComponent(typeof(PlayerInput))]

public class GameInput : BaseMonoBehaviour
{
    private Game game;
    public Game Game { get { game = game != null ? game : GetComponent<Game>(); return game; } }

    // Systems
    public UserInterface UserInterface { get => Game.userInterface; }
    public HUD Hud { get => Game.hud; }

    // Fields
    public Character CurCharacter { get => Game.CurCharacter; }
    public Selector CurSelector { get => Game.curSelector; }
    public SimpleController CurController { get => Game.curController; }
    public Targeter Targeter { get => Game.targeter; }
    public ILooker CurLooker { get => Game.curLooker; }
    public InputMode Input { get => Game.InputMode; set => Game.InputMode = value; }
    public Menu Menu { get => Game.ActiveMenu; set => Game.ActiveMenu = value; }


    // Wrappers
    public delegate void Delegate();
    public void IsPressed(InputValue inputValue, Delegate yesDel, Delegate noDel=null)
    {
        if (inputValue.isPressed)
            yesDel?.Invoke();
        else
            noDel?.Invoke();
    }


    // Actions

    [Header("Debugs")]
    [SerializeField] private bool debugMove;
    [SerializeField] private bool debugLook;
    [SerializeField] private bool debugAim;

    // Movement
    public void OnMove(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();
        if (debugLook) print($"OnMove: {inputVector} in {Game.MoveMode}");
        switch (Game.MoveMode)
        {
            case MoveMode.Character:
                if (CurCharacter != null)
                    CurCharacter.MoveCharacter(inputVector);
                break;
            case MoveMode.Selector:
                if (CurController != null)
                    CurController.MoveVector = inputVector;
                break;
        }
    }

    // Looking
    public void OnLook(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();
        if (debugLook) print($"OnLook: {inputVector} in {Game.LookMode}");
        switch (Game.LookMode)
        {
            case LookMode.Targeter:
                Game.Mode.Looker?.Look(inputVector); break;
        }
    }

    // Aiming
    public void OnAim(InputValue inputValue) { if (CurCharacter != null) CurCharacter.Aim(inputValue.Get<Vector2>()); }
    public void OnToggleAiming(InputValue inputValue) { if (CurCharacter != null) CurCharacter.SetAimModeActive(inputValue.isPressed); }

    // Castables
    public enum CastableIdx { Ability1, Ability2, Weapon1, Weapon2, Agility }
    public void UseCastable(InputValue inputValue, CastableIdx idx)
    {
        if (CurCharacter != null)
        {
            IsPressed(inputValue, 
                () => { CurCharacter.TriggerCastable((int)idx); },
                () => { CurCharacter.ReleaseCastable((int)idx); }
            ); 
        }
    }
    public void OnUseAgility(InputValue inputValue) { UseCastable(inputValue, CastableIdx.Agility); }
    public void OnUseWeapon1(InputValue inputValue) { UseCastable(inputValue, CastableIdx.Weapon1); }
    public void OnUseWeapon2(InputValue inputValue) { UseCastable(inputValue, CastableIdx.Weapon2); }
    public void OnUseAbility1(InputValue inputValue) { UseCastable(inputValue, CastableIdx.Ability1); }
    public void OnUseAbility2(InputValue inputValue) { UseCastable(inputValue, CastableIdx.Ability2); }

    // Character Switching
    public void SwitchCharacterInput(InputValue inputValue, int idx) { IsPressed(inputValue, () => { game.SwitchToCompanion(idx); }); }
    public void OnSwitchCharacterLeft(InputValue inputValue) { SwitchCharacterInput(inputValue, 2); }
    public void OnSwitchCharacterRight(InputValue inputValue) { SwitchCharacterInput(inputValue, 1); }
    //public void SetCharacterInput(InputValue inputValue, int idx) { IsPressed(inputValue, () => { game.SetCharacterIdx(idx); }); }
    //public void OnSwitchCharacterCenter(InputValue inputValue) { SetCharacterInput(inputValue, 0); }
    //public void OnCycleCharacterLeft(InputValue inputValue) { SetCharacterInput(inputValue, game.CurCharIdx - 1); }
    //public void OnCycleCharacterRight(InputValue inputValue) { SetCharacterInput(inputValue, game.CurCharIdx + 1); }

    // Interaction
    public void OnInteract(InputValue inputValue)
    {
        if (inputValue.isPressed)
            foreach (Interactable interactable in Game.interactables)
                interactable.Interact();
    }

    // Selector
    public void SelectValue(InputValue inputValue, bool deSelect = false)
    {
        if (inputValue.isPressed)
        {
            switch (Input)
            {
                case InputMode.Selection:
                    Delegate del = deSelect ? CurSelector.DeSelect : CurSelector.Select;
                    del.Invoke(); break;
                case InputMode.Menu:
                    UserInterface.Select(); break;
            }
        }
    }
    public void OnSelect(InputValue inputValue) { SelectValue(inputValue, true); }
    public void OnDeSelect(InputValue inputValue) { SelectValue(inputValue, false); }

    // Lock-On
    public void OnToggleLockOn(InputValue inputValue)
    {
        IsPressed(inputValue, () =>
        {
            switch (Input)
            {
                case InputMode.LockOn:
                    Input = InputMode.Character; break;
                default:
                    if (Targeter.HasTarget()) Input = InputMode.LockOn; break;
            };
        });
    }
    public void OnSwitchTargets(InputValue inputValue)
    {
        SwitchTargets(inputValue.Get<float>());
    }
    public void OnSwitchTargetsScroll(InputValue inputValue)
    {
        scroll += inputValue.Get<float>();
        if (!scrollerStarted)
            scroller = StartCoroutine(ScrollPoll());
    }

    [Foldout("Target Switching", true)]
    [Header("Target Switching")]
    [SerializeField] private float holdTime = 0.8f;
    [ReadOnly][SerializeField] private bool reachedZero = true;
    [ReadOnly][SerializeField] int triggerCount = 0;
    [ReadOnly][SerializeField] private float switchTargetValue = 0f;
    [Foldout("Target Switching")]
    [SerializeField] private bool debugTS;
    public void SwitchTargets(float value)
    {
        switchTargetValue = value;
        if (debugTS) print($"OnSwitchTargets ({switchTargetValue})");
        if (switchTargetValue != 0 && reachedZero)
        {
            reachedZero = false;
            triggerCount += 1;
            StartCoroutine(SwitchTarget(triggerCount));
        }
        else if (switchTargetValue == 0 && !reachedZero)
        {
            reachedZero = true;
        }
    }
    private IEnumerator SwitchTarget(int triggerIdx)
    {
        Targeter.SwitchTargets(switchTargetValue < 0);
        yield return new WaitForSeconds(holdTime);
        if (triggerIdx == triggerCount)
            reachedZero = true;
    }

    [Foldout("Target Scroll Polling", true)]
    [Header("Target Scroll Polling")]
    [SerializeField] private float scrollPollPerHold = 10;
    [SerializeField] private bool polling = true;
    [SerializeField] private float scrollHoldTime = 0.4f;
    [ReadOnly][SerializeField] private float pollTime = 0f;
    private Coroutine scroller;
    [ReadOnly][SerializeField] private bool scrollerStarted = false;
    [ReadOnly][SerializeField] private float scroll = 0f;
    [ReadOnly][SerializeField] private float scrollHold = 0f;
    [Foldout("Target Scroll Polling")]
    [SerializeField] private bool debugSP = false;
    private IEnumerator ScrollPoll()
    {
        scrollerStarted = true;
        if (debugSP) print("Starting Scroll Polling");
        while (polling)
        {
            //float time = Mathf.Min(holdTime / scrollPollPerHold, holdTime - Mathf.Clamp(pollTime, 0, holdTime));
            float time = holdTime / scrollPollPerHold;
            yield return new WaitForSeconds(time);
            scrollHold += scroll;
            pollTime += time;
            if (debugSP) print($"Zero? {scroll == 0}");
            if (scroll == 0f)
            {
                if (scrollHold != 0)
                    SwitchTargets(0f);
                scrollHold = 0f;
                pollTime = 0f;
            }
            else if (pollTime >= scrollHoldTime)
            {
                SwitchTargets(Mathf.Clamp(scrollHold, -1, 1));
            }
            scroll = 0f;
        }
    }

    // Zoom
    public void OnZoom(InputValue inputValue)
    {
        float value = inputValue.Get<float>();
        if (value != 0f)
        {
            switch (Game.LookMode)
            {
                case LookMode.Targeter:
                    Game.Mode.Looker?.Zoom(value < 0); break;
            }
        }
    }

    // Skill Wheel
    public void OnToggleSkillWheel(InputValue inputValue)
    {
        if (Game.settings.useD20Menu && inputValue.isPressed)
        {
            Input = Input == InputMode.Character ? InputMode.Selection : InputMode.Character;
            Hud.abilityMenu.Toggle();
        }
    }

    // Dialogue
    public void OnContinue(InputValue inputValue) 
    {
        IsPressed(inputValue, () =>
        {
            if (Input != InputMode.Cutscene)
                UserInterface.Continue();
            else
                return;
        });
    }

    // Tests
    public void OnTest(InputValue inputValue)
    {
        float testFloat = inputValue.Get<float>();
        print($"Test float: {testFloat}");
    }

    // Mouse
    public float hideMouseDelay = 10f;
    [ReadOnly][SerializeField] private bool mouseShown;
    public void ShowMouse(bool showMouse)
    {
        mouseShown = showMouse;
        Cursor.visible = mouseShown;
        StopCoroutine(HideMouseDelay());
        if (mouseShown)
        {
            StartCoroutine(HideMouseDelay());
        }
    }
    private IEnumerator HideMouseDelay()
    {
        yield return new WaitForSeconds(hideMouseDelay);
        ShowMouse(false);
    }
    public void OnMouseDelta(InputValue inputValue)
    {
        if (Game.Mode.showMouse)
        {
            Vector2 output = inputValue.Get<Vector2>();
            if (output != Vector2.zero)
            {
                if (!mouseShown) ShowMouse(true);
            }
        }
    }
    public void OnMouseClick(InputValue inputValue)
    {
        if (Game.Mode.showMouse)
        {
            IsPressed(inputValue, () =>
            {
                if (!mouseShown) ShowMouse(true);
            });
        }
    }

    // Menus
    public void OnControlSheet(InputValue inputValue) { IsPressed(inputValue, () => { Menu = Menu == Menu.ControlSheet ? Menu.None : Menu.ControlSheet; }); }
    public void OnCharacterSheet(InputValue inputValue) { IsPressed(inputValue, () => { Menu = Menu == Menu.CharacterSheet ? Menu.None : Menu.CharacterSheet; }); }
    public void OnPauseMenu(InputValue inputValue) { IsPressed(inputValue, () => { return; }); }
    public void OnDismiss(InputValue inputValue) { IsPressed(inputValue, () => { Input = InputMode.Character; }); }
}
