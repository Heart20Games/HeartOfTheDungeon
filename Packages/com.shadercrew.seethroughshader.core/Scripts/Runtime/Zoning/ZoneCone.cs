using ShaderCrew.SeeThroughShader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneCone : ZoneData
{

    // why not in ZoneData?
    public ZoneCone(Transform transform)
    {
        this.transform = transform;
        //this.type = 3; //Cone
        this.type = (int)ZoneType.Cone;
    }

    public static ZoneCone CreateInstance(Transform transform)
    {
        ZoneCone data = ScriptableObject.CreateInstance<ZoneCone>();
        data.Init(transform);
        return data;
    }

    public override float[] ToShaderFloatArraySegment()
    {
        if (id == 0)
        {
            id = IdGenerator.Instance.Id;
        }
        //this.type = 3; //Cone
        this.type = (int)ZoneType.Cone;
        List<float> list = new List<float>();
        list.Add(id);
        list.Add(type); //instead array length?
        list.AddRange(computeShaderInformationCone());
        if (transitionData == null)
        {
            transitionData = new TransitionData(0, 0, 0, 0);
        }
        list.AddRange(transitionData.ToShaderFloatArraySegmentZone());
        //list.Add(layerMask.value);
        return list.ToArray();
    }


    private float[] computeShaderInformationCone()
    {
        Vector3 center = this.transform.position;
        Vector3 centerBottom = new Vector3(0f, -1f, 0f);
        Vector3 centerTop = new Vector3(0f, 1f, 0f);
        Vector3 cylinderBase = transform.TransformPoint(centerBottom);
        Vector3 cylinderTop = transform.TransformPoint(centerTop);
        float distanceBottomToTop = Vector3.Distance(cylinderBase, cylinderTop);
        Vector3 axisDirection = Vector3.Normalize(transform.TransformPoint(centerTop) - transform.TransformPoint(centerBottom));

        float radius = this.transform.lossyScale.x;
        float height = distanceBottomToTop;
        // height = this.transform.lossyScale.y;
        List<float> shaderData = new List<float>();
        shaderData.Add(center.x);
        shaderData.Add(center.y);
        shaderData.Add(center.z);
        shaderData.Add(axisDirection.x);
        shaderData.Add(axisDirection.y);
        shaderData.Add(axisDirection.z);
        shaderData.Add(radius);
        shaderData.Add(height);
        return shaderData.ToArray();

    }
}
