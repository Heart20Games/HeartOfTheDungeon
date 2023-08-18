using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScriptableObject : ScriptableObject
{
    [HideInInspector] public bool initialized = false;
    public virtual void Init() { initialized = true; }

    #if !UNITY_EDITOR
        private void Awake() => Init();
    #endif
}
