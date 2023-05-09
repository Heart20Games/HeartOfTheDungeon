using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Assertions;
using static ContextSteeringStructs;

public class ContextSteering : MonoBehaviour
{
    [SerializeField] private ContextSteeringController[] controllers;

    private void Awake()
    {
        controllers = FindObjectsOfType<ContextSteeringController>();
        foreach (var controller in controllers)
        {
            controller.Initialize();
        }
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

                MapOntoPeer(iCon, jCon);
                MapOntoPeer(jCon, iCon);
            }
        }
    }

    public void MapOntoPeer(ContextSteeringController aCon, ContextSteeringController bCon)
    {
        Map map = bCon.GetMapOf(aCon.identity);
        if (map.valid)
        {
            Vector3 sourceVector = aCon.transform.position - bCon.transform.position;
            Vector2 vector = new(sourceVector.x, sourceVector.z);
            bCon.MapTo(vector, ContextType.Peer, map);
        }
    }
}
