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
            for (int i = 0; i < controllers.Length - 1; i++)
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
            Identity identity = bCon.RelativeIdentity(aCon.Identity);
            Map map = bCon.GetMapOf(identity);
            if (map.valid)
            {
                Vector3 sourceVector = aCon.transform.position - bCon.transform.position;
                Vector2 vector = new(sourceVector.x, sourceVector.z);
                bCon.MapTo(vector, identity, map);
            }
        }
    }
}
