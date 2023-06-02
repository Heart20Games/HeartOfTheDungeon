using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : BaseMonoBehaviour
{
    public Image fill;
    public Transform cam;

    private void Start() 
    {
        cam = Camera.main.transform;
    }

   
    public void UpdateFill (float curHP, float maxHP)
    {
        fill.fillAmount = (float)curHP / (float)maxHP;
    }

    private void LateUpdate()
    {
        if (cam != null)
        {
            transform.eulerAngles = cam.transform.eulerAngles;
        }
    }

}
