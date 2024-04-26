using Body.Behavior;
using HotD.Castables;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnemyAI : Brain
{
    [SerializeField] private Action currentAction;

    [SerializeField] private Transform[] wayPoints;

    [SerializeField] private float distanceToReturnHome;
    [SerializeField] private float distanceToChangeWaypoints;
    [SerializeField] private float wayPointWaitTime;

    private float wayPointTimeStep;

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
        if (currentAction != Action.Chase) return;

        if (!Target.parent.GetComponent<Body.Character>().Alive)
        {
            PatrolState();

            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentAction = Action.Duel;

            _Character.castables[_Character.CastableID].GetComponent<Damager>()._Impactor = Target.GetComponent<Impact>();

            Duel();
        }
        else
        {
            if(currentAction == Action.Duel)
            {
                currentAction = Action.Chase;
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
}