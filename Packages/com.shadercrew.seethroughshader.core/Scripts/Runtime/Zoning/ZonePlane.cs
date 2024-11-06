using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{
    public class ZonePlane : ZoneData
    {

        // why not in ZoneData?
        public ZonePlane(Transform transform)
        {
            this.transform = transform;
            this.type = (int)ZoneType.Plane;
        }

        public static ZonePlane CreateInstance(Transform transform)
        {
            ZonePlane data = ScriptableObject.CreateInstance<ZonePlane>();
            data.Init(transform);
            return data;
        }

        public override float[] ToShaderFloatArraySegment()
        {
            if (id == 0)
            {
                id = IdGenerator.Instance.Id;
            }
            this.type = (int)ZoneType.Plane;
            List<float> list = new List<float>();
            list.Add(id);
            list.Add(type); //instead array length?
            list.AddRange(computeShaderInformationPlane());
            if (transitionData == null)
            {
                transitionData = new TransitionData(0, 0, 0, 0);
            }
            list.AddRange(transitionData.ToShaderFloatArraySegmentZone());
            //list.Add(layerMask.value);
            return list.ToArray();
        }

        private float[] computeShaderInformationPlane()
        {
            Plane plane = new Plane(transform.up, transform.position);

            Vector3 normal = new Vector3(plane.normal.x, plane.normal.y, plane.normal.z);
            float distanceToOrigin = plane.distance;

            List<float> shaderData = new List<float>();
            shaderData.Add(normal.x);
            shaderData.Add(normal.y);
            shaderData.Add(normal.z);
            shaderData.Add(distanceToOrigin);
            return shaderData.ToArray();

        }

    }
}