using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : MonoBehaviour
{

    public float rotationSpeed = 60f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotationVector = Vector3.zero;
        rotationVector.y = rotationSpeed * Time.deltaTime;
        this.transform.Rotate(rotationVector);
    }
}
