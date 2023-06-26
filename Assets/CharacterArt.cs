using UnityEngine;

public class CharacterArt : MonoBehaviour
{
    public Transform baseArt;
    public Transform renderMask;
    public MeshRenderer renderSurface;

    public Material maskShader;
    public RenderTexture maskTexture;
    public RenderTexture renderTexture;

    public string renderLayer = "CharacterRender";
    public string maskLayer = "CharacterMask";

    private void Awake()
    {
        if (baseArt != null)
        {
            RecursiveBase(baseArt);
            if (renderMask == null)
            {
                renderMask = Instantiate(baseArt, transform);
                RecursiveMask(renderMask);
            }

        }
    }

    public void RecursiveBase(Transform root)
    {
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
