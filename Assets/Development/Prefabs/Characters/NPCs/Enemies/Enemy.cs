using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Enemy : BaseMonoBehaviour
{
    public EnemyType enemyType;
    public PlayerCore player;
    public Animator animator;
    public Transform pivot;
    private Weapon weapon;

    public string weaponTag = "Weapon";
    private int health = 1;

    private float timeTillAttack;
    private float timeTillMove;
    private float timeMoving;

    private bool hasFootsteps = false;
    private bool pursuing = false;

    public void Start()
    {
        timeTillAttack = enemyType.attackCooldown;
        timeTillMove = enemyType.moveCooldown;
        timeMoving = 0;
        health = enemyType.maxHealth;
        enemyType.Initialize();
        if (enemyType.weapon != null)
        {
            weapon = Instantiate(enemyType.weapon, gameObject.transform);
        }
    }

    public void Update()
    {
        pursuing = false;

        timeTillAttack -= Time.deltaTime;
        if (timeTillAttack <= 0)
        {
            timeTillAttack = enemyType.attackCooldown + timeTillAttack;
            Attack();
        }

        if (timeTillMove == 0)
        {
            timeMoving += Time.deltaTime;
            pursuing = true;
            if (timeMoving >= enemyType.moveTime)
            {
                Debug.Log("Stop Walking");
                animator.SetBool("walk", false);
                timeTillMove = enemyType.moveCooldown;
                timeMoving = 0;
                if (enemyType.useWalkSound && hasFootsteps)
                {
                    Debug.Log("Fin Noise!");
                    hasFootsteps = false;
                    enemyType.walkSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
            }
        }
        else
        {
            timeTillMove -= Time.deltaTime;
            if (timeTillMove <= 0)
            {
                Debug.Log("Start Walking");
                animator.SetBool("walk", true);
                timeTillMove = 0;
                timeMoving = 0;
            }
        }
    }

    public void FixedUpdate()
    {
        if (pursuing)
        {
            Pursue();
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Something's Touching Me...");
        Debug.Log(collider.gameObject.tag);
        Debug.Log(collider.gameObject.name);
        Rigidbody rigidbody = collider.attachedRigidbody;
        Weapon opponentWeapon = null;
        if (rigidbody != null)
        {
            opponentWeapon = rigidbody.GetComponent<Weapon>();
        }
        if (opponentWeapon != null && opponentWeapon.CompareTag("Player"))
        //if (collision.gameObject.CompareTag("Weapon"))
        {
            //Debug.Log("Took a while swinging");
            //if (weapon.HitEnemy(this))
            //{
            //    Debug.Log("Ouch, I've been hit!");
            //    Debug.Log(enemyType.hurtSound);
            //    health -= opponentWeapon.damage;
            //    if (enemyType.useHurtSound)
            //    {
            //        enemyType.hurtSound.start();
            //    }
            //    if (health <= 0)
            //    {
            //        Die();
            //    }
            //}
        }
    }

    public void Die()
    {
        Debug.Log("Whyyyyy?!");
        Destroy(gameObject);
    }

    public void Attack()
    {
        if (weapon != null)
        {
            Vector3 diff = transform.position - player.transform.position;
            if (diff.magnitude <= enemyType.attackDistance)
            {
                Debug.Log("I'll get you!");
                if (enemyType.useFightSound)
                {
                    enemyType.fightSound.start();
                }
                Vector3 pDirection = diff.normalized;
                weapon.Cast(pDirection);
            }
        }
    }

    public void Pursue()
    {
        Vector3 diff = transform.position - player.transform.position;
        if (diff.magnitude >= enemyType.attackDistance/2)
        {
            Vector3 destination = Vector3.MoveTowards(transform.position, player.transform.position, enemyType.moveSpeed * Time.fixedDeltaTime);
            GetComponent<Rigidbody>().MovePosition(destination);

            Vector3 direction = (transform.position - destination).normalized;
            float xDir = Mathf.Sign(transform.InverseTransformDirection(direction).x);
            float xScale = Mathf.Abs(pivot.localScale.x) * xDir;
            //transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
            pivot.localScale = new Vector3(xScale, pivot.localScale.y, pivot.localScale.z);

            if (enemyType.useWalkSound && !hasFootsteps)
            {
                Debug.Log("Make Noise!");
                hasFootsteps = true;
                enemyType.walkSound.start();
            }
        }
        else if (enemyType.useWalkSound && hasFootsteps)
        {
            Debug.Log("Stop Noise!");
            hasFootsteps = false;
            enemyType.walkSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
