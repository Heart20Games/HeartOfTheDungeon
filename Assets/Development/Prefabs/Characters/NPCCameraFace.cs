using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCameraFace : MonoBehaviour
{
    public Transform cam;
    public GameObject player;

    private void Start() 
    {
        player = GameObject.Find("/Player V2/Body");
        cam = player.transform.Find("Main Camera");    
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = cam.transform.eulerAngles; 
    }
}
