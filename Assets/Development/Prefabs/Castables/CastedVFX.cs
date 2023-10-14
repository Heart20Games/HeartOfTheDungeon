using HotD.Castables;
using UnityEngine.VFX;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(VisualEffect))]
public class CastedVFX : Casted
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public VisualEffect[] visuals;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        visuals = GetComponentsInChildren<VisualEffect>();
    }

    public bool VisualsEnabled
    {
        get
        {
            foreach (var visual in visuals)
                if (!visual.enabled) return false;
            return true;
        }
        set
        {
            foreach (var visual in visuals)
                visual.enabled = value;
        }
    }
}
