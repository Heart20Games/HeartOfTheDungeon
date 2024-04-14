using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPartySpawner
{
    public void RegisterPartyAdder(UnityAction<ASelectable> action);
    public void RegisterPartyRemover(UnityAction<ASelectable> action);
}
