using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpinAndFall : MonoBehaviour
{
    public float spinSpeed = 30f;
    public float fallSpeed = 5f;
    public float destructionY = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotationVector = Vector3.one;
        rotationVector  *= spinSpeed * Time.deltaTime;
        this.transform.Rotate(rotationVector);

        Vector3 translationVector = Vector3.zero;
        translationVector.y = -fallSpeed * Time.deltaTime;
        this.transform.position += translationVector;

        if(this.transform.position.y <= destructionY)
        {
            DestroyImmediate(this.gameObject);
        }
    }
}
