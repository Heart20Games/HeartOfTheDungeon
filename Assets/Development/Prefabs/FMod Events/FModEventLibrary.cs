using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FModEventLibary", menuName = "ScriptableObjects/FModEventLibrary", order = 1)]
public class FModEventLibary : ScriptableObject
{
    [Serializable]
    public struct FModEvent
    {
        public FModEvent(string _name, EventReference _reference)
        {
            name = _name;
            reference = _reference;
        }
        public string name;
        public EventReference reference;
    }

    public List<FModEvent> events = new List<FModEvent>();
    private Dictionary<string, FModEvent> bank = new Dictionary<string, FModEvent>();
    private bool initialized = false;

    public void Initialize()
    {
        string names = "";
        bank.Clear();
        foreach (FModEvent fmodEvent in events)
        {
            names += "\n" + fmodEvent.name;
            bank[fmodEvent.name] = fmodEvent;
        }
        //Debug.Log(names);
    }

    public EventReference GetReference(string key)
    {
        if (!initialized)
        {
            Initialize();
        }
        return bank[key].reference;
    }
}