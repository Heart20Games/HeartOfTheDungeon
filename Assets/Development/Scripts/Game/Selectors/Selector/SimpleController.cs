using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * The simple controller is meant to be a stripped down implementation of a controllable, moveable character with a camera and cursor.
 */

[RequireComponent(typeof(SimpleMovement))]
public class SimpleController : MonoBehaviour, IControllable
{
    public Impact cursor;
    public CinemachineVirtualCamera virtualCamera;
    private SimpleMovement movement;

    [Header("Control")]
    private bool controllable = false;
    public bool PlayerControlled { get { return controllable; } set { SetPlayerControlled(value); } }
    public UnityEvent<bool> onControl;

    public Vector2 MoveVector { get => movement.MoveVector; set => movement.MoveVector = value; }

    private void Awake()
    {
        movement = GetComponent<SimpleMovement>();
        PlayerControlled = false;
    }

    public void SetDisplayable(bool displayable = true)
    {
        // Not Implemented
    }

    public void SetSpectatable(bool spectatable)
    {
        if (virtualCamera != null)
            virtualCamera.gameObject.SetActive(spectatable);
    }

    public void SetPlayerControlled(bool controllable = true, bool _=false)
    {
        this.controllable = controllable;
        gameObject.SetActive(controllable);
        onControl.Invoke(controllable);
    }
}
