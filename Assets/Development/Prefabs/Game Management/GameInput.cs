using UnityEngine;
using static Game;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Body;
using Body.Behavior;
using static GameModes;

[RequireComponent(typeof(Game))]
[RequireComponent(typeof(PlayerInput))]

public class GameInput : MonoBehaviour
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
    public GameMode Mode { get => Game.Mode; set => Game.Mode = value; }


    // Initialization
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Wrappers
    public delegate void Delegate();
    public void IsPressed(InputValue inputValue, Delegate del)
    {
        if (inputValue.isPressed)
            del.Invoke();
    }


    // Actions

    // Cheats / Shortcuts
    public void OnRestartLevel(InputValue inputValue)
    {
        if (inputValue.isPressed)
            if (Game.restartable)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Movement
    public void OnMove(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();
        switch (Game.Mode)
        {
            case GameMode.Character:
                if (Game.CanUseCharacter())
                    CurCharacter.MoveCharacter(inputVector);
                break;
            case GameMode.Selection:
                if (Game.CanUseSelector())
                    CurController.MoveVector = inputVector;
                break;
        }
    }

    // Aiming
    public void OnAim(InputValue inputValue) { CurCharacter.Aim(inputValue.Get<Vector2>()); }
    public void OnToggleAiming(InputValue inputValue) { CurCharacter.SetAimModeActive(inputValue.isPressed); }

    // Castables
    public enum CastableIdx { Ability1, Ability2, Weapon1, Weapon2, Agility }
    public void UseCastable(InputValue inputValue, CastableIdx idx) { IsPressed(inputValue, () => { CurCharacter.ActivateCastable((int)idx); }); }
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
            Delegate del = deSelect ? CurSelector.DeSelect : CurSelector.Select;
            if (Game.CanUseSelector())
                del.Invoke();
        }
    }
    public void OnSelect(InputValue inputValue) { SelectValue(inputValue, true); }
    public void OnDeSelect(InputValue inputValue) { SelectValue(inputValue, false); }

    // Lock-On
    public void OnToggleLockOn(InputValue inputValue) { IsPressed(inputValue, () => { Mode = Mode == GameMode.LockedOn ? GameMode.Character : GameMode.LockedOn; }); }
    public void SwitchTargets(InputValue inputValue) { Targeter.SwitchTargets(inputValue.Get<float>() < 0); }

    // Skill Wheel
    public void OnToggleSkillWheel(InputValue inputValue)
    {
        if (Game.settings.useD20Menu && inputValue.isPressed)
        {
            Mode = Mode == GameMode.Character ? GameMode.Selection : GameMode.Character;
            Hud.abilityMenu.Toggle();
        }
    }

    // Control Screen
    public void OnToggleControls(InputValue inputValue) { IsPressed(inputValue, () => { Mode = GameMode.Dismiss; }); }
    public void OnDismiss(InputValue inputValue) { IsPressed(inputValue, () => { Mode = GameMode.Character; }); }

    // Dialogue
    public void OnContinue(InputValue inputValue) { IsPressed(inputValue, UserInterface.Continue ); }
}
