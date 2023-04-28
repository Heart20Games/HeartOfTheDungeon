using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballStretch : MonoBehaviour
{
    public GameObject cannonball;
    private Rigidbody cannonbody;
    private Vector3 curVelocity;
    private Vector3 startingScale;
    private float stretchScale;
    public float maxStretch;
    public float stretchAmount;

    
    // Start is called before the first frame update
    void Start()
    {
        startingScale = transform.localScale;
        cannonbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        curVelocity = transform.InverseTransformDirection(cannonbody.velocity);
        float absX = Mathf.Abs(cannonbody.velocity.x);
        float absY = Mathf.Abs(cannonbody.velocity.y);
        float absZ = Mathf.Abs(cannonbody.velocity.z);

        //float angleY = 
        //Debug.Log(curVelocity);
        // Vector3 relativeVelocity = new Vector3((Mathf.Abs(curVelocity.x))*stretchAmount, (Mathf.Abs(curVelocity.y))*stretchAmount, (Mathf.Abs(curVelocity.z))*stretchAmount);
        // Debug.Log(relativeVelocity);
        // stretchScale = new Vector3(Mathf.Clamp(relativeVelocity.x, 0, maxStretch), Mathf.Clamp(relativeVelocity.y, 0, maxStretch), Mathf.Clamp(relativeVelocity.z, 0, maxStretch));
        // Debug.Log(stretchScale);
        // transform.localScale = stretchScale + startingScale;

        //float avgVelocity = (Mathf.Abs(curVelocity.x) + Mathf.Abs(curVelocity.y) + Mathf.Abs(curVelocity.z)) / 3;
        //float stretchAmount = Mathf.Clamp(avgVelocity * stretchScale, 0, maxStretch);
        //transform.localScale = new Vector3(startingScale.x + stretchAmount, startingScale.y, startingScale.z);
    }

    private void Update() 
    {        
        if(Input.GetKeyDown("h"))
        {
            cannonbody.AddForce(3, 3, 0, ForceMode.Impulse);
        }
    }
}
