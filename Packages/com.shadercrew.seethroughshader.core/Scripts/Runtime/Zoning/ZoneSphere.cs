using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{
    public class ZoneSphere : ZoneData
    {

        // why not in ZoneData?
        public ZoneSphere(Transform transform)
        {
            this.transform = transform;
            //this.type = 1; //Sphere
            this.type = (int)ZoneType.Sphere;

        }

        public static ZoneSphere CreateInstance(Transform transform)
        {
            ZoneSphere data = ScriptableObject.CreateInstance<ZoneSphere>();
            data.Init(transform);
            return data;
        }

        public override float[] ToShaderFloatArraySegment()
        {
            if (id == 0)
            {
                id = IdGenerator.Instance.Id;
            }
            this.type = 1; //Sphere
            List<float> list = new List<float>();
            list.Add(id);
            list.Add(type); //instead array length?
            list.AddRange(computeShaderInformationSphere());
            if (transitionData == null)
            {
                transitionData = new TransitionData(0, 0, 0, 0);
            }
            list.AddRange(transitionData.ToShaderFloatArraySegmentZone());
            //list.Add(layerMask.value);
            return list.ToArray();
        }

        private float[] computeShaderInformationSphere()
        {
            Vector3 center = this.transform.position;
            float radius = this.transform.lossyScale.x / 2;

            List<float> shaderData = new List<float>();
            shaderData.Add(center.x);
            shaderData.Add(center.y);
            shaderData.Add(center.z);
            shaderData.Add(radius);
            return shaderData.ToArray();

        }

    }
}