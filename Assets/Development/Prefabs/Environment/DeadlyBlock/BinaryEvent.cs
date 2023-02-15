using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct BinaryEvent
{
    public BinaryEvent(UnityEvent _enter, UnityEvent _exit)
    {
        enter = _enter;
        exit = _exit;
    }
    public UnityEvent enter;
    public UnityEvent exit;
}
