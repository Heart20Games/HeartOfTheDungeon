using Body.Behavior.ContextSteering;
using UnityEngine;

public class EnemyTriggerZone : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<CSController>()) return;

        if(other.GetComponent<CSController>().identity == CSIdentity.Identity.Friend)
        {
            enemyAI.ChasePlayer(other.transform, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<CSController>()) return;

        if (other.GetComponent<CSController>().identity == CSIdentity.Identity.Friend)
        {
            enemyAI.PatrolState();
        }
    }
}