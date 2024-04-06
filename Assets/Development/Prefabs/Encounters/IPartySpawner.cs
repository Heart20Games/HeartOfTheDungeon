using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPartySpawner
{
    public void RegisterPartyReceiver(UnityAction<ASelectable> action);
}
