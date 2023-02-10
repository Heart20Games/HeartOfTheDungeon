using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    private AgentFollower agentFollower;
    private Transform player;
    public GameObject projectilePrefab;
    public float fireInterval = 5f;
    private float timeSinceLastFire = 0f;
    public bool isFiring = true;

    void Awake()
    {
        agentFollower = GetComponent<AgentFollower>();
        player = agentFollower.player;
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
