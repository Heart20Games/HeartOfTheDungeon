using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// event system by JCPereira 
namespace ShaderCrew.SeeThroughShader
{
    [AddComponentMenu("")]
    public class SeeThroughShaderZone : MonoBehaviour
    {

        public ZoneData zoneData;

        public TransformContainer tempTransform;

        private ZoneDataStorage zoneDataStorage = ZoneDataStorage.Instance;

        public ZoneType type;

        public int numOfPlayersInside = 0;
        public float transitionDuration = 2;

        bool isInitialized = false;

        public bool isActivated = false;

        //public LayerMask layerMask;


        private List<SeeThroughShaderPlayer> playersInside = new List<SeeThroughShaderPlayer>();

        public bool showZoneOnlyWhenSelected = false;



#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!showZoneOnlyWhenSelected || Selection.Contains(this.gameObject))
            {

                //if(isInitialized)
                //{
                if (type == ZoneType.Box)
                {
                    Gizmos.color = new Color(0, 1, 0, 0.2f);
                    Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Cube.fbx"), transform.position, transform.rotation, transform.lossyScale);
                    //Gizmos.DrawCube(transform.position, transform.lossyScale);
                }
                else if (type == ZoneType.Sphere)
                {
                    Gizmos.color = new Color(0, 1, 0, 0.2f);
                    Gizmos.DrawSphere(transform.position, transform.lossyScale.x / 2);
                }
                else if (type == ZoneType.Cylinder)
                {
                    Gizmos.color = new Color(0, 1, 0, 0.2f);
                    Vector3 adjustedScale = new Vector3(transform.lossyScale.x / 2, transform.lossyScale.y, transform.lossyScale.x / 2);
                    Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Cylinder.fbx"), transform.position, transform.rotation, adjustedScale);
                }
                else if (type == ZoneType.Cone)
                {
                    Gizmos.color = new Color(0, 1, 0, 0.2f);
                    Mesh coneMesh = Resources.Load("ZoneConeMesh") as Mesh;
                    Vector3 adjustedScale = new Vector3(transform.lossyScale.x , transform.lossyScale.y, transform.lossyScale.x);
                    Gizmos.DrawMesh(coneMesh, transform.position, transform.rotation, adjustedScale);
                }
                else if (type == ZoneType.Plane)
                {
                    Gizmos.color = new Color(0, 1, 0, 0.2f); 
                    Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Plane.fbx"), transform.position, transform.rotation, transform.lossyScale*10);
                    Vector3 scale = transform.lossyScale * 10;
                    scale.z *= -1;

                    Gizmos.color = new Color(1, 0, 0, 0.2f);
                    Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Plane.fbx"), transform.position, transform.rotation, scale);

                }
                //}

            }

        }
#endif
        private void init()
        {
            if (!isInitialized)
            {

                if (type == ZoneType.Box)
                {
                    //zoneData = new ZoneBox(transform);
                    zoneData = ZoneBox.CreateInstance(transform);
                } else if (type == ZoneType.Sphere)
                {
                    zoneData = ZoneSphere.CreateInstance(transform);
                }
                else if (type == ZoneType.Cylinder)
                {
                    zoneData = ZoneCylinder.CreateInstance(transform);
                }
                else if (type == ZoneType.Cone)
                {
                    zoneData = ZoneCone.CreateInstance(transform);
                }
                else if (type == ZoneType.Plane)
                {
                    zoneData = ZonePlane.CreateInstance(transform);
                }
                zoneData.id = IdGenerator.Instance.Id;
                zoneData.zoneInstanceID = GetInstanceID();
                //zoneData.layerMask = layerMask;
                zoneDataStorage.AddOrUpdateZoneData(zoneData);
                isInitialized = true;

                playersInside = new List<SeeThroughShaderPlayer>();
            }

        }

        public void ForceInit()
        {
            init();
        }


        void Start()
        {
            init();
        }

        private void OnDisable()
        {
            if (isInitialized)
            {
                zoneDataStorage.RemoveZoneData(zoneData);
                zoneDataStorage.UpdateZonesDatasShaderFloatArray();

                ClearPlayersInside();
            }
        }

        private void OnEnable()
        {
            if (isInitialized)
            {
                zoneDataStorage.AddOrUpdateZoneData(zoneData);
                zoneDataStorage.UpdateZonesDatasShaderFloatArray();
            }
        }

        void Update()
        {
            if (zoneData != null && zoneData.UpdateNecessary())
            {
                zoneDataStorage.AddOrUpdateZoneData(zoneData);
                zoneDataStorage.UpdateZonesDatasShaderFloatArray();
            }

        }


        private void ClearPlayersInside()
        {
            foreach (SeeThroughShaderPlayer player in playersInside)
            {
                player.OnExitZone(this);
            }

            playersInside.Clear();
        }



        private void OnTransformParentChanged()
        {
            zoneData.id = IdGenerator.Instance.Id;
            zoneDataStorage.AddOrUpdateZoneData(zoneData);
            PlayersDataStorage playersDataStorage = PlayersDataStorage.Instance;
            playersDataStorage.RemoveAllTransitionDatasFromZone(zoneData);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (this.isActiveAndEnabled)
            {
                //Check inconsistent player at list
                playersInside.RemoveAll(r => r.isActiveAndEnabled == false || r == null);

                if (other.TryGetComponent<SeeThroughShaderPlayer>(out var STSPlayer))
                {
                    if (playersInside.Contains(STSPlayer) == false)
                    {
                        playersInside.Add(STSPlayer);
                        STSPlayer.OnEnterZone(this);
                    }

                    TransitionController.transitionEffectZones(1, other.transform, this);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (this.isActiveAndEnabled)
            {
                //Check inconsistent player at list
                playersInside.RemoveAll(r => r.isActiveAndEnabled == false || r == null);

                if (other.TryGetComponent<SeeThroughShaderPlayer>(out var STSPlayer))
                {
                    if (playersInside.Remove(STSPlayer))
                    {
                        STSPlayer.OnExitZone(this);
                    }

                    TransitionController.transitionEffectZones(-1, other.transform, this);
                }
            }
        }



        public void UpdateZoneData()
        {
            zoneDataStorage.AddOrUpdateZoneData(zoneData);
            zoneDataStorage.UpdateZonesDatasShaderFloatArray();
        }


        public void toggleZoneActivation()
        {
            if(this.isActiveAndEnabled)
            {

                if (isActivated)
                {
                    TransitionController.transitionEffectZones(-1, this);
                    isActivated = false;
                }
                else
                {
                    TransitionController.transitionEffectZones(1, this);
                    isActivated = true;
                }

            }

        }

    }
}