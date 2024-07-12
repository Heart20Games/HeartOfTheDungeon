using HotD;
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
    public Checkpoint initialSpawnpoint;
    public List<Checkpoint> checkpoints;
    public List<Party> parties;
    public List<DialogueTrigger> dialogueTriggers;

    // Settings
    [Header("Settings")]
    [Tooltip("Respawn all parties when respawning?")] public bool alwaysSpawnAll = false;
    [Tooltip("Reset all dialogue triggers when respawning?")] public bool alwaysResetDialogues = false;
    public bool debug = false;

    private void Awake()
    {
        // Checkpoints
        if (checkpoints == null || checkpoints.Count == 0)
        {
            checkpoints = new List<Checkpoint>(FindObjectsOfType<Checkpoint>());
            foreach (var checkpoint in checkpoints)
            {
                if (checkpoint.session == null || (session != null && checkpoint.session != session))
                    checkpoint.session = session;
                checkpoint.onActivate.AddListener(SaveSessionData);
            }
        }
        // Parties
        if (parties == null || parties.Count == 0)
        {
            parties = new List<Party>(FindObjectsOfType<Party>());
            foreach (var party in parties)
            {
                party.onAllDead.AddListener(() => { AddPartyDefeated(party.Name); });
            }
        }
        // Dialogue Triggers
        if (dialogueTriggers == null || dialogueTriggers.Count == 0)
        {
            dialogueTriggers = new List<DialogueTrigger>(FindObjectsOfType<DialogueTrigger>());
            foreach (var trigger in dialogueTriggers)
            {
                trigger.onDoneTalking.AddListener(() => { AddYarnNodeReached(trigger.targetNode); });
            }
        }
    }

    // Respawn Everything

    public void RespawnToLastCheckpoint()
    {
        ActivateDialogues();
        SpawnParties();

        if(Party.mainParty != null)
        {
            SpawnAtCheckpoint(Party.mainParty);
        }  
    }

    // Session Data Management

    public void SaveSessionData()
    {
        Assert.IsNotNull(session);
        Assert.IsNotNull(story);

        // Scene Progress
        story.TryGetProgress(session.name, out _, true);
        SceneProgress progress = story.sceneProgress[session.name];
        progress.checkpointsReached.Add(session.checkpoint);
        progress.partiesDefeated.UnionWith(session.unsavedPartiesDefeated);
        
        // Yarn Node Progress
        story.yarnNodesReached.UnionWith(session.unsavedYarnNodesReached);
        session.unsavedYarnNodesReached.Clear();
    }

    public void AddPartyDefeated(string partyName)
    {
        session.unsavedPartiesDefeated.Add(partyName);
    }

    public void AddYarnNodeReached(string yarnNode)
    {
        session.unsavedYarnNodesReached.Add(yarnNode);
    }

    // Reset Dialogues

    public void ActivateDialogues()
    {
        if (alwaysResetDialogues)
            ActivateAllDialogues();
        else
            ActivateSomeDialogues();
    }

    public void ActivateAllDialogues()
    {
        foreach (DialogueTrigger trigger in dialogueTriggers)
        {
            trigger.SetActive(true);
        }
    }

    public void ActivateSomeDialogues()
    {
        foreach (DialogueTrigger trigger in dialogueTriggers)
        {
            trigger.SetActive(IsYarnNodeReached(trigger.targetNode));
        }
    }

    public bool IsYarnNodeReached(string node)
    {
        return (
            session.unsavedYarnNodesReached.Contains(node) ||
            !story.yarnNodesReached.Contains(node)
        );
    }

    // Spawn Parties (All vs NonDefeated)

    public void SpawnParties()
    {
        if (alwaysSpawnAll)
            SpawnAll();
        else
            SpawnNonDefeated();
    }

    private void SpawnAll()
    {
        Print("Spawn All Enemies.", true, this);
        foreach (Party party in parties)
        {
            if (party != Party.mainParty)
            {
                Print($"Spawning {party.Name}.", debug);
                party.Respawn();
            }
        }
    }

    private void SpawnNonDefeated()
    {
        Print("Spawn Only Non_Defeated Enemies.", debug);
        foreach (var party in parties)
        {
            if (party != Party.mainParty)
            {
                if (IsPartyDefeated(party.Name))
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

    public bool IsPartyDefeated(string node)
    {
        return (
            session.unsavedPartiesDefeated.Contains(node) ||
            (story.GetProgress(session.scene).partiesDefeated?.Contains(node) == false)
        );
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
        if (initialSpawnpoint != null)
        {
            initialSpawnpoint.SpawnAtCheckpoint(party);
            initialSpawnpoint.Activate();
            return true;
        }
        return false;
    }
}
