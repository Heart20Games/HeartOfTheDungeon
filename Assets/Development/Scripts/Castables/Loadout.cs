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

        public List<CastableItem> All()
        {
            allItems ??= new();
            allItems.Clear();
            foreach(var item in weapons)
            {
                allItems.Add(item);
            }
            foreach(var item in abilities)
            {
                allItems.Add(item);
            }
            if (mobility != null)
                allItems.Add(mobility);
            return allItems;
        }
    }
}