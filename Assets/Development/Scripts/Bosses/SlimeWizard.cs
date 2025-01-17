using HotD.Body;
using HotD.Castables;
using MyBox;
using System.Collections;
using UnityEngine;

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

    [SerializeField] private MagicBolt_ChargingVFXScript magicBoltVfx;

    [SerializeField] private DodgeZone dodgeZone;

    [SerializeField] private ArtRenderer artRenderer;

    [SerializeField] private Rigidbody myRigidBody;

    [SerializeField] private Animator slimeWizardAnimator;

    [SerializeField] private Transform firingPoint;

    private CastedCollider magicLaserObject;

    [SerializeField] private float attackCoolDown;
    [SerializeField] private float laserTrackingSpeed;

    private Coroutine magicBoltRoutine;
    private Coroutine laserRoutine;

    private float coolDownTimer;

    [SerializeField][ReadOnly] private bool attacked;
    [SerializeField][ReadOnly] private bool isShootingLaser;
    [SerializeField][ReadOnly] private bool chargingLevelOne;
    [SerializeField][ReadOnly] private bool chargingLevelTwo;

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

        magicBoltVfx.SetTriggers(Coordination.Triggers.StartCast);

        slimeWizardAnimator.ResetTrigger("StartCast");
        slimeWizardAnimator.SetTrigger("StartAction");

        switch (attackIndex)
        {
            case 0:
                slimeWizardAnimator.SetInteger("ChargeLevel", 1);
                slimeWizardAnimator.SetFloat("Action", 1);
                chargingLevelOne = true;
                break;
            case 1:
                slimeWizardAnimator.SetInteger("ChargeLevel", 1);
                slimeWizardAnimator.SetFloat("Action", 1);
                chargingLevelTwo = true;
                break;
            case 2:
                slimeWizardAnimator.SetInteger("ChargeLevel", 1);
                slimeWizardAnimator.SetFloat("Action", 1);
                break;
            case 3:
                slimeWizardAnimator.SetInteger("ChargeLevel", 1);
                slimeWizardAnimator.SetFloat("Action", 1);
                chargingLevelOne = true;
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

        var magicAttack = Instantiate(actions[attackIndex].ProjectileToShoot);

        if(magicAttack.GetComponent<Projectile>())
        {
            Projectile projectile = magicAttack.GetComponent<Projectile>();

            projectile.SetTarget(target);
            projectile.SetSource(firingPoint);

            projectile.AddException(character.AliveCollider);

            projectile.ShouldIgnoreDodgeLayer = true;

            magicAttack.transform.position = firingPoint.position;
            magicAttack.transform.position += firingPoint.forward * 1.2f;
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

        chargingLevelOne = false;
        chargingLevelTwo = false;

        magicBoltVfx.SetTriggers(Coordination.Triggers.EndCast);

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
        magicBoltVfx.SetTriggers(Coordination.Triggers.EndCast);

        chargingLevelOne = false;
        chargingLevelTwo = false;

        slimeWizardAnimator.SetFloat("Action", 0);
        slimeWizardAnimator.SetInteger("ChargeLevel", 0);

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