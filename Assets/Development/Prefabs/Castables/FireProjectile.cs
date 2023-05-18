using UnityEngine;
using Body.Behavior;

public class FireProjectile : BaseMonoBehaviour
{
    private Brain agentFollower;
    private Transform player;
    public GameObject projectilePrefab;
    public float fireInterval = 5f;
    private float timeSinceLastFire = 0f;
    public bool isFiring = true;

    void Awake()
    {
        agentFollower = GetComponent<Brain>();
        player = agentFollower.Target;
    }

    void Update()
    {
        timeSinceLastFire += Time.deltaTime;
        if (isFiring)
        {
            if (timeSinceLastFire >= fireInterval)
            {
                Fire();
                timeSinceLastFire = 0f;
            }
        }
    }

    public void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        Vector3 direction = (player.position - transform.position).normalized;
        rb.AddForce(direction * 500f);
    }
}
