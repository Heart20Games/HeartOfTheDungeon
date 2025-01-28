using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Body.Behavior.ContextSteering
{
    using static CSIdentity;
    using static CSContext;
    using static CSMapping;

    public class CSHive : BaseMonoBehaviour
    {
        [SerializeField] private CSController[] controllers;
        private readonly List<Transform> transforms = new();

        private void Awake()
        {
            controllers = FindObjectsOfType<CSController>();
            foreach (var controller in controllers)
            {
                transforms.Add(controller.transform);
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                controllers[i].activeContexts.Clear();
            }
            for (int i = 0; i < controllers.Length - 1; i++)
            {
                for (int j = i + 1; j < controllers.Length; j++)
                {
                    var iCon = controllers[i];
                    var jCon = controllers[j];

                    if (iCon.Alive && jCon.Alive)
                    {
                        Assert.AreNotEqual(iCon, jCon);

                        if (jCon.Active)
                            MapOntoPeer(iCon, jCon, transforms[i], transforms[j]);
                        if (iCon.Active)
                            MapOntoPeer(jCon, iCon, transforms[j], transforms[i]);
                    }
                }
            }
        }

        public void MapOntoPeer(CSController aCon, CSController bCon, Transform aTransform = null, Transform bTransform = null)
        {
            Identity identity = RelativeIdentity(aCon.identity, bCon.identity);
            Vector3 sourceVector = aTransform.position - bTransform.position; // aCon.transform.position - bCon.transform.position;
            Vector2 vector = sourceVector.XZVector2();
            bCon.MapTo(vector, identity);
        }
    }
}
