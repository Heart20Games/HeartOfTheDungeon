using Body.Behavior;
using Body.Behavior.ContextSteering;
using UnityEngine;

public class EnemyAI : Brain
{
    [SerializeField] private Action currentAction;

    [SerializeField] private Transform[] wayPoints;

    [SerializeField] private float distanceToReturnHome;
    [SerializeField] private float distanceToChangeWaypoints;
    [SerializeField] private float wayPointWaitTime;
    [SerializeField] private float attackTime;

    private float wayPointTimeStep;
    private float attackTimeStep;

    private int wayPointIndex;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        PatrolState();
    }

    public override void Update()
    {
        base.Update();

        HomeDestination(wayPoints[wayPointIndex]);
        WaypointDestination(wayPoints[wayPointIndex]);

        AttackState();
    }

    public void ChasePlayer(Transform targetObject)
    {
        currentAction = Action.Chase;

        attackTimeStep = 0;

        Target = targetObject;
    }

    public void PatrolState()
    {
        Target = wayPoints[wayPointIndex];
        currentAction = Action.Patrol;

        agent.destination = Target.position;

        agent.isStopped = false;

        Patrol();
    }

    private void AttackState()
    {
        if(!Target.GetComponent<CSController>()) return;

        if(Target.GetComponent<CSController>().identity != CSIdentity.Identity.Friend) return;

        if(Target.parent.GetComponent<Body.Character>().CurrentHealth <= 0)
        {
            PatrolState();

            attackTimeStep = 0;

            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentAction = Action.Duel;

            if(character.castables[character.CastableID] != null)
            {
                character.castables[character.CastableID].GetComponent<Damager>()._Impactor = Target.GetComponent<Impact>();
            }

            attackTimeStep += Time.deltaTime;
            if(attackTimeStep >= attackTime)
            {
                Duel();

                attackTimeStep = 0;
            }
        }
        else
        {
            if(currentAction == Action.Duel)
            {
                ChasePlayer(Target);
            }
        }
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
                if (agent.remainingDistance <= distanceToChangeWaypoints)
                {
                    wayPointTimeStep += Time.deltaTime;

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
            }
        }

        return targetPosition;
    }

    public void SetWayPoints(Transform wayPointOne, Transform wayPointTwo)
    {
        wayPoints[0] = wayPointOne;
        wayPoints[1] = wayPointTwo;
    }
}