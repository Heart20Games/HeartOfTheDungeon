using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public PlayerCore player;
    public Animator animator;
    private Weapon weapon;

    public string weaponTag = "Weapon";
    private int health = 1;

    private float timeTillAttack;
    private float timeTillMove;
    private float timeMoving;

    private bool hasFootsteps = false;

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
        timeTillAttack -= Time.deltaTime;
        if (timeTillAttack <= 0)
        {
            timeTillAttack = enemyType.attackCooldown + timeTillAttack;
            Attack();
        }

        if (timeTillMove == 0)
        {
            timeMoving += Time.deltaTime;
            Pursue();
            if (timeMoving >= enemyType.moveTime)
            {
                Debug.Log("Stop Walking");
                animator.SetBool("walk", false);
                timeTillMove = enemyType.moveCooldown;
                timeMoving = 0;
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

    public void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Something's Touching Me...");
        Debug.Log(collider.gameObject.tag);
        Debug.Log(collider.gameObject.name);
        Weapon weapon = collider.attachedRigidbody.GetComponent<Weapon>();
        if (weapon != null && weapon.CompareTag("Player"))
        //if (collision.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("Took a while swinging");
            if (weapon.HitEnemy(this))
            {
                Debug.Log("Ouch, I've been hit!");
                Debug.Log(enemyType.hurtSound);
                enemyType.hurtSound.start();
                health -= 1;
                if (health <= 0)
                {
                    Die();
                }
            }
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
                enemyType.fightSound.start();
                Vector3 pDirection = diff.normalized;
                weapon.Swing(pDirection);
            }
        }
    }

    public void Pursue()
    {
        GetComponent<Rigidbody>().MovePosition(Vector3.MoveTowards(transform.position, player.transform.position, enemyType.moveSpeed * Time.deltaTime));
            
        if (!hasFootsteps)
        {
            hasFootsteps = true;
            enemyType.walkSound.start();
        }
        else if (hasFootsteps)
        {
            hasFootsteps = false;
            enemyType.walkSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
