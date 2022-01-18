using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2.0f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;

        Health target;
        //the first attack should happen immediately
        float timeSinceLastAttack = Mathf.Infinity;

        private void Update()
        {
            //take record for the time
            timeSinceLastAttack += Time.deltaTime;

            //if you do not click on an enemy or the enemy is dead, skip
            if (target == null || target.IsDead()) return;
            //if the enemy is way too far from the player, get close to enemy first
            if (Vector3.Distance(transform.position, target.transform.position) >= weaponRange)
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            else
            {
                //stop moving and stary to attack
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            //face to the target
            transform.LookAt(target.transform);
            //control the cooldown time between 2 attacks 
            if (timeSinceLastAttack == 0 || timeSinceLastAttack >= timeBetweenAttacks)
            {
                GetComponent<Animator>().ResetTrigger("stopAttack");
                //This will trigger the Hit() event
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0f;
            }
        }

        //Animation Event
        void Hit()
        {
            if (target == null) { return; }
            target.TakeDamage(weaponDamage);
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Cancel()
        {
            target = null;
            timeSinceLastAttack = 0;
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
            GetComponent<Mover>().Cancel();
        }
    }
}