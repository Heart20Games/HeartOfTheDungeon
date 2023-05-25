using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : BaseMonoBehaviour
{
    public float length;

    public UnityEvent onPlay;
    public UnityEvent onComplete;

    public void Play()
    {
        onPlay.Invoke();
        StartCoroutine(RunTimer());
    }

    public IEnumerator RunTimer()
    {
        yield return new WaitForSeconds(length);
        onComplete.Invoke();
    }
}
