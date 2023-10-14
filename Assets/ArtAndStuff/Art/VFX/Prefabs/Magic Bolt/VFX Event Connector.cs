using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXEventConnector : MonoBehaviour
{
    [SerializeField] private VFXEventController vfxEvent;

    private void Start() 
    {
        vfxEvent = GetComponentInParent<VFXEventController>();    
    }

    public void FirePoint1()
    {
        vfxEvent.VFXFirePoint1();
    }

    public void FirePoint2()
    {
        vfxEvent.VFXFirePoint2();
    }

    public void EndCast()
    {
        vfxEvent.EndCast();
    }
}
