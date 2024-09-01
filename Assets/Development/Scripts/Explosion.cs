using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

public class Explosion: BaseMonoBehaviour
{
    public Identity identity = Identity.Neutral;
    public float radius = 5f;
    public float force = 10f;
    public int damage = 10;

    public bool triggerExplosion = false;

    public bool debug;
    public float debugRayDuration = 0.01f;
    
    private void Update()
    {
        if (triggerExplosion)
        {
            triggerExplosion = false;
            Explode();
        }
    }

    public void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector3.forward * radius, Color.yellow, debugRayDuration);
        Debug.DrawRay(transform.position, Vector3.right * radius, Color.yellow, debugRayDuration);
        Debug.DrawRay(transform.position, Vector3.back * radius, Color.yellow, debugRayDuration);
        Debug.DrawRay(transform.position, Vector3.left * radius, Color.yellow, debugRayDuration);
    }

    public void Explode()
    {
        RaycastHit[] hits = Physics.SphereCastAll(new(transform.position, Vector3.forward), radius);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider.TryGetComponent(out Rigidbody rigidbody))
            {
                Print($"Push them! {hit.collider.gameObject.name}", debug);
                rigidbody.AddExplosionForce(force, transform.position, radius);
            }
            if (hit.collider.TryGetComponent(out IDamageReceiver damageable))
            {
                Print($"Damage them! {hit.collider.gameObject.name}", debug);
                damageable.SetDamagePosition(hit.point);
                damageable.TakeDamage(damage, identity);
            }
        }
    }
}
