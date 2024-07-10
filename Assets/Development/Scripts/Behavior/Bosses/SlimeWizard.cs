using HotD.Castables;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class Actions
{
    [SerializeField] private GameObject projectileToShoot;

    public GameObject ProjectileToShoot => projectileToShoot;
}

public class SlimeWizard : EnemyAI
{
    [SerializeField] private Actions[] actions;

    [SerializeField] private Transform slimeTransform;

    [SerializeField] private VisualEffect magicBoltVfx;

    [SerializeField] private Animator magicBoltVfxAnimator;

    [SerializeField] private float attackCoolDown;

    private Coroutine magicBoltRoutine;
    private Coroutine laserRoutine;

    private float coolDownTimer;

    private bool attacked;
    private bool isShootingLaser;
    private bool chargingLevelOne;
    private bool chargingLevelTwo;
    private bool chargingLevelThree;

    public bool IsShootingLaser => isShootingLaser;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        if (character.CurrentHealth <= 0) return;

        base.Update();

        if(DidAttack)
        {
            if(!attacked)
            {
                attacked = true;

                if(magicBoltRoutine == null)
                {
                    magicBoltRoutine = StartCoroutine(CreateProjectile());
                }
            }
            
            if(!isShootingLaser && magicBoltRoutine == null)
            {
                IncrementAttackCoolDown();
            }
        }
    }

    private IEnumerator CreateProjectile()
    {
        int randomAttack = Random.Range(0, actions.Length);

        Caster caster = GetComponent<Caster>();

        magicBoltVfx.gameObject.SetActive(true);
        magicBoltVfx.Play();

        magicBoltVfxAnimator.SetBool("Sustain", true);

        switch(randomAttack)
        {
            case 0:
                magicBoltVfxAnimator.Play("Level 1 Charge");
                chargingLevelOne = true;
                break;
            case 1:
                magicBoltVfxAnimator.Play("Level 1 Charge");
                magicBoltVfxAnimator.SetBool("Level 2 Available", true);
                chargingLevelTwo = true;
                break;
            case 2:
                magicBoltVfxAnimator.Play("Level 1 Charge");
                magicBoltVfxAnimator.SetBool("Level 3 Available", true);
                chargingLevelThree = true;
                break;
        }

        if(chargingLevelOne)
        {
            yield return new WaitForSeconds(0.7f);
        }
        else if(chargingLevelTwo)
        {
            yield return new WaitForSeconds(3.5f);
        }
        else
        {
            yield return new WaitForSeconds(6f);
        }

        var magicAttack = Instantiate(actions[randomAttack].ProjectileToShoot, transform);

        if(magicAttack.GetComponent<Projectile>())
        {
            Projectile projectile = magicAttack.GetComponent<Projectile>();

            projectile.ShouldIgnoreDodgeLayer = true;

            magicAttack.transform.position = caster.WeaponLocation.position;
            magicAttack.transform.position += caster.WeaponLocation.forward * 1.2f;

            projectile.direction = new Vector3(Target.transform.position.x - caster.WeaponLocation.position.x, 0, Target.transform.position.z - caster.WeaponLocation.position.z).normalized;
        }
        else
        {
            isShootingLaser = true;

            magicAttack.transform.SetParent(slimeTransform, false);
            magicAttack.transform.position = new Vector3(magicAttack.transform.position.x, magicAttack.transform.position.y + 1f, magicAttack.transform.position.z);

            CastedCollider castedCollider = magicAttack.GetComponent<CastedCollider>();

            castedCollider.onCast.Invoke(new Vector3(0, transform.position.y, 0));

            if(laserRoutine == null)
            {
                laserRoutine = StartCoroutine(LaserRoutine());
            }
        }

        magicBoltVfxAnimator.SetBool("Sustain", false);

        chargingLevelOne = false;
        chargingLevelTwo = false;
        chargingLevelThree = false;

        magicBoltVfx.Stop();

        magicBoltRoutine = null;
    }

    private IEnumerator LaserRoutine()
    {
        float t = 0;

        while(t < 4)
        {
            t += Time.deltaTime;

            slimeTransform.Rotate(0, 0.3f, 0, Space.World);

            yield return null;
        }

        laserRoutine = null;
        isShootingLaser = false;
    }

    private void IncrementAttackCoolDown()
    {
        coolDownTimer += Time.deltaTime;
        if(coolDownTimer >= attackCoolDown)
        {
            ResetAttackTime();
            DidAttack = false;
            attacked = false;
            coolDownTimer = 0;
        }
    }
}