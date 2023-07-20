using System.Collections.Generic;
using UnityEngine;

public class Targeter : Selector, ILooker
{
    public float range = 10f;
    [ReadOnly][SerializeField] private List<ASelectable> selectableBank = new();

    private void Awake()
    {
        // Initialize full list of selectables here.
    }

    public void SwitchTargets(bool left)
    {
        // Set selected to the next nearest selectable in the given direction.
    }
}
