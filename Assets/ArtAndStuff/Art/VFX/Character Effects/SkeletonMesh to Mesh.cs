using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class SkeletonMeshtoMesh : MonoBehaviour
{

    public MeshFilter skeletonMesh;
    public VisualEffect vfxGraph;
    public float refreshRate;
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

            yield return new WaitForSeconds (refreshRate);
        }
    }
}
