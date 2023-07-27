using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SimpleMovement))]
public class SimpleController : MonoBehaviour, IControllable
{
    public Impact cursor;
    public CinemachineVirtualCamera virtualCamera;
    private SimpleMovement movement;

    [Header("Control")]
    private bool controllable = false;
    public bool Controllable { get { return controllable; } set { SetControllable(value); } }
    public UnityEvent<bool> onControl;

    public Vector2 MoveVector { get => movement.MoveVector; set => movement.MoveVector = value; }

    private void Awake()
    {
        movement = GetComponent<SimpleMovement>();
        Controllable = false;
    }

    public void SetControllable(bool controllable = true)
    {
        if (virtualCamera != null)
            virtualCamera.gameObject.SetActive(controllable);
        this.controllable = controllable;
        gameObject.SetActive(controllable);
        onControl.Invoke(controllable);
    }
}
