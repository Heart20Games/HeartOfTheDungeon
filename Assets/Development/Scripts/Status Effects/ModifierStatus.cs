using HotD.Body;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HotD.CharacterModes;
using static HotD.CharacterModes.CharacterModifier;

[CreateAssetMenu(fileName = "NewModifierStatus", menuName = "Statuses/ModifierStatus", order = 1)]

public class ModifierStatus : StatusEffect
{
    public enum ApplyBy { Modifier, Name, Type }
    [SerializeField] private ApplyBy applyBy;

    [ConditionalField(true, "ApplyByModifier")]
    [SerializeField] private CharacterModifier modifier;
    [ConditionalField(true, "ApplyByName")]
    [SerializeField] private string modifierName;
    [ConditionalField(true, "ApplyByType")]
    [SerializeField] private ModifierType modifierType;

    private bool ApplyByModifier() { return applyBy == ApplyBy.Modifier; }
    private bool ApplyByName() { return applyBy == ApplyBy.Name; }
    private bool ApplyByType() { return applyBy == ApplyBy.Type; }

    private void AddOrRemoveModifier(Character character, bool add)
    {
        switch (applyBy)
        {
            case ApplyBy.Modifier: character.AddOrRemoveModifier(modifier, add); break;
            case ApplyBy.Name: character.AddOrRemoveModifier(modifierName, add); break;
            case ApplyBy.Type: character.AddOrRemoveModifier(modifierType, add); break;
        }
    }

    public override void Apply(Character character, int strength)
    {
        base.Apply(character, strength);
        AddOrRemoveModifier(character, true);
    }

    public override void Remove(Character character)
    {
        base.Remove(character);
        AddOrRemoveModifier(character, false);
    }
}
