using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] float shoutDistance = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerence = 0.1f;
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0, 1)] [SerializeField] float patrolSpeedFraction = 0.1f;
        //[Range(0,1)] [SerializeField] float chaseSpeedFraction=0.8f;

        Fighter fighter;
        Health health;
        Mover mover;
        GameObject player;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindGameObjectWithTag("Player");
            guardPosition = new LazyValue<Vector3>(getInitialValue);
        }

        private Vector3 getInitialValue()
        {
            return transform.position;
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (isAggrevated() && fighter.CanAttack(player))  //attack behaviour
            {
                timeSinceLastSawPlayer = 0;
                fighter.Attack(player);

                aggrevateNearbyEnemies();
            }
            else if (timeSinceLastSawPlayer <= suspicionTime)  //suspicious state
            {
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            else //patrol behaviour
            {
                Vector3 nextPosition = guardPosition.value;

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
                mover.MoveToCoursor(nextPosition, patrolSpeedFraction);
            }

            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private bool isAggrevated()
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            return distance <= chaseDistance || timeSinceAggrevated < aggroCooldownTime;
        }

        private void aggrevateNearbyEnemies()
        {
            //use sphere cast to catch gameobject within shoutdistance
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0f);
            //if there ais other enemy, aggrevate him
            foreach (RaycastHit hit in hits)
            {
                AIController enemyController = hit.collider.GetComponent<AIController>();
                if (enemyController != null)
                    enemyController.Aggrevate();
            }
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
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
