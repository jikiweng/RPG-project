using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerence = 0.1f;
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0,1)] [SerializeField] float patrolSpeedFraction=0.1f;
        //[Range(0,1)] [SerializeField] float chaseSpeedFraction=0.8f;

        Fighter fighter;
        Health health;
        Mover mover;
        GameObject player;

        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Start()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindGameObjectWithTag("Player");

            guardPosition = transform.position;
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (Vector3.Distance(player.transform.position, transform.position) <= chaseDistance
            && fighter.CanAttack(player))  //attack behaviour
            {
                timeSinceLastSawPlayer = 0;
                fighter.Attack(player);
            }
            else if (timeSinceLastSawPlayer <= suspicionTime)  //suspicious state
            {
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            else //patrol behaviour
            {
                Vector3 nextPosition = guardPosition;

                if (patrolPath != null)
                {
                    float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());

                    if (distanceToWaypoint < waypointTolerence)
                    {
                        if (timeSinceArrivedAtWaypoint >= waypointDwellTime) 
                            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
                    }
                    else
                        timeSinceArrivedAtWaypoint = 0;
                    nextPosition = GetCurrentWaypoint();
                }
                mover.MoveToCoursor(nextPosition,patrolSpeedFraction);
            }

            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetCurrentWaypoint(currentWaypointIndex);
        }

        //call by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
