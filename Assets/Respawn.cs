
using UnityEngine;
using UnityEngine.Events;

public class Respawn : BaseMonoBehaviour
{
    private Vector3 spawnPoint;
    private Quaternion spawnRotation;
    private Vector3 spawnScale;

    public UnityEvent onRespawn;

    private void Awake()
    {
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;
        spawnScale = transform.localScale;
    }

    public void RespawnTrigger()
    {
        transform.position = spawnPoint;
        transform.rotation = spawnRotation;
        transform.localScale = spawnScale;
        onRespawn.Invoke();
    }
}
