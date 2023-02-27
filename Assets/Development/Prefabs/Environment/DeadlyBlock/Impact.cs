using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Impact : MonoBehaviour
{   
    // Properties

    public List<string> desiredTags;

    public BinaryEvent onCollision;
    public BinaryEvent onTrigger;

    private List<GameObject> touching;
    public GameObject other;


    // Events

    private void Start()
    {
        touching = new List<GameObject>();
    }

    private void OnEventEnter(GameObject _other, UnityEvent onEvent)
    {
        other = _other;
        if (desiredTags.Contains(other.tag) && !touching.Contains(other))
        {
            touching.Add(other);
            onEvent.Invoke();
        }
    }

    private void OnEventExit(GameObject _other, UnityEvent onEvent)
    {
        other = _other;
        if (desiredTags.Contains(other.tag))
        {
            if (touching.Contains(other))
            {
                touching.Remove(other);
            }
            onEvent.Invoke();
        }
    }


    // Signals
    
    private void OnTriggerExit(Collider _other)
    {
        OnEventExit(_other.gameObject, onTrigger.exit);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnEventEnter(collision.gameObject, onCollision.enter);
    }

    private void OnCollisionExit(Collision collision)
    {
        OnEventExit(collision.gameObject, onCollision.exit);
    }

    private void OnTriggerEnter(Collider _other)
    {
        OnEventEnter(_other.gameObject, onTrigger.enter);
    }
}
