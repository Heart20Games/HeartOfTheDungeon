using UnityEngine;

[System.Serializable]
public class Actions
{
    [SerializeField] private HotD.Castables.Projectile projectileToShoot;

    public HotD.Castables.Projectile ProjectileToShoot => projectileToShoot;
}

public class SlimeWizard : EnemyAI
{
    [SerializeField] private Actions[] actions;

    [SerializeField] private float attackCoolDown;

    private float coolDownTimer;
    private bool attacked;

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
                CreateProjectile();
            }
            
            IncrementAttackCoolDown();
        }
    }

    private void CreateProjectile()
    {
        int randomAttack = Random.Range(0, actions.Length);

        HotD.Castables.Caster caster = GetComponent<HotD.Castables.Caster>();

        var projectile = Instantiate(actions[0].ProjectileToShoot, transform);

        projectile.ShouldIgnoreDodgeLayer = true;

        projectile.transform.position = new Vector3(caster.WeaponLocation.position.x, caster.WeaponLocation.position.y, caster.WeaponLocation.position.z);

        projectile.direction = new Vector3(Target.transform.position.x - caster.WeaponLocation.position.x, 0, Target.transform.position.z - caster.WeaponLocation.position.z).normalized;
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