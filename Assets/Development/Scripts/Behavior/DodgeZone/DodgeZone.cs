using HotD.Castables;
using System.Collections;
using UnityEngine;

public class DodgeZone : MonoBehaviour
{
    [SerializeField] private SlimeWizard slimeWizard;

    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform magicShieldParent;

    [SerializeField] private Animator slimeAnimator;

    [SerializeField] private MagicShieldImpact magicShieldPrefab;

    private MagicShieldImpact magicShieldImpactObject;

    [Tooltip("Percent Chance of Dodging.")]
    [Range(0f, 100f)]
    [SerializeField] private int dodgeChance;

    [SerializeField] private float sideStepSpeed;

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
            sideStepRoutine ??= StartCoroutine(SidestepDodge());
        }
        else
        {
            magicShieldRoutine ??= StartCoroutine(MagicShieldDodge());
        }
    }

    private IEnumerator MagicShieldDodge()
    {
        var magicShield = Instantiate(magicShieldPrefab, magicShieldParent);

        magicShieldImpactObject = magicShield;

        magicShield.transform.localScale = new Vector3(-6f, 6f, 6f);
        magicShield.transform.localPosition = new Vector3(0, 6.0f, 0);

        magicShield.ToggleShield(true);

        yield return new WaitForSeconds(3);

        magicShield.ToggleShield(false);

        yield return new WaitForSeconds(1f);

        magicShieldRoutine = null;
        if(magicShieldImpactObject != null)
        {
            magicShieldImpactObject = null;
        }
        Destroy(magicShield.gameObject);
    }

    public void RemoveShieldEffect()
    {
        if(magicShieldRoutine != null)
        {
            StopCoroutine(magicShieldRoutine);
            magicShieldRoutine = null;
        }

        if (magicShieldImpactObject != null)
        {
            Destroy(magicShieldImpactObject.gameObject);
            magicShieldImpactObject = null;
        }
    }

    private IEnumerator SidestepDodge()
    {
        float t = 0;

        int randNum = Random.Range(0, 2);

        bool hitObstacle = false;

        RaycastHit hit;

        slimeAnimator.SetBool("Dodge", true);

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

            characterTransform.position += randNum == 0 ? characterTransform.right * sideStepSpeed * Time.deltaTime : -characterTransform.right * sideStepSpeed * Time.deltaTime;

            yield return null;
        }

        slimeAnimator.SetBool("Dodge", false);

        sideStepRoutine = null;
    }
}