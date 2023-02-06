using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Impact : MonoBehaviour
{
    public List<string> desiredTags;
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    private void OnCollisionEnter(Collision collision)
    {
        if (desiredTags.Contains(collision.gameObject.tag))
        {
            onPlayerEnter.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (desiredTags.Contains(collision.gameObject.tag))
        {
            onPlayerExit.Invoke();
        }
    }
}
