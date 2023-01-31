using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{
    [RequireComponent(typeof(Fighter))]
    public class EnemyAIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5;
        [SerializeField] float suspicionTime = 3;
        [SerializeField] ControlPath controlPath;
        [SerializeField] float waypointTolerance = 1;
        [SerializeField] int waypointToStartAt;
        [SerializeField] float waypointDwellTime = 2.5f;
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float aggrevationTime = 10;
        [SerializeField] float shoutDistance = 7;
        private Fighter enemyFighter;
        private GameObject player;
        private Health health;
        private Mover enemyMover;
        private LazyValue<Vector3> spotToGuard;
        private float timeSinceLastPlayerEncounter = Mathf.Infinity,timeSinceArrivedAtWaypoint = Mathf.Infinity,timeSinceAggrevated = Mathf.Infinity;
        private int currentWaypointIndex = 0;
        private void Awake()
        {
            health = GetComponent<Health>();
            enemyFighter = GetComponent<Fighter>();
            enemyMover = GetComponent<Mover>();
            player = GameObject.FindGameObjectWithTag("Player");
            spotToGuard = new LazyValue<Vector3>(GetInitialSpot);
        }
        private Vector3 GetInitialSpot()
        {
            return transform.position;
        }
        void Start()
        {
            spotToGuard.ForceInit();
            currentWaypointIndex = waypointToStartAt;
        }

        void Update()
        {
            if (health.IsDead()) return;
            if (IsAggrevated() && enemyFighter.CurrentEnemyCanAttack(player))
            {
                AttackBehaviour();
            }
            else if (suspicionTime >= timeSinceLastPlayerEncounter)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();
        }
        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }
        private void UpdateTimers()
        {
            timeSinceLastPlayerEncounter += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Debug.DrawRay(transform.position, Vector3.up * 5, Color.blue);
            Vector3 nextPositionToGuard = spotToGuard.value;
            if(controlPath != null)
            {
                if (AtWaypoint())
                {
                    CycleWaypoint();
                    timeSinceArrivedAtWaypoint = 0;
                }
                nextPositionToGuard = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint >= waypointDwellTime)
            {
                enemyMover.StartMoveAction(nextPositionToGuard,patrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return controlPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = controlPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToCurrentWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToCurrentWaypoint <= waypointTolerance;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            Debug.DrawRay(transform.position, Vector3.up * 5, Color.red);
            enemyFighter.Attack(player);
            timeSinceLastPlayerEncounter = 0;
            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position,shoutDistance);
            foreach(Collider hit in hits)
            {
                EnemyAIController controller = hit.gameObject.GetComponent<EnemyAIController>();
                if (controller != null)
                {
                    controller.Aggrevate();
                }
            }
        }

        private bool IsAggrevated()
        {
            return (Vector3.Distance(player.transform.position, transform.position) <= chaseDistance) || (timeSinceAggrevated < aggrevationTime);
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}