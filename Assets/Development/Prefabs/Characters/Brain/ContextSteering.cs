using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ContextSteeringController;

public class ContextSteering : MonoBehaviour
{
    [SerializeField] private List<ContextSteeringController> controllers;

    private void Awake()
    {
        controllers = new(FindObjectsOfType<ContextSteeringController>());
        foreach (var controller in controllers)
        {
            controller.Initialize();
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < controllers.Count-1; i++)
        {
            for (int j = i + 1; j < controllers.Count; j++)
            {
                var iCon = controllers[i];
                var jCon = controllers[j];
                
                MapOntoPeer(iCon, jCon);
                MapOntoPeer(jCon, iCon);
            }
        }
    }

    public void MapOntoPeer(ContextSteeringController aCon, ContextSteeringController bCon)
    {
        Vector3 aPosition = aCon.transform.position;
        Vector3 bPosition = bCon.transform.position;
        Vector2 aPos = new(aPosition.x, aPosition.z);
        Vector2 bPos = new(bPosition.x, bPosition.z);

        MapType mapType = bCon.GetMapOf(aCon.identity);
        if (mapType != MapType.None)
        {
            bCon.MapTo(bPos - aPos, mapType, ContextType.Peer);
        }
    }
}
