using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.VFX;


public class SkeletonMeshtoMesh : MonoBehaviour
{

    public MeshFilter skeletonMesh;
    public VisualEffect vfxGraph;
    public float refreshRate;
    public Transform meshTransform;
    public UnityEngine.Vector3 scale;
    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 rotation;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine (UpdateVFXGraph());
    }

    IEnumerator UpdateVFXGraph()
    {
        while(gameObject.activeSelf)
        {
            Mesh m = new Mesh();
            m = skeletonMesh.mesh;
            vfxGraph.SetMesh("Mesh", m);
            scale = meshTransform.localScale;
            position = meshTransform.localPosition;
            rotation = meshTransform.localEulerAngles;
            vfxGraph.SetVector3("Scale", scale);
            vfxGraph.SetVector3("Position", position);
            vfxGraph.SetVector3("Rotation", rotation);

            yield return new WaitForSeconds (refreshRate);
        }
    }
}
