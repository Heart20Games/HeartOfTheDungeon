using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    public GameObject CompanionA;
    public GameObject CompanionB;

    private AgentFollower followerA;
    private AgentFollower followerB;

    private void Start()
    {
        followerA = CompanionA.GetComponent<AgentFollower>();
        followerB = CompanionB.GetComponent<AgentFollower>();
    }

    public void Toggle()
    {
        //followerA.isFollowing = !followerA.isFollowing;
        //followerB.isFollowing = !followerB.isFollowing;
    }
}
