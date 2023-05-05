using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Assertions;
using static ContextSteeringController;

public class ContextSteering : MonoBehaviour
{
    [SerializeField] private ContextSteeringController[] controllers;
    private Vector2[] positions;

    private void Awake()
    {
        controllers = FindObjectsOfType<ContextSteeringController>();
        foreach (var controller in controllers)
        {
            controller.Initialize();
        }
        positions = new Vector2[controllers.Length];
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < controllers.Length-1; i++)
        {
            for (int j = i + 1; j < controllers.Length; j++)
            {
                var iCon = controllers[i];
                var jCon = controllers[j];

                Assert.AreNotEqual(iCon, jCon);

                Vector3 aPosition = iCon.transform.position;
                Vector3 bPosition = jCon.transform.position;
                Vector2 iPos = new(aPosition.x, aPosition.z);
                Vector2 jPos = new(bPosition.x, bPosition.z);

                MapOntoPeer(iCon, jCon, iPos, jPos);
                MapOntoPeer(jCon, iCon, jPos, iPos);
            }
        }
    }

    public void MapOntoPeer(ContextSteeringController aCon, ContextSteeringController bCon, Vector2 aPos, Vector2 bPos)
    {
        MapType mapType = bCon.GetMapOf(aCon.identity);
        if (mapType != MapType.None)
        {
            bCon.MapTo(bPos - aPos, mapType, ContextType.Peer);
        }
    }
}
