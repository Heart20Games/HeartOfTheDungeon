using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class D20Shield : BaseMonoBehaviour
{
    [SerializeField]
    public GameObject shieldImpact;

    private VisualEffect shieldImpactVFX;

    private Transform impactContainer;
    

    private void Start() 
    {
        impactContainer = this.transform;     
    }

    public void OnCollisionEnter(Collision other) 
    {
        //if(other.gameObject.GetComponent<Damager>())
        //{        
            Debug.Log("Collision Detected");
            var impact = Instantiate(shieldImpact, transform) as GameObject;
            impact.transform.localPosition = new Vector3(0f, 0f, 0f);
            impact.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            shieldImpactVFX = impact.GetComponent<VisualEffect>();
            Vector3 worldPoint = other.contacts[0].point;
            Vector3 localPoint = impactContainer.InverseTransformPoint(worldPoint);
            shieldImpactVFX.SetVector3("Mask Position", new Vector3((Mathf.Clamp((localPoint.x), -.85f, .85f)), (Mathf.Clamp((localPoint.y), -.85f, .85f)), (Mathf.Clamp((localPoint.z), -.85f, .85f))));
            Destroy(impact, .5f);
        //}    
    }
}
