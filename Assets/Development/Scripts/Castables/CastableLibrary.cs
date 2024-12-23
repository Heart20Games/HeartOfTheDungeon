using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    [CreateAssetMenu(fileName = "CastableLibrary", menuName = "Loadouts/CastableLibrary", order = 1)]
    public class CastableLibrary : BaseScriptableObject
    {
        public List<Loadout> loadouts = new();
        public List<CastableItem> castableItems = new();

        public override void Init()
        {
            loadouts.Clear();
            castableItems.Clear();
            loadouts.AddRange((Loadout[])Resources.FindObjectsOfTypeAll(typeof(Loadout)));
            castableItems.AddRange((CastableItem[])Resources.FindObjectsOfTypeAll(typeof(CastableItem)));
        }

        public Loadout GetLoadout(string name)
        {
            return loadouts.Find((Loadout data) => { return data.name == name; });
        }

        public CastableItem GetCastableItem(string name)
        {
            return castableItems.Find((CastableItem data) => { return data.name == name; });
        }
    }
}