using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface ISelectable
{
    /* ISelectable should...
     * 1. Handle what happens when selected.
     * 2. Handle what happens when deselected.
     * 3. Handle what happens when confirmed.
     */

    public void Select();
    public void DeSelect();
    public void Hover();
    public void UnHover();
    public void Confirm();
}
