using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasCamera : MonoBehaviour
{
    [SerializeField][ReadOnly] GameObject mainCamera;
    [SerializeField][ReadOnly] Canvas canvas;

    [ButtonMethod]
    public void SetCameraProperly()
    {
        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;

        mainCamera = Camera.main.gameObject;
        canvas.worldCamera = Camera.main; //mainCamera.GetComponent<Camera>();
    }

    private void Start()
    {
        SetCameraProperly();
    }
}
