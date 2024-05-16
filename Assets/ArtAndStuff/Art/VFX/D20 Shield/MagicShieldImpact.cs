using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MagicShieldImpact : MonoBehaviour
{
    private Vector3 objectPivot;
    public bool impact = false;
    [SerializeField] private bool impact1Active;
    [SerializeField] private bool impact2Active;
    [SerializeField] private bool impact3Active;
    public Vector3 impactLocation;
    public Vector3 impactAxis;
    private VisualEffect visualEffect;
    private float vertexDistortDuration = 0f;
    [SerializeField] private Vector3 vertexMovementAmount = new Vector3 (.12f, .12f, .12f);
    [SerializeField] private float vertexDistortionAmount;
    [SerializeField] float impactDuration = .35f;
    
    // Start is called before the first frame update
    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (visualEffect != null)
        {
            objectPivot = gameObject.transform.position * -1;
            visualEffect.SetVector3("Impact Object Pivot", objectPivot);
            if(impact == true)
            {
                impactTrigger();
                impact = false;
            }

            if (vertexDistortDuration > 0f)
            {
                VertexDistortion();
            }
        }
    }

    void impactTrigger()
    {
        if(!impact1Active)
            StartCoroutine(Impact1());
        else
            impact = false;
    }

    IEnumerator Impact1()
    {
        impact1Active = true;
        visualEffect.SetVector3("Impact Rotation Axis", impactAxis);
        visualEffect.SetVector3("Impact Ripples Position", impactLocation);
        visualEffect.SendEvent("Impact1");
        vertexDistortDuration = impactDuration; 
        yield return new WaitForSeconds(impactDuration);
        impact1Active = false;
        impact = false;
    }

    void VertexDistortion()
    {
        vertexDistortDuration -= impactDuration/50;
        visualEffect.SetFloat("Vertex Distortion Amount", Mathf.Lerp(0f, vertexDistortionAmount, vertexDistortDuration/impactDuration));
        visualEffect.SetVector3("Vertex Movement Amount", Vector3.Lerp(vertexMovementAmount, new Vector3(0.0f, 0.0f, 0.0f), vertexDistortDuration/impactDuration)); 
    }


}
