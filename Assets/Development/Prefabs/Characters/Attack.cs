using UnityEngine;
using Body;

public class Attack : BaseMonoBehaviour
{
    public bool canAttack = true;
    private Character character;
    private ArtRenderer artRenderer;
    private Transform pivot;
    public ICastable Castable;
    private Vector3 weapRotation = Vector3.forward;

    private void Awake()
    {
        character = GetComponent<Character>();
        artRenderer = character.artRenderer;
        pivot = character.pivot;
    }

    private Vector3 lastDirection;
    public void Slashie(Vector2 attackVector)
    {
        if (Castable != null && Castable.CanCast())
        {
            float pMag = Mathf.Abs(pivot.localScale.x);
            float sign = attackVector.x < 0 ? -1 : 1;
            sign *= character.movement.Flip ? 1 : -1;
            pivot.localScale = new Vector3(pMag * sign, pivot.localScale.y, pivot.localScale.z);

            if (Mathf.Abs(attackVector.x) > 0.5f || Mathf.Abs(attackVector.y) > 0.5f)
                weapRotation = Vector3.right * -attackVector.x + Vector3.forward * -attackVector.y;
            
            if (artRenderer != null)
                artRenderer.Attack(Castable.GetItem() == null ? 0 : Castable.GetItem().attackIdx);
            
            if (weapRotation != lastDirection)
                lastDirection = weapRotation;
            
            Castable.Cast(weapRotation); // uses last rotation if not moving
        }
    }
}