using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class ProgressManager : BaseMonoBehaviour
{


    // Data
    [Header("Data")]
    public Session session;
    public Story story;

    // Objects
    [Header("Objects")]
    public List<Checkpoint> checkpoints;
    public List<Party> parties;
    public Dictionary<string, Party> partyBank;

    // Settings
    [Header("Settings")]
    public bool alwaysSpawnAll = false;

    private void Awake()
    {
        if (checkpoints == null || checkpoints.Count == 0)
        {
            checkpoints = new List<Checkpoint>(FindObjectsOfType<Checkpoint>());
            foreach (var checkpoint in checkpoints)
            {
                if (checkpoint.session == null || (session != null && checkpoint.session != session))
                    checkpoint.session = session;
            }
        }
        if (parties == null || parties.Count == 0)
        {
            parties = new List<Party>(FindObjectsOfType<Party>());
            partyBank = new();
            foreach (Party party in parties)
            {
                partyBank.Add(party.name, party);
            }
        }
    }

    public void SpawnParties()
    {
        if (alwaysSpawnAll)
            SpawnAll();
        else
            SpawnNonDefeated();
    }

    public void SpawnNonDefeated()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneProgress progress = story.GetProgress(scene.name);
        foreach (var party in parties)
        {
            if (party != Party.mainParty)
            {
                if (progress.groupsDefeated.Contains(party.name))
                {
                    foreach (var member in party.members)
                    {
                        member.autoRespawn = false;
                    }
                    party.Despawn();
                }
                else
                {
                    party.Respawn();
                }
            }
        }
    }

    public void SpawnAll()
    {
        foreach(Party party in parties)
        {
            if (party != Party.mainParty)
            {
                party.Respawn();
            }
        }
    }

    /// <summary>Spawns the given party at the checkpoint found on the current session.</summary>
    /// <returns>Returns true if a valid checkpoint was found.</returns>
    public bool SpawnAtCheckpoint(Party party)
    {
        Assert.IsNotNull(session, "Trying to spawn party at session checkpoint, but session is null.");
        return SpawnAtCheckpoint(party, session.checkpoint);
    }

    /// <summary>Spawns the given party at the given checkpoint.</summary>
    /// <returns>Returns true if a valid checkpoint was found.</returns>
    public bool SpawnAtCheckpoint(Party party, string checkpointName)
    {
        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint.Name == checkpointName)
            {
                checkpoint.SpawnAtCheckpoint(party);
                return true;
            }
        }
        return false;
    }
}
