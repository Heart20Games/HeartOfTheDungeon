using UnityEngine;

public class SmoothLookAt : MonoBehaviour
{
    public Transform target;
    public float damping;

    void LateUpdate()
    {
        var rotation = Quaternion.LookRotation(target.position - transform.position);
        // rotation.x = 0; This is for limiting the rotation to the y axis. I needed this for my project so just
        // rotation.z = 0;                 delete or add the lines you need to have it behave the way you want.
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }
}
