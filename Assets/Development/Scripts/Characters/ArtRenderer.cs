using HotD.Castables;
using UnityEngine;

namespace HotD.Body
{
    public class ArtRenderer : CastCoordinator, IWeaponDisplay
    {
        // Properties

        public Transform baseArt;
        public Material baseMaterial;

        [Header("Equipment")]
        public Transform weaponHand;

        //public List<string> animationParameters = new() { "run", "hit", "attack", "dead", "die" };
        //private readonly Dictionary<string, bool> parameterExists = new();
        //public List<string> existingParameters = new();
        public bool Running { get => GetBool("Run"); set => SetBool("Run", value); }
        public float RunVelocity { get => GetFloat("RunVelocity"); set => SetFloat("RunVelocity", value); }
        public void Trigger(int idx) { SetTrigger($"trigger{idx}"); }
        public void Release(int idx) { SetTrigger($"release{idx}"); }
        public void Cast(int idx) { SetTrigger($"Action{idx}"); }
        public void UnCast(int idx) { SetTrigger($"EndCast{idx}"); }
        public void Hit() { SetTrigger("Hit"); }
        public bool Dead { get => GetBool("Dead"); set => SetBool("Dead", value); }

        public bool toggleDead = false;

        [Header("Camera Shader Layers")]
        public bool useCameraShaderLayers;
        [SerializeField] private bool keepActive;
        public Transform renderMask;
        public MeshRenderer renderSurface;

        public Material maskShader;
        public RenderTexture maskTexture;
        public RenderTexture renderTexture;

        public string renderLayer = "CharacterRender";
        public string maskLayer = "CharacterMask";

        public bool KeepActive => keepActive;

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

        //private bool HasParameter(string parameter)
        //{
        //    return animator.HasParameter(parameter, this);
        //}

        //private void SetAnimBoolTrigger(string boolParameter, bool value, string triggerParameter)
        //{
        //    if (debug) print($"Set Anim BoolTrigger: {boolParameter} / {triggerParameter} ({value})");
        //    SetBool(boolParameter, value);
        //    if (value)
        //        SetTrigger(triggerParameter);
        //}


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
}