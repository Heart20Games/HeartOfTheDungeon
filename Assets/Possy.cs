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
    public bool aggroed;
    public UnityEvent onAggro;
    public UnityEvent onAllDead;

    [Header("Follow Target")]
    public Transform followTarget;
    public bool useLeaderAsFollowTarget;

    [Header("Noise and Scaling")]
    public MovementNoise noise;
    public float destinationScale = 1f;


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
            character.onDmg.AddListener(CharacterDied);
            character.onDeath.AddListener(CharacterDied);
            character.onControl.AddListener(CharacterControlled);
        }
    }

    public void CharacterControlled(bool controlled)
    {
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
        if (!aggroed)
        {
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
        CharacterAggroed();
    }

    public void CharacterDied()
    {
        foreach (var character in characters)
        {
            if (character.alive) return;
        }
        onAllDead.Invoke();
    }
}
