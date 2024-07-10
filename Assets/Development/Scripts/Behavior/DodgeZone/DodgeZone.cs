using HotD.Castables;
using System.Collections;
using UnityEngine;

public class DodgeZone : MonoBehaviour
{
    [SerializeField] private SlimeWizard slimeWizard;

    [SerializeField] private Transform characterTransform;

    [SerializeField] private MagicShieldImpact magicShieldPrefab;

    [SerializeField] private int dodgeChance;

    private Coroutine sideStepRoutine;
    private Coroutine magicShieldRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (slimeWizard.IsShootingLaser) return;

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
        if(sideStepRoutine != null) return;
        if(magicShieldRoutine != null) return;

        int randomChance = Random.Range(0, 2);

        if (randomChance == 0)
        {
            if (sideStepRoutine == null)
            {
                sideStepRoutine = StartCoroutine(SidestepDodge());
            }
        }
        else
        {
            if (magicShieldRoutine == null)
            {
                magicShieldRoutine = StartCoroutine(MagicShieldDodge());
            }
        }
    }

    private IEnumerator MagicShieldDodge()
    {
        var magicShield = Instantiate(magicShieldPrefab);

        magicShield.transform.parent = characterTransform;
        magicShield.transform.localScale = new Vector3(-2.5f, 2.5f, 2.5f);
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

        int randNum = Random.Range(0, 2);

        bool hitObstacle = false;

        RaycastHit hit;

        while (t < 0.5f)
        {
            t += Time.deltaTime;

            //Draws a ray to check if there are any obstacles nearby so that the wizard doesn't sidestep into a wall.
            if(!hitObstacle)
            {
                if (Physics.Raycast(characterTransform.position, randNum == 0 ? characterTransform.right : -characterTransform.right, out hit, 10))
                {
                    if (randNum == 0)
                    {
                        randNum = 1;
                    }
                    else
                    {
                        randNum = 0;
                    }
                }

                hitObstacle = true;
            }

            characterTransform.position += randNum == 0 ? characterTransform.right * 10 * Time.deltaTime : -characterTransform.right * 10 * Time.deltaTime;

            yield return null;
        }

        sideStepRoutine = null;
    }
}