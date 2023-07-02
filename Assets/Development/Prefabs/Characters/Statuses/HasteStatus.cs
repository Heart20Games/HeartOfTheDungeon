using System.Collections.Generic;
using UnityEngine;
using Body;

[CreateAssetMenu(fileName = "NewHasteStatus", menuName = "Statuses/HasteStatus", order = 1)]
public class HasteStatus : StatusEffect, ITimeScalable
{
    public float factor = 0.5f;
    private readonly Dictionary<Character, float> oldMovementTimeScales = new();
    private readonly Dictionary<Character, float> oldBrainTimeScales = new();

    public override void Apply(Character character, int strength)
    {
        base.Apply(character, strength);
        float newTimeScale = 1 + strength * factor;
        oldMovementTimeScales[character] = character.movement.TimeScale;
        oldBrainTimeScales[character] = character.brain.TimeScale;
        character.movement.TimeScale = newTimeScale;
        character.brain.TimeScale = newTimeScale;
    }

    public override void Remove(Character character)
    {
        base.Remove(character);
        character.movement.TimeScale = oldMovementTimeScales[character];
        character.brain.TimeScale = oldBrainTimeScales[character];
        oldMovementTimeScales.Remove(character);
        oldBrainTimeScales.Remove(character);
    }

    public float SetTimeScale(float timeScale)
    {
        AdjustTimeScaleDictionary(oldMovementTimeScales, timeScale);
        AdjustTimeScaleDictionary(oldBrainTimeScales, timeScale);
        return timeScale;
    }

    private void AdjustTimeScaleDictionary(Dictionary<Character, float> dictionary, float timeScale)
    {
        foreach(Character character in dictionary.Keys)
        {
            dictionary[character] = timeScale;
        }
    }
}
