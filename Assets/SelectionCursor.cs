using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SelectionCursor : MonoBehaviour
{
    public Rigidbody myRigidbody;

    public float speed = 700f;
    public float maxVelocity = 10f;
    public float moveDrag = 0.5f;
    public float stopDrag = 7.5f;
    public UnityEvent OnSetMoveVector;
    public bool controllable = false;
    public bool Controllable { get { return controllable; } set { SetControllable(value); } }

    private Vector2 moveVector = new Vector2(0, 0);
    public Vector2 MoveVector { get { return moveVector; } set { SetMoveVector(value); } }

    private void Awake()
    {
        if (myRigidbody == null)
        {
            myRigidbody = GetComponent<Rigidbody>();
        }
    }

    public void SetControllable(bool controllable=true)
    {
        this.controllable = controllable;
    }

    public void SetMoveVector(Vector2 vector)
    {
        moveVector = vector;
        myRigidbody.drag = moveVector.magnitude == 0 ? stopDrag : moveDrag;
    }

    private void FixedUpdate()
    {
        myRigidbody.AddRelativeForce(new Vector3(moveVector.x, 0, moveVector.y) * speed * Time.fixedDeltaTime, ForceMode.Force);
        if (myRigidbody.velocity.magnitude > maxVelocity)
        {
            myRigidbody.velocity = myRigidbody.velocity.normalized * maxVelocity;
        }
    }
}