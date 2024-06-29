using HotD.Castables;
using System.Collections;
using UnityEngine;

public class DodgeZone : MonoBehaviour
{
    [SerializeField] private Transform characterTransform;

    [SerializeField] private MagicShieldImpact magicShieldPrefab;

    [SerializeField] private int dodgeChance;

    private Coroutine sidestepRoutine;
    private Coroutine magicShieldRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Projectile>())
        {
            if(other.GetComponent<Projectile>().ShouldIgnoreDodgeLayer) return;

            CalculateDodgeChance();
        }
    }

    private void CalculateDodgeChance()
    {
        int randomChance = Random.Range(0, 100);

        if (randomChance <= dodgeChance)
        {
            ChooseDodgeManeuver();
        }
    }

    private void ChooseDodgeManeuver()
    {
        if(sidestepRoutine != null) return;
        if(magicShieldRoutine != null) return;

        int randomChance = Random.Range(0, 2);

        if (randomChance == 0)
        {
            if (sidestepRoutine == null)
            {
                sidestepRoutine = StartCoroutine(SidestepDodge());
            }
        }
        else
        {
            if(magicShieldRoutine == null)
            {
                magicShieldRoutine = StartCoroutine(MagicShieldDodge());
            }
        }
    }

    private IEnumerator MagicShieldDodge()
    {
        var magicShield = Instantiate(magicShieldPrefab);

        magicShield.transform.parent = characterTransform;
        magicShield.transform.localScale = new Vector3(-2, 2, 2);
        magicShield.transform.localPosition = new Vector3(0, 3.157f, 0);

        magicShield.ToggleShield(true);

        yield return new WaitForSeconds(3);

        magicShield.ToggleShield(false);

        yield return new WaitForSeconds(1f);

        magicShieldRoutine = null;
        Destroy(magicShield.gameObject);
    }

    private IEnumerator SidestepDodge()
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

        sidestepRoutine = null;
    }
}