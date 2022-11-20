using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public bool canAttack = true;
    public PlayerCore pCore;
    public Animator animator;
    public Transform pivot;
    public Weapon Weapon;
    private Vector3 weapRotation = Vector3.forward;
    
    void Start()
    {
        pCore = GetComponent<PlayerCore>();
    }

    public void Slashie()
    {
        Vector2 movement = pCore.moveControls.getAttackVector();
        float pMag = Mathf.Abs(pivot.localScale.x);
        float sign = movement.x < 0 ? -1 : 1;
        pivot.localScale = new Vector3(pMag * sign, pivot.localScale.y, pivot.localScale.z);
        if (canAttack)
        {
            if (!Weapon.swinging) // set in weapon animation
            {
                if (Mathf.Abs(movement.x) > 0.5f || Mathf.Abs(movement.y) > 0.5f)
                {
                    weapRotation = Vector3.right * -movement.x + Vector3.forward * -movement.y;
                }
                animator.SetTrigger("attack");
                Weapon.Swing(weapRotation); // uses last rotation if not moving
                print("I'mma slashin'");
            }
            else
            {
                Debug.Log("Weapon Already Swinging");
            }
        }
    }
}