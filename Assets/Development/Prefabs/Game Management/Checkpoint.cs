using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : BaseMonoBehaviour
{
    [Header("Checkpoint")]
    public bool useGameObjectName = true;
    [ConditionalField("useGameObjectName", true)]
    public new string name = "[New Checkpoint]";

    [Header("Party")]
    public Party targetParty;

    [Foldout("Events", true)]
    public UnityEvent onInteract;
    [Foldout("Events")]
    public UnityEvent onSpawn;

    public string Name
    {
        get { return useGameObjectName ? gameObject.name : name; }
        set
        {
            name = value;
            if (useGameObjectName)
                gameObject.name = value;
        }
    }

    [ButtonMethod]
    public void Interact()
    {
        onInteract.Invoke();
    }

    [ButtonMethod]
    public void SpawnAtCheckpoint()
    {
        SpawnAtCheckpoint(targetParty);
    }

    public void SpawnAtCheckpoint(Party party)
    {
        party.transform.position = transform.position;
        party.Respawn();
        targetParty = party;
    }
}
