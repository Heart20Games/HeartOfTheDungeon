using HotD.Body;
using MyBox;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using static HotD.CharacterModes;
using static HotD.CharacterModes.CharacterModifier;

[CreateAssetMenu(fileName = "NewModifierStatus", menuName = "Statuses/ModifierStatus", order = 1)]

public class ModifierStatus : StatusEffect
{
    public enum ApplyBy { Modifier, Name, Type }
    [SerializeField] private ApplyBy applyBy;

    [FormerlySerializedAs("modifier")]
    [ConditionalField(true, "ApplyByModifier")]
    [SerializeField] private CharacterModifier characterModifier;
    [ConditionalField(true, "ApplyByName")]
    [SerializeField] private string modifierName;
    [ConditionalField(true, "ApplyByType")]
    [SerializeField] private ModifierType modifierType;

    [SerializeField] private List<MoveModifier> moveModifiers = new();

    private bool ApplyByModifier() { return applyBy == ApplyBy.Modifier; }
    private bool ApplyByName() { return applyBy == ApplyBy.Name; }
    private bool ApplyByType() { return applyBy == ApplyBy.Type; }

    private void AddOrRemoveModifier(Character character, bool add)
    {
        Assert.IsNotNull(character);
        if (characterModifier.modifierType != ModifierType.None)
        {
            switch (applyBy)
            {
                case ApplyBy.Modifier: character.AddOrRemoveModifier(characterModifier, add); break;
                case ApplyBy.Name: character.AddOrRemoveModifier(modifierName, add); break;
                case ApplyBy.Type: character.AddOrRemoveModifier(modifierType, add); break;
            }
        }
        character.Movement?.AddOrRemoveModifiers(moveModifiers, add);
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

    protected override void OnValidate()
    {
        base.OnValidate();
        characterModifier.name = name;
        for (int i = 0; i < moveModifiers.Count; i++)
        {
            MoveModifier modifier = moveModifiers[i];
            string operation = modifier.type switch
            {
                MoveModifier.ModType.Override => "=",
                MoveModifier.ModType.Multiplicative => "x",
                MoveModifier.ModType.Additive => "+",
            };
            string value = modifier.IsBoolField() ? $"{modifier.boolValue}" : $"{modifier.floatValue}";
            modifier.name = $"{name}: {modifier.field} {operation} {value}";
            moveModifiers[i] = modifier;
        }
    }

    // Helper Buttons
}
