using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBack : MonoBehaviour
{
    public Transform bouncee;
    public Transform destination;

    public void Bounce()
    {
        bouncee.position = destination.position;
    }
}
