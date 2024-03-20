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

    // Settings
    [Header("Settings")]
    public bool alwaysSpawnAll = false;
    public bool debug = false;

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
        }
    }

    // Spawn Parties (All vs NonDefeated)

    public void SpawnParties()
    {
        if (alwaysSpawnAll)
            SpawnAll();
        else
            SpawnNonDefeated();
    }

    private void SpawnNonDefeated()
    {
        Print("Spawn Only Non_Defeated Enemies.", debug);
        Scene scene = SceneManager.GetActiveScene();
        SceneProgress progress = story.GetProgress(scene.name);
        foreach (var party in parties)
        {
            if (party != Party.mainParty)
            {
                if (progress.groupsDefeated.Contains(party.Name))
                {
                    Print($"Despawning {party.Name}", debug);
                    foreach (var member in party.members)
                    {
                        member.autoRespawn = false;
                    }
                    party.Despawn();
                }
                else
                {
                    Print($"Spawning {party.Name}.", debug);
                    party.Respawn();
                }
            }
        }
    }

    private void SpawnAll()
    {
        Print("Spawn All Enemies.");
        foreach(Party party in parties)
        {
            if (party != Party.mainParty)
            {
                Print($"Spawning {party.Name}.", debug);
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
