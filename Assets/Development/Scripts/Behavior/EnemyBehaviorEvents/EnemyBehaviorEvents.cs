using HotD.Body;
using UnityEngine;

public class EnemyBehaviorEvents : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;

    private Damager meleeDamager;

    private void Start()
    {
        meleeDamager = enemyAI.GetComponent<Character>().castables[0]?.Damager;
    }

    public void Melee(string castKey)
    {
        if (string.IsNullOrEmpty(castKey)) return;
        if (enemyAI.Target == null) return;
        if (meleeDamager == null) return;
        if (!enemyAI.IsInAttackingDistance()) return;

        meleeDamager.DamageTarget();
    }
}