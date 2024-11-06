using Body.Behavior.ContextSteering;
using System.Collections;
using UnityEngine;

public class EnemyTriggerZone : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;

    private void Awake()
    {
        StartCoroutine(EnableTriggerZone());
    }

    private IEnumerator EnableTriggerZone()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<CSController>()) return;

        Party party = HotD.Game.main.playerParty;
        CSController controller = party.leader.transform.GetChild(0).GetComponent<CSController>();

        if(other.GetComponent<CSController>() == controller)
        {
            enemyAI.ChasePlayer(other.transform, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<CSController>()) return;

        Party party = HotD.Game.main.playerParty;
        CSController controller = party.leader.transform.GetChild(0).GetComponent<CSController>();

        if (other.GetComponent<CSController>() == controller)
        {
            enemyAI.PatrolState();
        }
    }
}