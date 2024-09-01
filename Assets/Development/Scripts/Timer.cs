using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : BaseMonoBehaviour
{
    [SerializeField] public float length;
    public float Length
    {
        get => length;
        set
        {
            length = value;
            Print($"Set Timer Length: {length}", debug, this);
        }
    }
    public void SetLength(float length) { Length = length; }
    [SerializeField] public bool playOnStart = false;
    [SerializeField] private bool interruptOnPlay = false;
    [SerializeField] private bool debug = false;

    public UnityEvent onPlay;
    public UnityEvent onComplete;

    private Coroutine running;

    private void Start()
    {
        if (playOnStart) Play();
    }

    public void Play()
    {
        if (interruptOnPlay) Interrupt();
        if (running == null)
        {
            onPlay.Invoke();
            StartCoroutine(RunTimer());
            Print("Play", debug, this);
        }
    }

    public void Interrupt()
    {
        if (running != null)
        {
            StopCoroutine(running);
            running = null;
        }
    }

    public IEnumerator RunTimer()
    {
        yield return new WaitForSeconds(length);
        running = null;
        onComplete.Invoke();
    }
}
