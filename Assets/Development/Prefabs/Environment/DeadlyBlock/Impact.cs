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

    private List<GameObject> touching;

    private void Start()
    {
        touching = new List<GameObject>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (desiredTags.Contains(other.tag) && !touching.Contains(other))
        {
            touching.Add(other);
            onTouch.Invoke();
        }
        if(other.layer == 7) // Layer 7 is character
        {
            Debug.Log("Test");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (desiredTags.Contains(other.tag))
        {
            if (touching.Contains(other))
            {
                touching.Remove(other);
            }
            onUnTouch.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherG = other.gameObject;
        if (desiredTags.Contains(other.gameObject.tag) && !touching.Contains(otherG))
        {
            touching.Add(otherG);
            onEnter.Invoke();
        }
        if (otherG.layer == 7) // Layer 7 is character
        {
            Debug.Log("Test");
        }
    }

    

    private void OnTriggerExit(Collider other)
    {
        GameObject otherG = other.gameObject;
        if (desiredTags.Contains(otherG.tag))
        {
            if (touching.Contains(otherG))
            {
                touching.Remove(otherG);
            }
            onExit.Invoke();
        }
    }
}
