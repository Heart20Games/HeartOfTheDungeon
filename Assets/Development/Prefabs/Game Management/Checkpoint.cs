using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : BaseMonoBehaviour
{
    [Header("Checkpoint")]
    public bool useGameObjectName = true;
    [ConditionalField("useGameObjectName", true)]
    public new string name = "[New Checkpoint]";

    [Header("Party")]
    public Party targetParty;

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
    public void SpawnAtCheckpoint()
    {
        SpawnAtCheckpoint(targetParty);
    }

    public void SpawnAtCheckpoint(Party party)
    {
        party.transform.position = transform.position;
        party.Respawn();
    }
}
