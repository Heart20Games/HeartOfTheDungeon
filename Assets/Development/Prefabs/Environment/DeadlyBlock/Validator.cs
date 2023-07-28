using Body;
using System.Collections.Generic;
using UnityEngine;
using static ISelectable;

public class Validator : BaseMonoBehaviour
{
    public bool desireInteractors = false;
    public bool controlledCharactersOnly = false;

    [Header("Tags")]
    public List<string> desiredTags;

    [Header("Selectable")]
    [SerializeField] private List<SelectType> selectableTypes;
    public List<SelectType> SelectableTypes { get { return selectableTypes; } set { SetSelectableTypes(value); } }
    [HideInInspector] public ASelectable selectable;

    [Space]
    public bool debug = false;

    // Setters
    private void SetSelectableTypes(List<SelectType> selectableTypes)
    {
        if (debug) print("Set Types on Impact");
        this.selectableTypes = selectableTypes;
    }


    // Validation
    public bool Validate(GameObject other)
    {
        return HasValidTag(other) && IsValidSelectable(other) && IsValidInteractor(other) && IsValidCharacter(other);
    }


    // Validators
    private bool HasValidTag(GameObject other)
    {
        if (other == gameObject)
        {
            Debug.LogWarning($"Colliding with self: {other.tag}-{other.name}");
        }
        if (debug)
            print($"Valid Tag? {(desiredTags.Count == 0 || desiredTags.Contains(other.tag))} (them:{other.tag}-{other.name} me:{gameObject.tag}-{gameObject.name})");
        return desiredTags.Count == 0 || desiredTags.Contains(other.tag);
    }

    private bool IsValidSelectable(GameObject other)
    {
        if (selectableTypes.Count > 0)
        {
            selectable = other.GetComponent<ASelectable>();
            if (debug && selectable != null)
                print($"Valid Selectable? {selectable != null && selectableTypes.Contains(selectable.Type)} ({selectable.Type})");
            return selectable != null && selectableTypes.Contains(selectable.Type);
        }
        else return true;
    }

    private bool IsValidInteractor(GameObject other)
    {
        return !desireInteractors || (other.TryGetComponent<Interactor>(out var interactor) && interactor.enabled);
    }

    private bool IsValidCharacter(GameObject other)
    {
        if (controlledCharactersOnly)
        {
            if (other.gameObject.tag == "Character")
            {
                Character character = other.GetComponentInParent<Character>();
                if (character != null)
                    return character.controllable;
                else return true;

            }
            else return true;
        }
        else return true;
    }
}
