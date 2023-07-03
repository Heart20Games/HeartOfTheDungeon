using Body;
using Body.Behavior;
using Body.Behavior.ContextSteering;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Possy : BaseMonoBehaviour
{
    [Header("Members")]
    public Character leader;
    public List<Character> characters = new();
    public List<Character> pets = new();

    static Possy mainPossy;
    public bool isMainPossy = false;

    [Header("Events")]
    public bool aggroed = false;
    public UnityEvent onAggro;
    public UnityEvent onAllDead;

    [Header("Follow Target")]
    public Transform followTarget;
    public bool useLeaderAsFollowTarget;

    [Header("Noise and Scaling")]
    public MovementNoise noise;
    public float destinationScale = 1f;

    public bool debug = false;


    private void Start()
    {
        if (useLeaderAsFollowTarget)
            followTarget = leader.body.transform;
        if (isMainPossy)
            mainPossy = this;
        foreach (var character in characters)
        {
            if (followTarget)
                character.brain.Target = followTarget;
            if (noise != null)
            {
                character.Controller.noise = noise;
                character.Controller.destinationScale = destinationScale;
            }
            character.Controller.onFoeContextActive.AddListener(CharacterAggroed);
            character.onDmg.AddListener(CharacterDamaged);
            character.onDeath.AddListener(CharacterDied);
            character.onControl.AddListener(CharacterControlled);
        }
    }

    public void CharacterControlled(bool controlled)
    {
        if (debug) print("Controlled");
        if (mainPossy && leader != null)
        {
            foreach (var character in characters)
            {
                if (character.controllable)
                    leader = character;
            }
        }
    }

    public void CharacterAggroed()
    {
        if (debug) print("Aggroed");
        if (!aggroed)
        {
            if (debug) print("Aggro!");
            aggroed = true;
            foreach (var character in characters)
            {
                character.Controller.destinationScale = 1f;
                character.Controller.useNoise = false;
            }
            if (mainPossy != this && followTarget.TryGetComponent(out Brain brain))
            {
                brain.Target = mainPossy.followTarget;
            }
            onAggro.Invoke();
        }
    }

    public void CharacterDamaged()
    {
        if (debug) print("Damaged");
        CharacterAggroed();
    }

    public void CharacterDied()
    {
        if (debug) print("Died");
        foreach (var character in characters)
        {
            if (character.alive) return;
        }
        onAllDead.Invoke();
    }
}
