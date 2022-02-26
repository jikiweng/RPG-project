using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] bool isHoming = false;
        [SerializeField] float speed = 3;
        [SerializeField] float maxDistance = 20;
        [SerializeField] float lifeAfterImpact = 0.2f;
        [SerializeField] GameObject hitImpact = null;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] UnityEvent onHit;    //hit sound

        private Health target;
        private Vector3 targetPoint;
        private Vector3 startPoint;
        private GameObject instigator = null;
        private float damage;

        private void Start()
        {
            startPoint = transform.position;
        }

        public void AimAt(GameObject instigator, Health target, float damage)
        {
            this.damage = damage;
            this.target = target;
            this.instigator = instigator;
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            targetPoint = (targetCapsule == null) ? target.GetComponent<Transform>().position :
            target.GetComponent<Transform>().position + Vector3.up * targetCapsule.height / 2;
            transform.LookAt(targetPoint);
        }

        // Update is called once per frame
        void Update()
        {
            if (isHoming && !target.IsDead())
                transform.LookAt(targetPoint);
            if (target != null)
                transform.Translate(Vector3.forward * speed);
            if (Vector3.Distance(transform.position, startPoint) >= maxDistance)
                Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() == target)
            {
                if (target.IsDead()) return;

                onHit.Invoke();
                speed = 0;
                other.GetComponent<Health>().TakeDamage(instigator, damage);

                foreach (GameObject toDestroy in destroyOnHit)
                    Destroy(toDestroy);

                //show the hit impact
                if (hitImpact != null)
                    Instantiate(hitImpact, targetPoint, transform.rotation);
                Destroy(gameObject, lifeAfterImpact);
            }
            //if (other.tag == "Environment")
            //Destroy(gameObject);
        }
    }
}
