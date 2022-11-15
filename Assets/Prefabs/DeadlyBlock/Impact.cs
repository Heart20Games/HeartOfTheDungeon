using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{

    public string targetNode;
    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colliding");
        if(collision.gameObject.name == "Player")
        {
            PlayerCore player = collision.gameObject.GetComponent<PlayerCore>();
            player.talkable = true;
            Debug.Log("I can talk");
            player.targetNode = this.targetNode;
            // player.Die();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            PlayerCore player = collision.gameObject.GetComponent<PlayerCore>();
            player.talkable = false;
            player.targetNode = "";
            Debug.Log("Can't talk anymore");
        }
    }
}
