using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using static ISelectable;

public class Selector : BaseMonoBehaviour, IControllable, ITimeScalable
{
    public Rigidbody myRigidbody;
    public Impact cursor;
    public CinemachineVirtualCamera virtualCamera;

    [Header("Physics")]
    public float speed = 700f;
    public float maxVelocity = 10f;
    public float footstepVelocity = 1f;
    public float moveDrag = 0.5f;
    public float stopDrag = 7.5f;
    public bool canMove = true;
    public bool applyGravity = true;
    public float normalForce = 0.1f;
    public float gravityForce = 1f;
    public float groundDistance = 0.01f;

    [Header("Control")]
    private bool controllable = false; 
    public bool Controllable { get { return controllable; } set { SetControllable(value); } }

    [Header("Scale")]
    [SerializeField] private float timeScale = 1f;
    public float TimeScale { get => timeScale; set => timeScale = SetTimeScale(value); }

    [Header("Vectors")]
    [SerializeField] private Vector2 moveVector = new(0, 0);
    public Vector2 MoveVector { get { return moveVector; } set { SetMoveVector(value); } }
    private bool onGround = false;
    public bool swapAxes = true;

    [Header("Selection")]
    [SerializeField] private List<SelectType> selectableTypes;
    public List<SelectType> SelectableTypes { get { return selectableTypes; } set { SetSelectableTypes(value); } }
    public List<ASelectable> hoveringOver = new();
    public ASelectable selected;
    public UnityEvent onConfirm;


    private void Awake()
    {
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody>();
        Controllable = false;
    }

    public void SetControllable(bool controllable = true)
    {
        hoveringOver.Clear();
        if (cursor != null && cursor.touching.Count > 0)
            cursor.touching.Clear();
        if (virtualCamera != null)
            virtualCamera.gameObject.SetActive(controllable);
        this.controllable = controllable;
        gameObject.SetActive(controllable);
        DeSelect();
    }

    public void SetSelectableTypes(List<SelectType> types)
    {
        selectableTypes = types;
        cursor.SelectableTypes = selectableTypes;
    }

    public void SetMoveVector(Vector2 vector)
    {
        print("Setting move vector");
        moveVector = vector.SwapAxes(swapAxes) * (swapAxes ? new Vector2(-1, 1) : Vector2.one);
        myRigidbody.drag = moveVector.magnitude == 0 ? stopDrag : moveDrag;
        cursor.transform.SetLocalRotationWithVector(MoveVector);
    }

    private void FixedUpdate()
    {
        Vector3 cameraDirection = transform.position - Camera.main.transform.position;

        if (controllable)
        {
            Vector3 direction = moveVector.Orient(cameraDirection).FullY();
            Debug.DrawRay(transform.position, direction * 3, Color.green, Time.fixedDeltaTime);
            myRigidbody.AddRelativeForce(speed * Time.fixedDeltaTime * timeScale * direction, ForceMode.Force);
        }

        if (myRigidbody.velocity.magnitude > maxVelocity)
            myRigidbody.velocity = myRigidbody.velocity.normalized * maxVelocity;

        if (applyGravity)
        {
            onGround = Physics.Raycast(transform.position, Vector3.down, groundDistance);
            float power = onGround ? normalForce : gravityForce;
            myRigidbody.AddForce(speed * timeScale * power * Time.fixedDeltaTime * Vector3.down);
        }
    }


    // Selectables

    public void Hover(Impact impact)
    {
        ASelectable selectable = impact.selectable;
        if (selectable != null)
        {
            if (hoveringOver.Count > 0)
            {
                hoveringOver[^1].UnHover();
            }
            hoveringOver.Add(selectable);
            selectable.Hover();
        }
    }

    public void UnHover(Impact impact)
    {
        ASelectable selectable = impact.selectable;
        if (selectable != null)
        {
            if (selected = selectable)
            {
                DeSelect();
            }
            hoveringOver.Remove(selectable);
            selectable.UnHover();
            if (hoveringOver.Count > 0)
            {
                hoveringOver[^1].Hover();
            }
        }
    }

    public void Select()
    {
        if (hoveringOver.Count > 0)
        {
            selected = hoveringOver[^1];
            if (selected.isSelected)
            {
                Confirm();
            }
            else
            {
                selected.Select();
            }
        }
    }

    public void DeSelect()
    {
        if (selected != null)
        {
            selected.DeSelect();
            selected = null;
        }
    }

    public void Confirm()
    {
        selected.Confirm();
        onConfirm.Invoke();
    }


    // TimeScale

    private Vector3 tempVelocity;
    public float SetTimeScale(float timeScale)
    {
        if (myRigidbody != null && this.timeScale != timeScale)
        {
            if (timeScale == 0)
            {
                tempVelocity = myRigidbody.velocity;
                myRigidbody.velocity = new Vector3();
            }
            else if (this.timeScale == 0)
            {
                myRigidbody.velocity = tempVelocity;
            }
            else
            {
                float ratio = timeScale / this.timeScale;
                myRigidbody.velocity *= ratio;
            }
        }
        this.timeScale = timeScale;
        return timeScale;
    }
}