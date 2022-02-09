using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        private float healthPoints = -1f;

        private bool isDead = false;
        public bool IsDead() { return isDead; }

        private void Start()
        {
            if (healthPoints < 0)
                healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            //this will return the big one between healthPoints-damage and 0
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            print(healthPoints);
            if (!isDead && healthPoints == 0)
            {
                Die();
                //get XP
                if (instigator.GetComponent<Experience>() != null)
                    instigator.GetComponent<Experience>().GetExperience(GetComponent<BaseStats>().GetExperienceReward());
            }
        }

        public float GetPercentage()
        {
            return healthPoints / GetComponent<BaseStats>().GetStat(Stat.Health) * 100;
        }

        private void Die()
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
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