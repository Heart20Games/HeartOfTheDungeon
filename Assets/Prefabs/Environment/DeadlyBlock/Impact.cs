using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Impact : MonoBehaviour
{
    public List<string> desiredTags;
    public UnityEvent onTouch;
    public UnityEvent onUnTouch;
    public UnityEvent onEnter;
    public UnityEvent onExit;

    private void OnCollisionEnter(Collision collision)
    {
        if (desiredTags.Contains(collision.gameObject.tag))
        {
            onTouch.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (desiredTags.Contains(collision.gameObject.tag))
        {
            onUnTouch.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (desiredTags.Contains(other.gameObject.tag))
        {
            onEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (desiredTags.Contains(other.gameObject.tag))
        {
            onExit.Invoke();
        }
    }
}
