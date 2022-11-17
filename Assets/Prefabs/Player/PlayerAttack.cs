using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Weapon;
    [SerializeField] float Duration = 3f;
    float cooldown = 0.0f;
    bool attacking = false;

    public PlayerCore pCore;
    // Start is called before the first frame update
    void Start()
    {
        pCore = GetComponent<PlayerCore>();
    }

    private void Update()
    {
        if (attacking)
        {
            cooldown += Time.deltaTime;
            if (cooldown >= Duration)
            {
                attacking = false;
                cooldown = 0;
                Weapon.SetActive(false);
                //Weapon.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            }
        }
    }




    public void Slashie()
    {
        Vector2 movement = pCore.moveControls.getMoveVector();

        if (!attacking)
        {
            if (Mathf.Abs(movement.x) > 0.5f || Mathf.Abs(movement.y) > 0.5f)
            {
                Vector3 weapRotation = Vector3.right * -movement.x + Vector3.forward * -movement.y;
                if(weapRotation.sqrMagnitude > 0.0f)
                {
                    Quaternion newRotation = Quaternion.LookRotation(weapRotation, Vector3.up);
                    Weapon.transform.localRotation = newRotation;
                }
            }
            print("I'mma slashin'");
            Weapon.SetActive(true);
            attacking = true;
        }
        
        
        
        
        
    }

}
