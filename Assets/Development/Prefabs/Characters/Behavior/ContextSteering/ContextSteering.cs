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

        private void Awake()
        {
            controllers = FindObjectsOfType<CSController>();
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
                            MapOntoPeer(iCon, jCon);
                        if (iCon.Active)
                            MapOntoPeer(jCon, iCon);
                    }
                }
            }
        }

        public void MapOntoPeer(CSController aCon, CSController bCon)
        {
            Identity identity = bCon.RelativeIdentity(aCon.Identity);
            Vector3 sourceVector = aCon.transform.position - bCon.transform.position;
            Vector2 vector = sourceVector.XZVector();
            bCon.MapTo(vector, identity);
        }
    }
}
