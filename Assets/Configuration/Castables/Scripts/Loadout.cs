using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLoadout", menuName = "Loadouts/Loadout", order = 1)]
public class Loadout : ScriptableObject
{
    public enum Slot { None, Weapon1, Weapon2, Ability1, Ability2, Mobility }

    public List<CastableItem> abilities;
    public List<CastableItem> weapons;

    public CastableItem mobility;

    public void SetSlot(Slot slot, CastableItem item)
    {
        switch (slot)
        {
            case Slot.Weapon1: SetIndexOn(0, item, weapons); break;
            case Slot.Weapon2: SetIndexOn(1, item, weapons); break;
            case Slot.Ability1: SetIndexOn(0, item, abilities); break;
            case Slot.Ability2: SetIndexOn(1, item, abilities); break;
            case Slot.Mobility: mobility = item; break;
        }
    }

    private void SetIndexOn(int index, CastableItem item, List<CastableItem> items)
    {
        for (int i = items.Count - index - 1; i < 0; i++)
        {
            items.Add(null);
        }
        items[index] = item;
    }
}