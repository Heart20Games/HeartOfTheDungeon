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
    public List<SelectType> SelectableTypes { get { return selectableTypes; } set { SetSelectableTypes(value); } }

    public BinaryEvent onCollision;
    public BinaryEvent onTrigger;

    public bool debug = false;

    public readonly List<GameObject> touching = new();
    [HideInInspector] public GameObject other;
    [HideInInspector] public ASelectable selectable;

    private void SetSelectableTypes(List<SelectType> selectableTypes)
    {
        print("Set Types on Impact");
        this.selectableTypes = selectableTypes;
    }

    private bool HasValidTag(GameObject other)
    {
        if (debug && desiredTags.Count != 0)
        {
            print("Valid Tag? " + (desiredTags.Count == 0 || desiredTags.Contains(other.tag)) + " (" + other.tag + ")");
        }
        return desiredTags.Count == 0 || desiredTags.Contains(other.tag);
    }

    private bool IsValidSelectable(GameObject other)
    {
        if (selectableTypes.Count > 0)
        {
            selectable = other.GetComponent<ASelectable>();
            if (debug && selectable != null)
            {
                print("Valid Selectable?" + (selectable != null && selectableTypes.Contains(selectable.Type)) + " (" + selectable.Type + ")");
            }
            return selectable != null && selectableTypes.Contains(selectable.Type);
        }
        else return true;
    }

    // Events

    private void OnEventEnter(GameObject other, UnityEvent onEvent)
    {
        this.other = other;
        if (HasValidTag(other) && IsValidSelectable(other) && !touching.Contains(other))
        {
            touching.Add(other);
            onEvent.Invoke();
        }
    }

    private void OnEventExit(GameObject other, UnityEvent onEvent)
    {
        this.other = other;
        if (HasValidTag(other) && IsValidSelectable(other))
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
