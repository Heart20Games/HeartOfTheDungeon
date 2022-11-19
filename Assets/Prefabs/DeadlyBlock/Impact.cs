using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    public string targetNode;
    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            PlayerCore player = collision.gameObject.GetComponent<PlayerCore>();
            player.talkable = true;
            player.targetNode = this.targetNode;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            PlayerCore player = collision.gameObject.GetComponent<PlayerCore>();
            player.talkable = false;
            player.targetNode = "";
        }
    }
}
