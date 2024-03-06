using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class FootstepsDustTrigger : MonoBehaviour
{
    
    [SerializeField] private VisualEffect footstep;
    [SerializeField] private string step;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //footstep = GetComponentInChildren<VisualEffect>();
        footstep.SetBool(step, false);
    }

    void TriggerFootstep()
    {
        footstep.SetBool(step, true);
        StartCoroutine(DisableFootsteps());
    }

    IEnumerator DisableFootsteps()
    {
        yield return new WaitForSeconds (.2f);
        footstep.SetBool(step, false);
    }
}
