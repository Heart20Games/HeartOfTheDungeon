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


    // Events

    private void Start()
    {
        touching = new List<GameObject>();
    }

    private void OnEventEnter(GameObject other, UnityEvent onEvent)
    {
        if (desiredTags.Contains(other.tag) && !touching.Contains(other))
        {
            if (other.tag == "Player")
            {
                Debug.LogWarning("PlayerEntered");
            }
            touching.Add(other);
            onEvent.Invoke();
        }
    }

    private void OnEventExit(GameObject other, UnityEvent onEvent)
    {
        if (desiredTags.Contains(other.tag))
        {
            if (other.tag == "Player")
            {
                Debug.LogWarning("PlayerExited");
            }
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
