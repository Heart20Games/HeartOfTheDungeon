using UnityEngine;
using Body;
using UnityEngine.Events;

public class Caster : BaseMonoBehaviour
{
    public bool canAttack = true;
    private Character character;
    private ArtRenderer artRenderer;
    private Transform pivot;
    public ICastable Castable;
    private Vector3 weapRotation = Vector3.forward;

    [Header("Vector")]
    public Vector2 castVector = new(0, 0);
    public UnityEvent<Vector2> OnSetCastVector;

    private void Awake()
    {
        character = GetComponent<Character>();
        artRenderer = character.artRenderer;
        pivot = character.pivot;
    }

    // Cast Vector
    public void SetCastVector(Vector2 aimVector, Vector2 fallback = new())
    {
        if (fallback.magnitude > 0 || aimVector.magnitude > 0)
        {
            castVector = aimVector.magnitude > 0 ? aimVector : fallback;
            if (castVector.magnitude > 0)
                OnSetCastVector.Invoke(castVector);
        }
    }

    // Cast
    private Vector3 lastDirection;
    public void Cast() { Cast(castVector); }
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