using HotD.Castables;
using UnityEngine.VFX;
using UnityEngine;

public class CastedVFX : Casted
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public VisualEffect[] visuals;
    [HideInInspector] public MeshRenderer[] meshes;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        visuals = GetComponentsInChildren<VisualEffect>();
        meshes = GetComponentsInChildren<MeshRenderer>();
    }

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
                animator.enabled = value;
            foreach (var visual in visuals)
                visual.enabled = value;
            foreach (var mesh in meshes)
                mesh.enabled = value;
        }
    }
}
