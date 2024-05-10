using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : BaseMonoBehaviour
{
    public float length;
    public bool playOnStart = false;

    public UnityEvent onPlay;
    public UnityEvent onComplete;

    private void Start()
    {
        if (playOnStart) Play();
    }

    public void Play()
    {
        onPlay.Invoke();
        StartCoroutine(RunTimer());
        Debug.Log("Pl");
    }

    public IEnumerator RunTimer()
    {
        yield return new WaitForSeconds(length);
        onComplete.Invoke();
    }
}
