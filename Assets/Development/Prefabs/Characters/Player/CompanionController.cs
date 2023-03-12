using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    public GameObject CompanionA;
    public GameObject CompanionB;

    private Brain followerA;
    private Brain followerB;

    private void Start()
    {
        followerA = CompanionA.GetComponent<Brain>();
        followerB = CompanionB.GetComponent<Brain>();
    }

    public void Toggle()
    {
        //followerA.isFollowing = !followerA.isFollowing;
        //followerB.isFollowing = !followerB.isFollowing;
    }
}
