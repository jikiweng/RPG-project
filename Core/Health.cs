using UnityEngine;
//using RPG.Movement;
//using RPG.Combat;
using RPG.Saving;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float healthPoints = 100f;

        private bool isDead = false;
        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(float damage)
        {
            //this will return the big one between healthPoints-damage and 0
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            print(healthPoints);
            Die();
        }

        private void Die()
        {
            if (!isDead && healthPoints == 0)
            {
                isDead = true;
                GetComponent<Animator>().SetTrigger("die");
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }

        //save health
        public object CaptureState()
        {
            return healthPoints;
        }
        //restore health
        public void RestoreState(object state)
        {
            healthPoints = (float)state;
            if (healthPoints == 0)
                Die();
        }
    }
}