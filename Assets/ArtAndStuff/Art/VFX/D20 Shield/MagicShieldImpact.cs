using HotD.Castables;
using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using static Impact;

public class MagicShieldImpact : CastLocationFollower
{
    [Foldout("Impact VFX", true)]
    [SerializeField] private bool impact = false;
    [SerializeField] private bool impact1Active;
    [SerializeField] private bool impact2Active;
    [SerializeField] private bool impact3Active;
    public Vector3 impactLocation;
    [ReadOnly][SerializeField] private Vector3 impactDirection;
    public Vector3 impactAxis;

    [Foldout("Shield VFX", true)]
    [SerializeField] private Vector3 vertexMovementAmount = new(.12f, .12f, .12f);
    [SerializeField] private float vertexDistortionAmount;
    [SerializeField] float impactDuration = .35f;
    [SerializeField] private float dissolveAmount;
    [SerializeField] private float dissolveDuration;
    [SerializeField] private bool shieldTransitioning;
    [SerializeField] private float starSpherePower;
    [Foldout("Shield VFX")]
    [SerializeField] private float starSphereExplode;
    
    [SerializeField] private bool shieldOn = false;
    [SerializeField] private bool debug;
    [SerializeField] private bool explode;

    private Vector3 objectPivot;
    private VisualEffect visualEffect;
    private float vertexDistortDuration = .5f;
    
    // Start is called before the first frame update
    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
        dissolveAmount = 1;
        starSpherePower = 1;
        starSphereExplode = 0;
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
            visualEffect.SetFloat("Star Sphere Explosion", starSphereExplode);

            if(impact == true)
            {
                ImpactTrigger();
                impact = false;
            }

            if (vertexDistortDuration > 0f)
            {
                VertexDistortion();
            }

            if (shieldOn && !shieldTransitioning && !explode && dissolveAmount > 0f)
            {
                StartCoroutine(ShieldActivate());
            }
            else if (!shieldOn && !shieldTransitioning && !explode && dissolveAmount < 1f)
            {
                StartCoroutine(ShieldDeactivate());
            }

            if(explode == true &&!shieldTransitioning)
            {
                StartCoroutine(ShieldExplode());
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

    //Creates an impact using Impact System 1 at the Impact Ripples Position, and aligns the it along the axis provided in Impact Rotation Axis
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
        while(dissolveAmount >= 0)
        {
            dissolveAmount -= Mathf.Clamp(1f/50f, 0f, 1f);
            starSpherePower -= Mathf.Clamp(1f/50f, 0f, 1f);
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        shieldTransitioning = false;
        dissolveAmount = 0f;
        starSpherePower = 0f;
    }

    //begins the shield deactivation VFX cycle
    private IEnumerator ShieldDeactivate()
    {
        shieldTransitioning = true;
        while(dissolveAmount <= 1f)
        {
            dissolveAmount += Mathf.Clamp(1f/50f, 0f, 1f);
            starSpherePower += Mathf.Clamp(1f/50f, 0f, 1f);
            yield return new WaitForSeconds(dissolveDuration/50f);
        }    
        shieldTransitioning = false;
        dissolveAmount = 1f;
        starSpherePower = 1f;
    }

      IEnumerator ShieldExplode()
    {
        shieldTransitioning = true;
        visualEffect.SendEvent("Explode");
        dissolveAmount = 1f;
        while(starSphereExplode < 1f)
        {
            //dissolveAmount += Mathf.Clamp(1f/50f, 0f, 1f);
            starSpherePower -= Mathf.Clamp(1f/50f, -1f, 1f);
            starSphereExplode += Mathf.Clamp(1f/50f, 0f, 1f);
            yield return new WaitForSeconds(dissolveDuration/400f);
        }
        while(starSpherePower < 1f)
        {
            starSpherePower += Mathf.Clamp(1f/50f, 0f, 1f);
            yield return new WaitForSeconds(dissolveDuration/200f);
        }
        shieldOn = false;
        starSphereExplode = 0f;
        starSpherePower = 1f;
        explode = false;
        shieldTransitioning = false;
    }

    public void ToggleShield(bool enabled)
    {
        if(enabled)
        {
            shieldOn = true;
        }
        else
        {
            shieldOn = false;
            StartCoroutine(ShieldDeactivate());
        }
    }

    public void OnImpact(Other other)
    {
        if (visualEffect != null && other.gameObject != null)
        {
            impactLocation = other.ImpactLocation;
            impactDirection = (impactLocation - objectPivot).normalized;
            objectPivot = gameObject.transform.position * -1;
            Quaternion quatRotation = Quaternion.FromToRotation(Vector3.forward, impactDirection);
            impactAxis = quatRotation.eulerAngles;
            Print($"Impact! {impactLocation} / {impactAxis}", debug, this);
            ImpactTrigger();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Projectile>())
        {
            if (other.GetComponent<Projectile>().ShouldIgnoreDodgeLayer) return;
        }

        if(other.GetComponent<Impact>())
        {
            OnImpact(other.GetComponent<Impact>().other);
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