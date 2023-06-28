using System.Collections.Generic;
using UnityEngine;

public class ArtRenderer : MonoBehaviour
{
    // Properties

    public Transform baseArt;
    public Material baseMaterial;

    [Header("Animation")]
    public Animator animator;
    private readonly List<string> animationParameters = new() { "run", "hit", "attack", "dead" };
    private readonly Dictionary<string, bool> parameterExists = new();
    public bool Running { get => GetAnimBool("run"); set => SetAnimBool("run", value); }
    public void Attack() { AnimTrigger("attack"); }
    public void Hit() { AnimTrigger("hit"); }
    public bool Dead { get => GetAnimBool("dead"); set => SetAnimBool("dead", value); }

    [Header("Camera Shader Layers")]
    public bool useCameraShaderLayers;
    public Transform renderMask;
    public MeshRenderer renderSurface;

    public Material maskShader;
    public RenderTexture maskTexture;
    public RenderTexture renderTexture;

    public string renderLayer = "CharacterRender";
    public string maskLayer = "CharacterMask";


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

        for (int i = 0; i < animationParameters.Count; i++)
        {
            parameterExists[animationParameters[i]] = animator.HasParameter(animationParameters[i]);
        }
    }


    // Animation

    private void AnimTrigger(string parameter)
    {
        if (parameterExists[parameter])
            animator.SetTrigger(parameter);
    }

    private bool GetAnimBool(string parameter)
    {
        if (parameterExists[parameter])
            return animator.GetBool(parameter);
        return false;
    }

    private void SetAnimBool(string parameter, bool value)
    {
        if (parameterExists[parameter])
            animator.SetBool(parameter, value);
    }


    // Recursive Initializers

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
