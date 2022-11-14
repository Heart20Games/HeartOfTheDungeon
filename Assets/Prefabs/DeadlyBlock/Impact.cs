using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colliding");
        if(collision.gameObject.name == "Player")
        {
            PlayerCore player = collision.gameObject.GetComponent<PlayerCore>();
            player.Die();
        }
    }
}
