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
    [SerializeField] private bool shieldOn = false;
    [SerializeField] private float dissolveAmount;
    [SerializeField] private float dissolveDuration;
    [SerializeField] private bool shieldTransitioning;
    [SerializeField] private float starSpherePower;
    
    // Start is called before the first frame update
    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
        dissolveAmount = 1;
        starSpherePower = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (visualEffect != null)
        {
            objectPivot = gameObject.transform.position * -1;
            visualEffect.SetVector3("Impact Object Pivot", objectPivot);
            visualEffect.SetFloat("Dissolve Amount", dissolveAmount);
            visualEffect.SetFloat("Star Sphere Power", starSpherePower);
            if(impact == true)
            {
                impactTrigger();
                impact = false;
            }

            if (vertexDistortDuration > 0f)
            {
                VertexDistortion();
            }

            if (shieldOn && !shieldTransitioning)
            {
                StartCoroutine(ShieldActivate());
            }
            else if (!shieldOn && !shieldTransitioning)
            {
                StartCoroutine(ShieldDeactivate());
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
    
    //Creates an impact using Impact System 1 at the Impact Ripples Position, and aligns the it along the axis provided in Impact Rotation Axis
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
    
    //causes gradually diminishing distortions in the mesh when called, based on the impactDuration variable value
    void VertexDistortion()
    {
        vertexDistortDuration -= impactDuration/50;
        visualEffect.SetFloat("Vertex Distortion Amount", Mathf.Lerp(0f, vertexDistortionAmount, vertexDistortDuration/impactDuration));
        visualEffect.SetVector3("Vertex Movement Amount", Vector3.Lerp(vertexMovementAmount, new Vector3(0.0f, 0.0f, 0.0f), vertexDistortDuration/impactDuration)); 
    }

    //begins the shield activation VFX cycle
    IEnumerator ShieldActivate()
    {
        shieldTransitioning = true;
        while(dissolveAmount > 0)
        {
            dissolveAmount -= 1f/50f;
            starSpherePower -= 1f/50f;
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        shieldTransitioning = false;
    }

    //begins the shield deactivation VFX cycle
    IEnumerator ShieldDeactivate()
    {
        shieldTransitioning = true;
        while(dissolveAmount < 1f)
        {
            dissolveAmount += 1f/50f;
            starSpherePower += 1f/50f;
            yield return new WaitForSeconds(dissolveDuration/50f);
        }    
        shieldTransitioning = false;
    }

}
