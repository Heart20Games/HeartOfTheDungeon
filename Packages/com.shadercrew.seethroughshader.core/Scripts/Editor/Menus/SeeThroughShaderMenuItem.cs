﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ShaderCrew.SeeThroughShader
{
    public class SeeThroughShaderMenuItem : MonoBehaviour
    {

        //// In development
        //[MenuItem("GameObject/See-through Shader/Manager")]
        //static void CreateSeeThroughShaderManger()
        //{
        //    string seeThroughManagerName = "See-through Shader Manager";
        //    if (GameObject.Find(seeThroughManagerName) == null)
        //    {
        //        GameObject seeThroughShaderManager = new GameObject(seeThroughManagerName);
        //        seeThroughShaderManager.AddComponent<SeeThroughShaderManager>();

        //        PlayersPositionManager[] playersPositionManagers = (PlayersPositionManager[])GameObject.FindObjectsOfType(typeof(PlayersPositionManager));
        //        if (playersPositionManagers.Length > 0)
        //        {
        //            if (playersPositionManagers.Length > 1)
        //            {
        //                string gameObjectNames = "(";
        //                foreach (PlayersPositionManager playersPositionManager in playersPositionManagers)
        //                {
        //                    gameObjectNames += " " + playersPositionManager.name + ", ";
        //                }
        //                gameObjectNames = gameObjectNames.Remove(gameObjectNames.Length - 2);
        //                gameObjectNames += ")";
        //                Debug.LogWarning("There are multiple GameObjects with the script PlayerPositionManager attached to itself." +
        //                    " Only one instance of this script is allowed, otherwise the 'See-through Shader' asset won't work. " +
        //                    "List of GameObjects using the script: " + gameObjectNames);
        //            }
        //            else
        //            {
        //                Debug.Log("PlayerPositionManager is already attached to GameObject " + playersPositionManagers[0].gameObject.name);
        //            }

        //        }
        //        else
        //        {
        //            seeThroughShaderManager.AddComponent<PlayersPositionManager>();
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogWarning("There is already a See-through Shader Manager present in the scene! You can't add another one!");
        //    }

        //}


        [MenuItem("Assets/Create/See-through Shader/Create Reference Material")]
        static void CreateReferenceMaterial()
        {
            ReferenceMaterialCreator window = ScriptableObject.CreateInstance<ReferenceMaterialCreator>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);
            //window.ShowPopup();
            window.titleContent = new GUIContent("See-through Shader Tools");
            window.ShowUtility();

        }

        [MenuItem("GameObject/See-through Shader/Zoning/Zone Box", false, 12)]
        static void CreateZoneBox()
        {
            //Debug.Log("Create Zone Box");
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            cube.transform.position = GetZoneSpawningTransform();

            cube.name = "ZoneBox";

            cube.GetComponent<BoxCollider>().isTrigger = true;
            DestroyImmediate(cube.GetComponent<MeshRenderer>());

            cube.AddComponent<SeeThroughShaderZone>();

            cube.GetComponent<SeeThroughShaderZone>().type = ZoneType.Box;
        }

        [MenuItem("GameObject/See-through Shader/Zoning/Zone Sphere", false, 12)]
        static void CreateZoneSphere()
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            sphere.transform.position = GetZoneSpawningTransform();

            sphere.name = "ZoneSphere";

            sphere.GetComponent<SphereCollider>().isTrigger = true;
            DestroyImmediate(sphere.GetComponent<MeshRenderer>());

            sphere.AddComponent<SeeThroughShaderZone>();

            sphere.GetComponent<SeeThroughShaderZone>().type = ZoneType.Sphere;
        }


        [MenuItem("GameObject/See-through Shader/Zoning/Zone Cylinder", false, 12)]
        static void CreateZoneCylinder()
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            cylinder.transform.position = GetZoneSpawningTransform();

            cylinder.name = "ZoneCylinder";

            cylinder.GetComponent<CapsuleCollider>().isTrigger = true;
            DestroyImmediate(cylinder.GetComponent<MeshRenderer>());

            cylinder.AddComponent<SeeThroughShaderZone>();

            cylinder.GetComponent<SeeThroughShaderZone>().type = ZoneType.Cylinder;
        }


        [MenuItem("GameObject/See-through Shader/Zoning/Zone Cone", false, 12)]
        static void CreateZoneCone()
        {
            GameObject cone = new GameObject();

            cone.transform.position = GetZoneSpawningTransform();

            cone.name = "ZoneCone";

            Mesh coneMesh = Resources.Load("ZoneConeMesh") as Mesh;

            MeshCollider collider = cone.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.isTrigger = true;
            collider.sharedMesh = coneMesh; 

            SeeThroughShaderZone seeThroughShaderZone = cone.AddComponent<SeeThroughShaderZone>();
            seeThroughShaderZone.type = ZoneType.Cone;
        }

        [MenuItem("GameObject/See-through Shader/Zoning/Zone Plane", false, 12)]
        static void CreateZonePlane()
        {
            //Debug.Log("Create Zone Box");
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

            plane.transform.position = GetZoneSpawningTransform();

            plane.name = "ZonePlane";

            MeshCollider collider = plane.GetComponent<MeshCollider>();
            collider.convex = true;
            collider.isTrigger = true;
            DestroyImmediate(plane.GetComponent<MeshRenderer>());

            plane.AddComponent<SeeThroughShaderZone>();

            plane.GetComponent<SeeThroughShaderZone>().type = ZoneType.Plane;
        }

        [MenuItem("GameObject/See-through Shader/Zoning/Zones Group", false, 12)]
        static void CreateZonesGroup()
        {
            GameObject zonesGroup = new GameObject("ZonesGroup");
            zonesGroup.AddComponent<SeeThroughShaderZonesGroup>();
        }


        private static Vector3 GetZoneSpawningTransform()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            Vector3 CenterPos;
            if (sceneView != null)
            {
                Camera sceneViewCam = SceneView.lastActiveSceneView.camera;
                Vector3 offset = SceneView.lastActiveSceneView.pivot - SceneView.lastActiveSceneView.camera.transform.position;
                float cameraDistance = offset.magnitude;
                CenterPos = sceneViewCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cameraDistance));
            }
            else if (Camera.main != null)
            {
                //float cameraDistance = Camera.main.nearClipPlane;
                CenterPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            }
            else
            {
                CenterPos = new Vector3(0, 0, 0);
                Debug.Log("SceneView and Camera.main is null. Zone got spawned at (0,0,0)");
            }
            return CenterPos;
        }
    }



}