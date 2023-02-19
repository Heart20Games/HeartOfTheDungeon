using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> targets;
    [SerializeField] private GameObject selectedTarget;


    public float rotationSpeed = 10.0f;

    private Transform spriteTransform;
    private Vector3 rotationDirection;
    private Vector2 movementVector;

    private void Start()
    {
        // Initialize the targets list here, if needed.
        spriteTransform= transform;

    }

    private void Update()
    {
        // Do something with the targets list here, such as looping through the list
        // and updating each target's position or status.
    }

    public void TargetA(InputAction.CallbackContext context)
    {
        
        Debug.Log("Target is: " + context.ReadValue<Vector2>());
        // Set companion based on direction.
        if(context.ReadValue<Vector2>().x < 0)
        {
            selectedTarget = targets[2]; // select Osseus
        }else if(context.ReadValue<Vector2>().x > 0)
        {
            selectedTarget = targets[1]; // select Rotta
        }else if(context.ReadValue<Vector2>().y < 0)
        {
            selectedTarget = targets[0]; // select Gobkin
        }
        this.transform.position = selectedTarget.transform.position;
        this.transform.SetParent(selectedTarget.transform);
        //this.transform.SetLocalPositionAndRotation(transform.position, this.transform.rotation);
    }

    public void OnLeftStickChange(InputAction.CallbackContext context)
    {

        Vector2 movementVector = context.ReadValue<Vector2>();
        if(Mathf.Abs(movementVector.x)+ Mathf.Abs(movementVector.y) > 0.5)
        {
            //rotationDirection.z = movementVector.x;
            //transform.Rotate(rotationDirection);
            rotationDirection.z = Mathf.Atan2(movementVector.y, movementVector.x) * Mathf.Rad2Deg;
            spriteTransform.rotation = Quaternion.Slerp(spriteTransform.rotation, Quaternion.Euler(90, -rotationDirection.z + 90, 0), rotationSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("This is the absolute value of your inputs: " + (Mathf.Abs(movementVector.x) + Mathf.Abs(movementVector.y)));
        }

    }
}
