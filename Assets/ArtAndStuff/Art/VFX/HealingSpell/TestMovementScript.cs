using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovementScript : MonoBehaviour
{
    public Transform theObject;
    public Transform end;
    public Transform start;
    public float moveSpeed = .001f;
    private float lerpValue = 0;
    private bool goUp = false;


    private void FixedUpdate() 
    {
        

        if (lerpValue <= 0)
        {
            goUp = true;
            lerpValue += moveSpeed;
        }
        else if (lerpValue >= 1)
        {
            goUp = false;
            lerpValue -= moveSpeed;
        }
        else if (goUp == true)
        {
            lerpValue += moveSpeed;
        }
        else if(goUp == false)
        {
            lerpValue -= moveSpeed;
        }
        
        theObject.position = new Vector3(Mathf.Lerp(start.position.x, end.position.x, lerpValue), Mathf.Lerp(start.position.y, end.position.y, lerpValue), Mathf.Lerp(start.position.z, end.position.z, lerpValue));
    
    }
}
