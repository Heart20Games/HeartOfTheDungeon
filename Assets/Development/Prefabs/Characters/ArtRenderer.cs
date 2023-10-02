using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ArtRenderer : MonoBehaviour
{
    // Properties

    public Transform baseArt;
    public Material baseMaterial;

    [Header("Equipment")]
    public Transform weaponHand;

    [Header("Animation")]
    public Animator animator;
    //public List<string> animationParameters = new() { "run", "hit", "attack", "dead", "die" };
    //private readonly Dictionary<string, bool> parameterExists = new();
    //public List<string> existingParameters = new();
    public bool Running { get => GetAnimBool("run"); set => SetAnimBool("run", value); }
    public void Trigger(int idx) { AnimTrigger($"trigger{idx}"); }
    public void Release(int idx) { AnimTrigger($"release{idx}"); }
    public void Cast(int idx) { AnimTrigger($"cast{idx}"); }
    public void UnCast(int idx) { AnimTrigger($"unCast{idx}"); }
    public void Hit() { AnimTrigger("hit"); }
    public bool Dead { get => GetAnimBool("dead"); set => SetAnimBoolTrigger("dead", value, "die"); }

    public bool toggleDead = false;

    [Header("Camera Shader Layers")]
    public bool useCameraShaderLayers;
    public Transform renderMask;
    public MeshRenderer renderSurface;

    public Material maskShader;
    public RenderTexture maskTexture;
    public RenderTexture renderTexture;

    public string renderLayer = "CharacterRender";
    public string maskLayer = "CharacterMask";

    public bool debug = false;


    // Initialization

    private void Awake()
    {
        if (baseArt != null)
        {
            if (animator == null)
                baseArt.TryGetComponent(out animator);
            RecursiveBase(baseArt);

            if (useCameraShaderLayers)
            {
                if (renderMask == null)
                {
                    renderMask = Instantiate(baseArt, transform);
                    RecursiveMask(renderMask);
                }
            }
            
            if (renderSurface != null)
            {
                renderSurface.gameObject.SetActive(true);
                renderSurface.enabled = true;
            }
        }
    }


    public void Update()
    {
        if (toggleDead)
        {
            Dead = !Dead;
            toggleDead = false;
        }
    }


    // Equipment
    public void DisplayWeapon(Transform weaponArt)
    {
        if (weaponHand != null && weaponArt)
        {
            Vector3 weaponLocalPosition = weaponArt.localPosition;
            weaponArt.SetParent(weaponHand, false);
            weaponArt.localPosition = weaponLocalPosition;
        }
    }


    // Animation

    private bool HasParameter(string parameter)
    {
        return animator.HasParameter(parameter);
    }

    private void AnimTrigger(string parameter)
    {
        if (HasParameter(parameter))
            animator.SetTrigger(parameter);
    }

    private bool GetAnimBool(string parameter)
    {
        if (HasParameter(parameter))
            return animator.GetBool(parameter);
        return false;
    }

    private void SetAnimBoolTrigger(string boolParameter, bool value, string triggerParameter)
    {
        if (debug) print($"Set Anim BoolTrigger: {boolParameter} / {triggerParameter} ({value})");
        SetAnimBool(boolParameter, value);
        if (value)
            AnimTrigger(triggerParameter);
    }

    private void SetAnimBool(string parameter, bool value)
    {
        if (HasParameter(parameter))
            animator.SetBool(parameter, value);
    }


    // Recursive Initializers

    public void RecursiveBaseLayer(string layerName)
    {
        baseArt.ApplyLayerRecursive(layerName);
    }

    public void RecursiveBase(Transform root)
    {
        if (baseMaterial != null && root.TryGetComponent<SpriteRenderer>(out var sprite))
            sprite.material = baseMaterial;
        if (useCameraShaderLayers)
            root.gameObject.layer = LayerMask.NameToLayer(renderLayer);
        
        for (int i = 0; i < root.childCount; i++)
        {
            RecursiveBase(root.GetChild(i));
        }
    }

    public void RecursiveMask(Transform root)
    {
        if (root.TryGetComponent<SpriteRenderer>(out var renderer))
        {
            renderer.material = maskShader;
            root.gameObject.layer = LayerMask.NameToLayer(maskLayer);
        }
        
        for (int i = 0; i < root.childCount; i++)
        {
            RecursiveMask(root.GetChild(i));
        }
    }
}
