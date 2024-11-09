using HotD.Body;
using HotD.Castables;
using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class Actions
{
    [SerializeField] private string name = "Action";
    [SerializeField] private GameObject projectileToShoot;

    [SerializeField] private int castChance;

    public GameObject ProjectileToShoot => projectileToShoot;

    public int CastChance => castChance;
}

public class SlimeWizard : EnemyAI
{
    [Foldout("Slime Wizard", true)]
    [SerializeField] private Actions[] actions;

    [SerializeField] private Transform slimeTransform;

    [SerializeField] private VisualEffect magicBoltVfx;

    [SerializeField] private DodgeZone dodgeZone;

    [SerializeField] private ArtRenderer artRenderer;

    [SerializeField] private Rigidbody myRigidBody;

    [SerializeField] private Animator magicBoltVfxAnimator;
    [SerializeField] private Animator slimeWizardAnimator;

    private CastedCollider magicLaserObject;

    [SerializeField] private float attackCoolDown;
    [SerializeField] private float laserTrackingSpeed;

    private Coroutine magicBoltRoutine;
    private Coroutine laserRoutine;

    private float coolDownTimer;

    private bool attacked;
    private bool isShootingLaser;
    private bool chargingLevelOne;
    private bool chargingLevelTwo;

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

    private void FixedUpdate()
    {
        WalkAnimation();
    }

    private void WalkAnimation()
    {
        if(agent.remainingDistance > agent.stoppingDistance && !agent.isStopped)
        {
            slimeWizardAnimator.SetBool("Run", true);

            artRenderer.RunVelocity = 3;

            artRenderer.Running = true;
        }
        else
        {
            slimeWizardAnimator.SetBool("Run", false);

            myRigidBody.velocity = Vector3.zero;
            artRenderer.RunVelocity = 0;

            artRenderer.Running = false;
        }
    }

    public void DeadAnimation()
    {
        StopEffectsOnDeath();
    }

    private IEnumerator CreateProjectile()
    {
        int weightedAttack = Random.Range(0, 100);
        int attackIndex = 0;

        for(int i = 0; i < actions.Length; i++)
        {
            if(weightedAttack >= actions[i].CastChance)
            {
                attackIndex = i;
                break;
            }
        }

        Caster caster = GetComponent<Caster>();

        magicBoltVfx.gameObject.SetActive(true);
        magicBoltVfx.Play();

        magicBoltVfxAnimator.SetBool("Sustain", true);
        slimeWizardAnimator.ResetTrigger("StartCast");
        slimeWizardAnimator.SetTrigger("StartAction");

        switch (attackIndex)
        {
            case 0:
                magicBoltVfxAnimator.Play("Level 1 Charge");
                slimeWizardAnimator.SetInteger("ChargeLevel", 1);
                slimeWizardAnimator.SetFloat("Action", 1);
                chargingLevelOne = true;
                //CallOutManager.instance.PlayPartyMemeberCallOut(0);
                break;
            case 1:
                magicBoltVfxAnimator.Play("Level 1 Charge");
                magicBoltVfxAnimator.SetBool("Level 2 Available", true);
                slimeWizardAnimator.SetInteger("ChargeLevel", 1);
                slimeWizardAnimator.SetFloat("Action", 1);
                chargingLevelTwo = true;
                //CallOutManager.instance.PlayPartyMemeberCallOut(1);
                break;
            case 2:
                magicBoltVfxAnimator.Play("Level 1 Charge");
                magicBoltVfxAnimator.SetBool("Level 3 Available", true);
                slimeWizardAnimator.SetInteger("ChargeLevel", 1);
                slimeWizardAnimator.SetFloat("Action", 1);
                //CallOutManager.instance.PlayPartyMemeberCallOut(2);
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
            yield return new WaitForSeconds(2f);
            slimeWizardAnimator.SetInteger("ChargeLevel", 2);
            yield return new WaitForSeconds(2f);
            slimeWizardAnimator.SetInteger("ChargeLevel", 3);
            yield return new WaitForSeconds(2f);
        }

        slimeWizardAnimator.SetTrigger("StartCast");

        var magicAttack = Instantiate(actions[attackIndex].ProjectileToShoot, transform);

        if(magicAttack.GetComponent<Projectile>())
        {
            Projectile projectile = magicAttack.GetComponent<Projectile>();

            projectile.AddException(character.AliveCollider);

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
            Level3BoltScaling boltScaling = castedCollider.transform.GetChild(0).GetComponent<Level3BoltScaling>();

            boltScaling.ShouldFollowCrossHair = false;

            magicLaserObject = castedCollider;

            castedCollider.onCast.Invoke(new Vector3(0, transform.position.y, 0));

            if(laserRoutine == null)
            {
                laserRoutine = StartCoroutine(LaserRoutine(Target));
            }
        }

        magicBoltVfxAnimator.SetBool("Sustain", false);

        chargingLevelOne = false;
        chargingLevelTwo = false;

        magicBoltVfx.Stop();

        magicBoltRoutine = null;
    }

    private IEnumerator LaserRoutine(Transform targetToFollow)
    {
        float t = 0;

        while(t < 4)
        {
            t += Time.deltaTime;

            Vector3 dir = targetToFollow.position - slimeTransform.position;
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir);
            slimeTransform.rotation = Quaternion.Slerp(slimeTransform.rotation, rot, laserTrackingSpeed * Time.deltaTime);

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

    public void StopEffectsOnDeath()
    {
        magicBoltVfx.Stop();

        chargingLevelOne = false;
        chargingLevelTwo = false;

        slimeWizardAnimator.SetFloat("Action", 0);
        slimeWizardAnimator.SetInteger("ChargeLevel", 0);

        magicBoltVfxAnimator.SetBool("Sustain", false);

        dodgeZone.RemoveShieldEffect();

        if (magicBoltRoutine != null)
        {
            StopCoroutine(magicBoltRoutine);
            magicBoltRoutine = null;
        }

        if(laserRoutine != null)
        {
            StopCoroutine(laserRoutine);

            if(magicLaserObject != null)
            {
                for(int i = 0; i < magicLaserObject.transform.childCount; i++)
                {
                    if (magicLaserObject.transform.GetChild(i).GetComponent<Level3BoltScaling>())
                    {
                        Level3BoltScaling boltScaling = magicLaserObject.transform.GetChild(i).GetComponent<Level3BoltScaling>();
                        boltScaling.WindDown();

                        break;
                    }
                }

                magicLaserObject = null;
            }

            isShootingLaser = false;
            laserRoutine = null;
        }

        slimeWizardAnimator.SetBool("Dead", true);
    }
}