using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    public GameObject CompanionA;
    public GameObject CompanionB;

    private AIBehaviors followerA;
    private AIBehaviors followerB;

    private void Start()
    {
        followerA = CompanionA.GetComponent<AIBehaviors>();
        followerB = CompanionB.GetComponent<AIBehaviors>();
    }

    public void Toggle()
    {
        //followerA.isFollowing = !followerA.isFollowing;
        //followerB.isFollowing = !followerB.isFollowing;
    }
}
