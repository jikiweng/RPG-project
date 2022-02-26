using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 5f;
        [SerializeField] float maxNavMeshDistance = 40f;

        Health health;
        NavMeshAgent navMeshAgent;

        // Start is called before the first frame update
        void Awake()
        {
            health = GetComponent<Health>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            //if the character is dead, stop the navMeshAgent
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        public bool CanMoveTo(Vector3 destiantion)
        {
            //check if there is a complete path get to target
            NavMeshPath path = new NavMeshPath();
            if (!NavMesh.CalculatePath(transform.position, destiantion, NavMesh.AllAreas, path)) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;

            //check if the path is too far
            if (getPathLength(path) > maxNavMeshDistance) return false;

            return true;
        }

        private float getPathLength(NavMeshPath path)
        {
            float sum = 0;
            if (path.corners.Length < 2) return sum;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                sum += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return sum;
        }

        public void MoveToCoursor(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        [System.Serializable]
        struct MoverSaveData
        { 
            public SerializableVector3 position;
            //public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            //data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        //after awake and before start
        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = data.position.ToVector();
            //transform.eulerAngles = data.rotation.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}