using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    [CreateAssetMenu(fileName = "NewLoadout", menuName = "Loadouts/Loadout", order = 1)]
    public class Loadout : ScriptableObject
    {
        public enum Slot { Weapon1, Weapon2, Ability1, Ability2, Mobility, None }

        public List<CastableItem> weapons;
        public List<CastableItem> abilities;

        public CastableItem mobility;

        private List<CastableItem> allItems;

        private bool changed = false;

        public CastableItem this[int idx]
        {
            get => GetSlot((Slot)idx);
            set => SetSlot((Slot)idx, value);
        }

        public CastableItem GetSlot(Slot slot)
        {
            return slot switch
            {
                Slot.Weapon1 => GetIndexOn(0, weapons ??= new()),
                Slot.Weapon2 => GetIndexOn(1, weapons ??= new()),
                Slot.Ability1 => GetIndexOn(0, abilities ??= new()),
                Slot.Ability2 => GetIndexOn(1, abilities ??= new()),
                Slot.Mobility => mobility,
                _ => null,
            };
        }

        public void SetSlot(Slot slot, CastableItem item)
        {
            switch (slot)
            {
                case Slot.Weapon1: SetIndexOn(0, item, weapons ??= new()); break;
                case Slot.Weapon2: SetIndexOn(1, item, weapons ??= new()); break;
                case Slot.Ability1: SetIndexOn(0, item, abilities ??= new()); break;
                case Slot.Ability2: SetIndexOn(1, item, abilities ??= new()); break;
                case Slot.Mobility: mobility = item; break;
            }
        }

        private CastableItem GetIndexOn(int index, List<CastableItem> items)
        {
            return (items.Count >= index) ? items[index] : null;
        }

        private void SetIndexOn(int index, CastableItem item, List<CastableItem> items)
        {
            for (int i = items.Count - index - 1; i < 0; i++)
            {
                items.Add(null);
            }
            items[index] = item;
            changed = true;
        }

        public List<CastableItem> All()
        {
            allItems ??= new();
            if (changed || allItems.Count == 0)
            {
                allItems.Clear();
                weapons ??= new();
                foreach(var item in weapons)
                {
                    allItems.Add(item);
                }
                abilities ??= new();
                foreach(var item in abilities)
                {
                    allItems.Add(item);
                }
                if (mobility != null)
                    allItems.Add(mobility);
            }
            return allItems;
        }
    }
}