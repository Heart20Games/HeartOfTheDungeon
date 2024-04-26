using Body;
using MyBox;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;
using static ISelectable;

public class Validator : BaseMonoBehaviour
{
    [Foldout("Validation", true)]
    [Header("Validation: Flags")]
    public bool desireInteractors = false;
    public bool controlledCharactersOnly = false;
    public bool useTargetIdentity = false;
    [ConditionalField("useTargetIdentity", false, true)]
    public Identity targetIdentity = Identity.Neutral;
    [Space()]

    // Validation: Tags
    public List<string> desiredTags;

    // Validation: Types
    public List<SelectType> selectableTypes;
    public List<SelectType> SelectableTypes { get { return selectableTypes; } set { SetSelectableTypes(value); } }
    [Foldout("Validation")][HideInInspector] public ASelectable selectable;

    [Header("Miscelaneous")]
    public bool debug = false;

    // Setters
    private void SetSelectableTypes(List<SelectType> selectableTypes)
    {
        if (debug) print("Set Types on Impact");
        this.selectableTypes = selectableTypes;
    }


    // Validation
    public bool Validate(GameObject other, Identity identity=Identity.Neutral)
    {
        return HasValidTag(other) && IsValidSelectable(other) && IsValidInteractor(other) && IsValidCharacter(other) && IsValidTarget(identity, other);
    }


    // Validators
    private bool IsValidTarget(Identity identity, GameObject other)
    {
        if (useTargetIdentity)
        {
            if (!other.TryGetComponent<AIdentifiable>(out var idable))
            {
                idable = other.GetComponentInParent<AIdentifiable>();
            }
            if (debug) print($"Found target: {idable}");
            if (idable != null)
            {
                if (debug) print($"Target Valid? {RelativeIdentity(identity, idable.Identity) == targetIdentity} ({idable}, {identity} vs {idable.Identity})");
                return Match(RelativeIdentity(identity, idable.Identity), targetIdentity);
            }
            else return false;
        }
        else return true;
    }

    private bool HasValidTag(GameObject other)
    {
        if (desiredTags.Count > 0)
        {
            if (other == gameObject)
                Debug.LogWarning($"Colliding with self: {other.tag}-{other.name}");
            Print($"Valid Tag? {(desiredTags.Count == 0 || desiredTags.Contains(other.tag))} (them:{other.tag}-{other.name} me:{gameObject.tag}-{gameObject.name})", debug);
            return desiredTags.Contains(other.tag);
        }
        else return true;
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
            if (other.CompareTag("Character"))
            {
                Character character = other.GetComponentInParent<Character>();
                if (character != null)
                    return character.PlayerControlled;
            }
        }
        return true;
    }
}
