using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Level3BoltScaling : MonoBehaviour
{
    public GameObject meshCollider;
    public VisualEffect bolt;
    public float scaleSpeed;
    public float castDuration;
    [SerializeField] private float currentScale;
    [SerializeField] public bool cast = false;

    // Start is called before the first frame update
    void Start()
    {
        currentScale = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        if(currentScale < 50f && cast == true)
        {
            currentScale += Time.deltaTime * scaleSpeed;
            currentScale = Mathf.Clamp(currentScale, 0f, 50f);
            bolt.SetFloat("Scale", currentScale);
            meshCollider.transform.localScale = new Vector3(meshCollider.transform.localScale.x, meshCollider.transform.localScale.y, (currentScale * 100)); 
        }
        else if (currentScale >= 50 && cast == true && (currentScale - 50) < castDuration)
        {
            currentScale += Time.deltaTime;
        }
        else if (currentScale >= 50 && cast == true && (currentScale - 50) >= castDuration)
        {
            currentScale = 0f;
            cast = false;
            bolt.SetFloat("Scale", currentScale);
            meshCollider.transform.localScale = new Vector3(meshCollider.transform.localScale.x, meshCollider.transform.localScale.y, (currentScale * 100));

        }
        else if (cast == false && currentScale != 0f)
        {
            currentScale = 0f;
             bolt.SetFloat("Scale", currentScale);
            meshCollider.transform.localScale = new Vector3(meshCollider.transform.localScale.x, meshCollider.transform.localScale.y, (currentScale * 100));
        }
    }
}
