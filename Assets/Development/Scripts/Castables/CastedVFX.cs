using HotD.Castables;
using UnityEngine.VFX;
using UnityEngine;
using UnityEngine.Events;

public class CastedVFX : Casted
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public VisualEffect[] visuals;
    [HideInInspector] public MeshRenderer[] meshes;

    public string triggerParameter = "";
    public string castParameter = "Cast";
    public string castKey = "";

    public void SetFirepoint(int firepointIdx) { toSetFirepoint.Invoke(firepointIdx); }
    [HideInInspector] public UnityEvent<int> toSetFirepoint;

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        visuals = GetComponentsInChildren<VisualEffect>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        base.Awake();
    }

    public bool animatorReset = false;

    public bool VisualsEnabled
    {
        get
        {
            if (animator != null)
                if (!animator.enabled) return false;
            foreach (var visual in visuals)
                if (!visual.enabled) return false;
            foreach (var mesh in meshes)
                if (!mesh.enabled) return false;
            return true;
        }
        set
        {
            if (animator != null)
            {
                if (!value)
                {
                    if (!animatorReset)
                    {
                        animator.SetTrigger("Reset");
                        animatorReset = true;
                    }
                }
                else
                {
                    animator.enabled = true;
                    animatorReset = false;
                }
            }
            foreach (var visual in visuals)
                visual.enabled = value;
            foreach (var mesh in meshes)
                mesh.enabled = value;
        }
    }

    public void AddListenerController(bool enableVisuals = false, bool active = false, UnityAction<int> firePointListener = null, UnityAction<int> powerLimitListener = null, UnityAction triggerListener = null, UnityAction releaseListener = null, UnityAction<Vector3> startCastListener = null, UnityAction endCastListener = null)
    {
        VisualsEnabled = false;
        toSetFirepoint.AddListener(firePointListener);
        base.AddListenerController(active, powerLimitListener, triggerListener, releaseListener, startCastListener, endCastListener);
    }
}
