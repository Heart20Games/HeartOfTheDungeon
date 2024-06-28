using HotD.Castables;
using MyBox;
using System.Collections;
using UnityEngine;

public class DodgeZone : MonoBehaviour
{
    private Coroutine dodgeRoutine;

    [SerializeField] private Transform characterTransform;

    [SerializeField] private int dodgeChance;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Projectile>())
        {
            if (other.GetComponent<Projectile>().ShouldIgnoreDodgeLayer) return;

            int randomChance = Random.Range(0, 100);

            if (randomChance <= dodgeChance)
            {
                if(dodgeRoutine == null)
                {
                    dodgeRoutine = StartCoroutine(DodgeAttack());
                }
            }
        }
    }

    private IEnumerator DodgeAttack()
    {
        float t = 0;

        int randNum = Random.Range(0, 1);

        while (t < 0.5f)
        {
            t += Time.deltaTime;

            characterTransform.position = new Vector3(randNum == 0 ? characterTransform.position.x + 10 * Time.deltaTime : characterTransform.position.x - 10 * Time.deltaTime, characterTransform.position.y, 
                                                      characterTransform.position.z);

            yield return null;
        }

        dodgeRoutine = null;
    }
}