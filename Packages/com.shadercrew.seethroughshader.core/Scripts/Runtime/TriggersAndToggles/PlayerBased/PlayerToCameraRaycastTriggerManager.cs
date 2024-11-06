using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{

    public class PlayerAndActivationTime
    {
        public Dictionary<GameObject, float> playerToLastTriggerActivationTime = new Dictionary<GameObject, float>();        
        public PlayerAndActivationTime(GameObject player, float lastTriggerActivationTime)
        {
            playerToLastTriggerActivationTime[player] = lastTriggerActivationTime;
        }
    }

    [AddComponentMenu(Strings.COMPONENTMENU_PLAYER_TO_CAMERA_RAYCAST_TRIGGER_MANAGER)]
    public class PlayerToCameraRaycastTriggerManager : MonoBehaviour
{
        private Dictionary<ManualTriggerByParent, PlayerAndActivationTime> triggerToPlayerAndActivationTime = new Dictionary<ManualTriggerByParent, PlayerAndActivationTime>();
        
        public List<GameObject> playerList;
        public float timeUntilExit = 0.1f;
        public bool ShowDebugRays = false;









#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Camera cam = Camera.main;
            if (cam != null && playerList != null && playerList.Count > 0)
            {
                // iterating through all players that are raycasting to the camera
                foreach (GameObject player in playerList)
                {
                    if (player != null)
                    {

                        if (player.GetComponent<SeeThroughShaderPlayer>() == null)
                        {
                            Gizmos.color = new Color(1, 0, 0, 3.0f);
                            Gizmos.DrawSphere(player.transform.position, 0.5f);
                            //DrawString("Test", player.transform.position);
                        }
                 
                    }

                }
            }

        }
#endif
        //public static void DrawString(string text, Vector3 worldPos, Color? textColor = null, Color? backColor = null)
        //{
        //    UnityEditor.Handles.BeginGUI();
        //    var restoreTextColor = GUI.color;
        //    var restoreBackColor = GUI.backgroundColor;

        //    GUI.color = textColor ?? Color.white;
        //    GUI.backgroundColor = backColor ?? Color.black;

        //    var view = UnityEditor.SceneView.currentDrawingSceneView;
        //    if (view != null && view.camera != null)
        //    {
        //        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        //        if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
        //        {
        //            GUI.color = restoreTextColor;
        //            UnityEditor.Handles.EndGUI();
        //            return;
        //        }
        //        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        //        var r = new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y);
        //        GUI.Box(r, text, EditorStyles.numberField);
        //        GUI.Label(r, text);
        //        GUI.color = restoreTextColor;
        //        GUI.backgroundColor = restoreBackColor;
        //    }
        //    UnityEditor.Handles.EndGUI();
        //}

        void Update()
        {
            Camera cam = Camera.main;
                        

            if (cam != null && playerList != null && playerList.Count > 0)
            {
                // iterating through all players that are raycasting to the camera
                foreach (GameObject player in playerList)
                {
                    if(ShowDebugRays)
                    {
                        Debug.DrawRay(player.transform.position, cam.transform.position - player.transform.position, Color.magenta);
                    }

                    if(player != null)
                    {

                        if (player.GetComponent<SeeThroughShaderPlayer>() == null) 
                        {
                            Gizmos.color = new Color(0, 1, 0, 0.2f);
                            Gizmos.DrawSphere(player.transform.position, 1);
                        }

                        // iterates through all raycast hits and if one hit or its parents contain the "ManualTriggerByParent" component, ActivateTrigger(player) 
                        // will be called with the current player as it's argument. It's a bit complex but we have to keep track of which player already activated
                        // which trigger, and fill out the dictionaries if certain trigger and/or player didn't exist in it yet
                        float distancePlayerToCamera = Vector3.Distance(cam.transform.position, player.transform.position);
                        RaycastHit[] hits = Physics.RaycastAll(player.transform.position, cam.transform.position - player.transform.position, distancePlayerToCamera);
                        foreach (RaycastHit hit in hits)
                        {
                            ManualTriggerByParent manualTriggerByParent = hit.transform.gameObject.GetComponentInParent<ManualTriggerByParent>();
                            if (manualTriggerByParent != null)
                            {
                                if (!triggerToPlayerAndActivationTime.ContainsKey(manualTriggerByParent))
                                {
                                    triggerToPlayerAndActivationTime[manualTriggerByParent] = new PlayerAndActivationTime(player, Time.realtimeSinceStartup);
                                    //manualTriggerByParent.ActivateTrigger(player.gameObject.GetComponent<Collider>());
                                    manualTriggerByParent.ActivateTrigger(player);
                                }
                                else if (!triggerToPlayerAndActivationTime[manualTriggerByParent].playerToLastTriggerActivationTime.ContainsKey(player))
                                {
                                    triggerToPlayerAndActivationTime[manualTriggerByParent].playerToLastTriggerActivationTime[player] = Time.realtimeSinceStartup;
                                    //manualTriggerByParent.ActivateTrigger(player.gameObject.GetComponent<Collider>());
                                    manualTriggerByParent.ActivateTrigger(player);

                                }
                                else
                                {
                                    float lastTriggerActivationTime = triggerToPlayerAndActivationTime[manualTriggerByParent].playerToLastTriggerActivationTime[player];
                                    // this checks if we haven't recently called the activate function for the particular player and trigger, as multiple activation calls in
                                    // a row breaks the shader code
                                    if (Time.realtimeSinceStartup - lastTriggerActivationTime > timeUntilExit)
                                    {
                                        //manualTriggerByParent.ActivateTrigger(player.GetComponent<Collider>());
                                        manualTriggerByParent.ActivateTrigger(player);

                                    }
                                    triggerToPlayerAndActivationTime[manualTriggerByParent].playerToLastTriggerActivationTime[player] = Time.realtimeSinceStartup;
                                }
                            }
                        }
                    }
                    
                }


                //  this checks if any triggers weren't activated recently and so automatically calls the deactive functions for specific triggers and players
                if (triggerToPlayerAndActivationTime.Count > 0)
                {
                    List<ManualTriggerByParent> keyList = new List<ManualTriggerByParent>(triggerToPlayerAndActivationTime.Keys);
                    foreach (ManualTriggerByParent trigger in keyList)
                    {
                        List<GameObject> playerList = new List<GameObject>(triggerToPlayerAndActivationTime[trigger].playerToLastTriggerActivationTime.Keys);
                        foreach (GameObject player in playerList)
                        {
                            if (player != null)
                            {
                                float lastTriggerActivationTime = triggerToPlayerAndActivationTime[trigger].playerToLastTriggerActivationTime[player];

                                //  If this is true, we can call the deactive function as we know that this particular player doesn't raycast onto
                                //  this specific trigger gameobject anymore. 
                                if (Time.realtimeSinceStartup - lastTriggerActivationTime > timeUntilExit)
                                {
                                    //trigger.DeactivateTrigger(player.GetComponent<Collider>());
                                    trigger.DeactivateTrigger(player);
                                    if (triggerToPlayerAndActivationTime[trigger].playerToLastTriggerActivationTime.Keys.Count > 1)
                                    {
                                        triggerToPlayerAndActivationTime[trigger].playerToLastTriggerActivationTime.Remove(player);
                                    }
                                    else
                                    {
                                        triggerToPlayerAndActivationTime.Remove(trigger);
                                    }
                                }
                            }
                        }
                    }


                }

            }
        }
    }
}