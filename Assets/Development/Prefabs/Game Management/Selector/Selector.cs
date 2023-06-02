using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static ISelectable;

public class Selector : BaseMonoBehaviour, IControllable
{
    public Rigidbody myRigidbody;
    public Impact cursor;

    public float speed = 700f;
    public float maxVelocity = 10f;
    public float moveDrag = 0.5f;
    public float stopDrag = 7.5f;
    public UnityEvent onConfirm;

    [SerializeField] private List<SelectType> selectableTypes;
    public List<SelectType> SelectableTypes { get { return selectableTypes; } set { SetSelectableTypes(value); } }

    private bool controllable = false; 
    public bool Controllable { get { return controllable; } set { SetControllable(value); } }

    private Vector2 moveVector = new Vector2(0, 0);
    public Vector2 MoveVector { get { return moveVector; } set { SetMoveVector(value); } }

    public List<ASelectable> hoveringOver = new List<ASelectable>();
    public ASelectable selected;

    private void Awake()
    {
        if (myRigidbody == null)
        {
            myRigidbody = GetComponent<Rigidbody>();
        }
        Controllable = false;
    }

    public void SetControllable(bool controllable = true)
    {
        hoveringOver.Clear();
        if (cursor != null && cursor.touching.Count > 0)
        {
            cursor.touching.Clear();
        }
        this.controllable = controllable;
        //cursor.gameObject.SetActive(controllable);
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

    
    // Selectables

    public void Hover(Impact impact)
    {
        ASelectable selectable = impact.selectable;
        if (selectable != null)
        {
            if (hoveringOver.Count > 0)
            {
                hoveringOver[hoveringOver.Count - 1].UnHover();
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
                hoveringOver[hoveringOver.Count - 1].Hover();
            }
        }
    }

    public void Select()
    {
        if (hoveringOver.Count > 0)
        {
            selected = hoveringOver[hoveringOver.Count - 1];
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
}