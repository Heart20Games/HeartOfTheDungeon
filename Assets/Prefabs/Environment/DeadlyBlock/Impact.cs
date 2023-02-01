using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    public string targetNode;
    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        PlayerCore playerCore = collision.gameObject.GetComponent<PlayerCore>();
        if(playerCore != null)
        {
            playerCore.FoundTalkable(targetNode);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        PlayerCore playerCore = collision.gameObject.GetComponent<PlayerCore>();
        if (playerCore != null)
        {
            playerCore.LeftTalkable(targetNode);
        }
    }
}
