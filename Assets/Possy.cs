using Body;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Possy : BaseMonoBehaviour
{
    public Character leader;
    public List<Character> characters = new();
    public bool aggroed;
    public UnityEvent onAggro;
    public UnityEvent onAllDead;

    static Possy mainPossy;
    public bool isMainPossy = false;

    private void Awake()
    {
        if (isMainPossy)
            mainPossy = this;
        foreach (var character in characters)
        {
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

    public void CharacterDamaged()
    {
        if (!aggroed)
        {
            aggroed = true;
            onAggro.Invoke();
        }
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
