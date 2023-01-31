using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using System.Collections.Generic;

namespace RPG.Movement
{
    [System.Serializable]
    public class Mover : MonoBehaviour,IAction,ISaveable
    {
        private NavMeshAgent navMeshAgent;
        [SerializeField] private float maxSpeed = 6;
        [SerializeField] private float maxPathLength = 35;
        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        void Update()
        {
            UpdateAnimator();
        }
        public void StartMoveAction(Vector3 destination,float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination,speedFraction);
        }
        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxPathLength) return false;
            return true;
        }
        private float GetPathLength(NavMeshPath path)
        {
            float pathLength = 0;
            if (path.corners.Length < 2) return pathLength;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return pathLength;
        }
        public void MoveTo(Vector3 destination,float speedFraction)
        {
            
            navMeshAgent.SetDestination(destination);
            navMeshAgent.speed = maxSpeed * Mathf.Clamp(speedFraction, 0.0000001f, Mathf.Infinity);
            navMeshAgent.isStopped = false;
        }
        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }
        private void UpdateAnimator()
        {
            if(navMeshAgent != null)
            {
                Vector3 currentVelocity = navMeshAgent.velocity;
                Vector3 localVelocity = transform.InverseTransformDirection(currentVelocity);
                float speed = localVelocity.z;
                GetComponent<Animator>().SetFloat("ForwardSpeed", speed);
            } 
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().Warp(position.ToVector());
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
