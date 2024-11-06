using ShaderCrew.SeeThroughShader;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ZoneRainGenerator : MonoBehaviour
{

    private float lastSpawnTime = 0;
    public float spawningInterval = 1;

    public Material mat;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup - lastSpawnTime > spawningInterval)
        {
            float x = Random.Range(this.transform.position.x - this.transform.lossyScale.x / 2, this.transform.position.x + this.transform.lossyScale.x / 2);

            float z = Random.Range(this.transform.position.z - this.transform.lossyScale.z / 2, this.transform.position.z + this.transform.lossyScale.z / 2);

            float y = this.transform.position.y + this.transform.lossyScale.y / 2;

            GameObject spawnedZone;
            MeshRenderer meshRenderer;
            switch (Random.Range(0,4))
            {
                case 0:
                    spawnedZone = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    spawnedZone.AddComponent<SeeThroughShaderZone>();
                    spawnedZone.GetComponent<SeeThroughShaderZone>().type = ZoneType.Box;
                    break;
                case 1:
                    spawnedZone = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                    spawnedZone.AddComponent<SeeThroughShaderZone>();
                    spawnedZone.GetComponent<SeeThroughShaderZone>().type = ZoneType.Sphere;
                    break;
                case 2:
                    spawnedZone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                    spawnedZone.AddComponent<SeeThroughShaderZone>();
                    spawnedZone.GetComponent<SeeThroughShaderZone>().type = ZoneType.Cylinder;
                    break;
                case 3:
                    spawnedZone = new GameObject();
                    Mesh coneMesh = Resources.Load("ZoneConeMesh") as Mesh;
                    MeshFilter meshFilter = spawnedZone.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = coneMesh;
                    spawnedZone.AddComponent<MeshRenderer>();
                    spawnedZone.AddComponent<SeeThroughShaderZone>();
                    spawnedZone.GetComponent<SeeThroughShaderZone>().type = ZoneType.Cone;
                    break;
                default:
                    spawnedZone = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    spawnedZone.AddComponent<SeeThroughShaderZone>();
                    spawnedZone.GetComponent<SeeThroughShaderZone>().type = ZoneType.Box;
                    break;
            }

            meshRenderer = spawnedZone.GetComponent<MeshRenderer>();
            meshRenderer.material = mat;
            spawnedZone.transform.position = new Vector3(x,y,z);
            spawnedZone.transform.localScale = Vector3.one * 3;
            DestroyImmediate(spawnedZone.GetComponent<Collider>());
            RandomSpinAndFall spinAndDestroyAtY = spawnedZone.AddComponent<RandomSpinAndFall>();
            spinAndDestroyAtY.destructionY = this.transform.position.y - this.transform.lossyScale.y / 2;


            lastSpawnTime = Time.realtimeSinceStartup;
        }
    }


}
