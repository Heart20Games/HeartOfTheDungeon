using System.Collections.Generic;
using UnityEngine;
using Body;
using HotD.Body;

[CreateAssetMenu(fileName = "NewHasteStatus", menuName = "Statuses/HasteStatus", order = 1)]
public class HasteStatus : StatusEffect, ITimeScalable
{
    public float factor = 0.5f;
    private readonly Dictionary<Character, float> oldMovementTimeScales = new();
    private readonly Dictionary<Character, float> oldBrainTimeScales = new();

    public override void Apply(Character character, int strength)
    {
        if (character != null)
        {
            base.Apply(character, strength);
            float newTimeScale = 1 + strength * factor;
            oldMovementTimeScales[character] = character.movement.TimeScale;
            oldBrainTimeScales[character] = character.brain.TimeScale;
            character.movement.TimeScale = newTimeScale;
            character.brain.TimeScale = newTimeScale;
        }
        else Debug.LogWarning($"Tried to apply {name} haste status effect to Character \"{character}\".");
    }

    public override void Remove(Character character)
    {
        base.Remove(character);
        if (oldMovementTimeScales.TryGetValue(character, out var timeScale))
        {
            character.movement.TimeScale = timeScale;
            oldMovementTimeScales.Remove(character);
        }
        if (oldBrainTimeScales.TryGetValue(character, out timeScale))
        {
            character.brain.TimeScale = timeScale;
            oldBrainTimeScales.Remove(character);
        }
    }

    public float TimeScale { get => lastTimeScale; set => SetTimeScale(value); }
    private float lastTimeScale;
    public float SetTimeScale(float timeScale)
    {
        lastTimeScale = timeScale;
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
