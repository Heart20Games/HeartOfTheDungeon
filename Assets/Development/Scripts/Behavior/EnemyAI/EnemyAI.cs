using Body.Behavior;
using Body.Behavior.ContextSteering;
using HotD;
using HotD.Body;
using MyBox;
using System.Collections;
using UnityEngine;

public class EnemyAI : Brain
{
    [Foldout("Enemy AI", true)]
    [SerializeField] protected Action currentAction;

    [SerializeField] protected Transform[] wayPoints;

    [SerializeField] protected Animator animator;

    [SerializeField] protected float distanceToReturnHome;
    [SerializeField] protected float wayPointWaitTime;
    [SerializeField] protected float attackTime;

    [SerializeField] protected bool shouldPatrol;
    [SerializeField] protected bool isBoss;

    private bool didAttack;

    private float wayPointTimeStep;
    private float attackTimeStep;

    private int wayPointIndex;

    public Action CurrentAction => currentAction;

    public bool ShouldPatrol => shouldPatrol;

    public bool DidAttack
    {
        get => didAttack;
        set => didAttack = value;
    }

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if(shouldPatrol)
        {
            PatrolState();
        }
        else
        {
            Target = Game.main.playerParty.leader.transform;
            ChasePlayer(Target, true);
        }
    }

    public override void Update()
    {
        base.Update();

        if (TryGetWayPoint(wayPointIndex, out var wayPoint))
        {
            HomeDestination(wayPoint);
            WaypointDestination(wayPoint);
        }

        if(!isBoss)
        {
            AttackState();
        }
        else
        {
            BossAttack();
        }
    }

    public void ChasePlayer(Transform targetObject, bool resetAttackTime)
    {
        currentAction = Action.Chase;

        animator.SetBool("Run", true);

        if (resetAttackTime)
        {
            attackTimeStep = 0;
        }

        Target = targetObject;
    }

    public void PatrolState()
    {
        if (TryGetWayPoint(wayPointIndex, out var target))
        {
            Target = wayPoints[wayPointIndex];
            currentAction = Action.Patrol;

            agent.destination = Target.position;

            agent.isStopped = false;
            didAttack = false;
            
            Patrol();
        }
    }

    public void AttackState()
    {
        if (Target == null) return;

        if(!Target.GetComponent<CSController>()) return;

        if(Target.GetComponent<CSController>().identity != CSIdentity.Identity.Friend) return;

        if (Target.parent.GetComponent<Character>().CurrentHealth <= 0)
        {
            PatrolState();

            attackTimeStep = 0;

            return;
        }

        SetTarget(Target);

        if (agent.isOnNavMesh && agent.remainingDistance < agent.stoppingDistance)
        {
            currentAction = Action.Duel;

            animator.SetBool("Run", false);

            if (character.castables[character.CastableID] != null)
            {
                character.castables[character.CastableID].Damager._Impactor = Target.GetComponent<Impactor>();
            }

            if(!didAttack)
            {
                attackTimeStep += Time.deltaTime;
            }
            
            if(attackTimeStep >= attackTime)
            {
                didAttack = true;

                animator.SetTrigger("StartAction");
                animator.SetInteger("Action", 1);

                StartCoroutine(ResetActions());

                Duel();

                attackTimeStep = 0;
            }
        }
        else
        {
            if(currentAction == Action.Duel)
            {
                ChasePlayer(Target, true);
            }
        }
    }

    public Impactor GetImpactor()
    {
        return character.castables[character.CastableID].Damager._Impactor;
    }

    private IEnumerator ResetActions()
    {
        yield return new WaitForSeconds(attackTime);
        animator.ResetTrigger("StartAction");
        animator.SetInteger("Action", 0);

        didAttack = false;
    }

    public void BossAttack()
    {
        if (Target == null) return;

        if (!Target.GetComponent<CSController>()) return;

        if (Target.GetComponent<CSController>().identity != CSIdentity.Identity.Friend) return;

        if (Target.parent.GetComponent<Character>().CurrentHealth <= 0)
        {
            PatrolState();

            attackTimeStep = 0;

            return;
        }

        SetTarget(Target);

        attackTimeStep += Time.deltaTime;

        if (IsAttacking())
        {
            if(!didAttack)
            {
                didAttack = true;

                currentAction = Action.Duel;
                agent.isStopped = true;
            }
        }
        else
        {
            ChasePlayer(Target, false);
            agent.isStopped = false;
        }
    }

    public bool IsInAttackingDistance()
    {
        return agent.remainingDistance <= agent.stoppingDistance;
    }

    public bool IsAttacking()
    {
        return attackTimeStep >= attackTime;
    }

    public float ResetAttackTime()
    {
        attackTimeStep = 0;

        return attackTimeStep;
    }

    //Checks to see if the object is too far away from a waypoint. In which case they will stop pursuing the player.
    private Transform HomeDestination(Transform targetPosition)
    {
        if (currentAction != Action.Patrol && pathFinder.target != null)
        {
            Vector3 distance = new Vector3(targetPosition.position.x - agent.transform.localPosition.x, agent.transform.localPosition.y, targetPosition.position.z - agent.transform.localPosition.z);

            if(Vector3.Distance(targetPosition.position, distance) >= distanceToReturnHome)
            {
                PatrolState();
            }
        }

        return targetPosition;
    }

    //Checks to see if the object is close enough to a waypoint before moving on to the next.
    private Transform WaypointDestination(Transform targetPosition)
    {
        if(agent != null)
        {
            if (currentAction == Action.Patrol && pathFinder.target != null)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    wayPointTimeStep += Time.deltaTime;

                    animator.SetBool("Run", false);

                    if (wayPointTimeStep >= wayPointWaitTime)
                    {
                        wayPointIndex++;
                        if (wayPointIndex >= wayPoints.Length)
                        {
                            wayPointIndex = 0;
                        }

                        PatrolState();

                        wayPointTimeStep = 0;
                    }
                }
                else
                {
                    animator.SetBool("Run", true);
                }
            }
        }

        return targetPosition;
    }

    private bool TryGetWayPoint(int wayPointIndex, out Transform wayPoint)
    {
        wayPoint = null;
        if (wayPoints != null && wayPoints.Length > wayPointIndex)
        {
            wayPoint = wayPoints[wayPointIndex];
            return wayPoint != null;
        }
        else return false;
    }

    public void SetWayPoints(Transform wayPointOne, Transform wayPointTwo)
    {
        wayPoints[0] = wayPointOne;
        wayPoints[1] = wayPointTwo;
    }
}