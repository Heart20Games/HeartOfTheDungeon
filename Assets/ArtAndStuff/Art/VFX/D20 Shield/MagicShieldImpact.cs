using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static Impact;

public class MagicShieldImpact : BaseMonoBehaviour
{
    private Vector3 objectPivot;
    public bool impact = false;
    [SerializeField] private bool impact1Active;
    [SerializeField] private bool impact2Active;
    [SerializeField] private bool impact3Active;
    public Vector3 impactLocation;
    [ReadOnly][SerializeField] private Vector3 impactDirection;
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
    [SerializeField] private bool debug;
    
    // Start is called before the first frame update
    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
        dissolveAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (visualEffect != null)
        {
            objectPivot = gameObject.transform.position * -1;
            visualEffect.SetVector3("Impact Object Pivot", objectPivot);
            visualEffect.SetFloat("Dissolve Amount", dissolveAmount);
            if(impact == true)
            {
                ImpactTrigger();
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

    void ImpactTrigger()
    {
        ImpactTrigger(impactAxis, impactLocation);
    }

    void ImpactTrigger(Vector3 impactAxis, Vector3 impactLocation)
    {
        if(!impact1Active)
            StartCoroutine(Impact1(impactAxis, impactLocation));
        else
            impact = false;
    }

    IEnumerator Impact1(Vector3 impactAxis, Vector3 impactLocation)
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

    IEnumerator ShieldActivate()
    {
        visualEffect.SetBool("ShieldOff", false);
        shieldTransitioning = true;
        while(dissolveAmount > 0)
        {
            dissolveAmount -= 1f/50f;
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        visualEffect.SendEvent("ShieldOn");
        shieldTransitioning = false;
    }

    IEnumerator ShieldDeactivate()
    {
        shieldTransitioning = true;
        visualEffect.SetBool("ShieldOff", true);
        while(dissolveAmount < 1f)
        {
            dissolveAmount += 1f/50f;
            yield return new WaitForSeconds(dissolveDuration/50f);
        }    
        shieldTransitioning = false;
    }

    public void OnImpact(Impact.Other other)
    {
        if (visualEffect != null && other.gameObject != null)
        {
            impactLocation = other.ImpactLocation;
            impactDirection = (impactLocation - objectPivot).normalized;
            objectPivot = gameObject.transform.position * -1;
            Quaternion quatRotation = Quaternion.FromToRotation(Vector3.forward, impactDirection);
            impactAxis = quatRotation.eulerAngles;
            Print($"Impact! {impactLocation} / {impactAxis}");
            ImpactTrigger();
        }
    }

    [ButtonMethod]
    public void TestImpactTrigger()
    {
        if (visualEffect != null)
        {
            ImpactTrigger();
        }
    }
}
