using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Assertions;
using static CSContext;
using static CSMapping;

public class ContextSteering : MonoBehaviour
{
    [SerializeField] private CSController[] controllers;

    private void Awake()
    {
        controllers = FindObjectsOfType<CSController>();
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

    public void MapOntoPeer(CSController aCon, CSController bCon)
    {
        Map map = bCon.GetMapOf(aCon.Identity);
        if (map.valid)
        {
            print("Mapping");
            Vector3 sourceVector = aCon.transform.position - bCon.transform.position;
            Vector2 vector = new(sourceVector.x, sourceVector.z);
            bCon.MapTo(vector, ContextType.Peer, map);
        }
    }
}
