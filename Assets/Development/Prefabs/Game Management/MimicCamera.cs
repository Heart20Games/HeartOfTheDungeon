using Body.Behavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MimicCamera : MonoBehaviour
{
    private Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        camera.fieldOfView = Camera.main.fieldOfView;
        camera.nearClipPlane = Camera.main.nearClipPlane;
        camera.farClipPlane = Camera.main.farClipPlane;
    }
}
