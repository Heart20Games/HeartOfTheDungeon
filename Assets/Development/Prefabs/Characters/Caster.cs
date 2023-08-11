using UnityEngine;
using Body;
using UnityEngine.Events;
using UnityEditor.AnimatedValues;

public class Caster : BaseMonoBehaviour
{
    public bool canAttack = true;
    private Character character;
    private ArtRenderer artRenderer;
    private Transform pivot;
    public ICastable Castable;
    private Vector3 weapRotation = Vector3.forward;
    [SerializeField] private bool debug;

    [Header("Vector")]
    public Vector2 fallback = new();
    public Vector2 fbOverride = new();
    public Vector2 castVector = new();
    public UnityEvent<Vector2> OnSetCastVector;

    private void Awake()
    {
        character = GetComponent<Character>();
        artRenderer = character.artRenderer;
        pivot = character.pivot;
    }

    // Cast Vector
    public void SetTarget(Transform target)
    {
        if (target != null)
        {
            if (debug) print($"Set target: {target} (on {character.Name})");
            Vector3 castPoint = (character.body.position + (Vector3.up * character.baseOffset)).XZVector();
            SetFallback(target.position - castPoint, true);
        }
        else SetFallback(new(), true);
    }
    public void SetFallback(Vector2 fallback, bool setOverride=false)
    {
        if (debug) print($"Set fallback{(setOverride ? " override" : "")}: {fallback} (on {character.Name})");
        if (setOverride) fbOverride = fallback;
        else this.fallback = fallback;
        SetVector(castVector);
    }
    public void SetVector(Vector2 aimVector)
    {
        if (fallback.magnitude > 0 || aimVector.magnitude > 0)
        {
            if (debug) print($"Set vector: {aimVector} (on {character.Name}");
            Vector2 fallback = fbOverride.magnitude > 0 ? fbOverride : this.fallback;
            castVector = aimVector.magnitude > 0 ? aimVector : fallback;
            if (castVector.magnitude > 0)
                OnSetCastVector.Invoke(castVector);
        }
    }


    // Cast
    private Vector3 lastDirection;

    // Cast based on the given Transform's position relative to the Camera.
    public void Cast(Transform body, Vector2 castVector) 
    {
        Debug.DrawRay(body.position, castVector.FullY() * 2f, Color.blue, 0.5f);
        Vector3 cameraDirection = body.position - Camera.main.transform.position;
        castVector = castVector.Orient(cameraDirection.XZVector().normalized).normalized;
        Debug.DrawRay(body.position, castVector.FullY() * 2f, Color.yellow, 0.5f);
        Cast(castVector);
    }
    // Cast using the given vector
    public void Cast(Vector2 castVector)
    {
        if (Castable != null && Castable.CanCast())
        {
            float pMag = Mathf.Abs(pivot.localScale.x);
            float sign = castVector.x < 0 ? -1 : 1;
            sign *= character.movement.Flip ? 1 : -1;
            pivot.localScale = new Vector3(pMag * sign, pivot.localScale.y, pivot.localScale.z);

            if (Mathf.Abs(castVector.x) > 0.5f || Mathf.Abs(castVector.y) > 0.5f)
                weapRotation = Vector3.right * -castVector.x + Vector3.forward * -castVector.y;
            
            if (artRenderer != null)
                artRenderer.Attack(Castable.GetItem() == null ? 0 : Castable.GetItem().attackIdx);
            
            if (weapRotation != lastDirection)
                lastDirection = weapRotation;
            
            Castable.Cast(weapRotation); // uses last rotation if not moving
        }
    }
}