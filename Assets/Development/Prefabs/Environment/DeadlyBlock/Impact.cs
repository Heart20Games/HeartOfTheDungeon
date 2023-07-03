using Body;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ISelectable;

public class Impact : BaseMonoBehaviour
{
    // Properties

    public List<string> desiredTags;
    [SerializeField] private List<SelectType> selectableTypes;
    public bool desireInteractors = false;
    public bool controlledCharactersOnly = false;
    public List<SelectType> SelectableTypes { get { return selectableTypes; } set { SetSelectableTypes(value); } }

    public BinaryEvent onCollision;
    public BinaryEvent onTrigger;

    public bool oneShot = false;
    public bool hasCollided = false;

    public void SetHasCollided(bool collided)
    {
        hasCollided = collided;
    }
    public bool debug = false;

    public readonly List<GameObject> touching = new();
    [HideInInspector] public GameObject other;
    [HideInInspector] public ASelectable selectable;

    private void SetSelectableTypes(List<SelectType> selectableTypes)
    {
        if (debug)
            print("Set Types on Impact");
        this.selectableTypes = selectableTypes;
    }

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
            {
                if (debug)
                    print($"Valid Selectable? {(selectable != null && selectableTypes.Contains(selectable.Type))} ({selectable.Type})");
            }
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
        if (controlledCharactersOnly && other.TryGetComponent(out Character character))
        {
            return character.controllable;
        }
        else return true;
    }

    // Events

    private void OnEventEnter(GameObject other, UnityEvent onEvent)
    {
        this.other = other;
        if ((!oneShot || !hasCollided) && HasValidTag(other) && IsValidSelectable(other) && IsValidInteractor(other) && IsValidCharacter(other) && !touching.Contains(other))
        {
            print($"Other: {other.name}");
            touching.Add(other);
            onEvent.Invoke();
            hasCollided = true;
        }
    }

    private void OnEventExit(GameObject other, UnityEvent onEvent)
    {
        this.other = other;
        if (HasValidTag(other) && IsValidSelectable(other) && IsValidInteractor(other))
        {
            if (touching.Contains(other))
            {
                touching.Remove(other);
            }
            onEvent.Invoke();
        }
    }


    // Signals
    
    private void OnTriggerExit(Collider other)
    {
        OnEventExit(other.gameObject, onTrigger.exit);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnEventEnter(collision.gameObject, onCollision.enter);
    }

    private void OnCollisionExit(Collision collision)
    {
        OnEventExit(collision.gameObject, onCollision.exit);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEventEnter(other.gameObject, onTrigger.enter);
    }
}
