using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{
    public Image fill;
    public Transform cam;
    public GameObject player;

    private void Start() 
    {
        player = GameObject.Find("/Player V2/Body");
        cam = player.transform.Find("Main Camera");    
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
