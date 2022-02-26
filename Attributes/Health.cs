using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] int regenerationPercentage = 70;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> { }

        LazyValue<float> healthPoints;
        public float HealthPoints { get { return healthPoints.value; } }

        private bool isDead = false;
        public bool IsDead() { return isDead; }

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetMaxHealth);
        }

        private void Start()
        {
            healthPoints.ForceInit();    //if healthPoints has not been set value yet, it will be set now
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += getHealthPoint;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= getHealthPoint;
        }

        public float GetMaxHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void getHealthPoint()
        {
            float healthRegeneration = GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationPercentage / 100;
            healthPoints.value = Mathf.Max(healthPoints.value, healthRegeneration);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            //this will return the big one between healthPoints-damage and 0
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            //print(healthPoints);
            if (!isDead && healthPoints.value == 0)
            {
                onDie.Invoke();
                Die();
                //get XP
                if (instigator.GetComponent<Experience>() != null)
                    instigator.GetComponent<Experience>().GetExperience(GetComponent<BaseStats>().GetExperienceReward());
            }
            else
                takeDamage.Invoke(damage);
        }

        public void Heal(float healthToRestore)
        {
            float maxHealthPoints=GetComponent<BaseStats>().GetStat(Stat.Health);
            healthPoints.value=Mathf.Min(healthPoints.value+healthToRestore,maxHealthPoints);
        }

        public float GetFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
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
            return healthPoints.value;
        }
        //restore health
        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            if (healthPoints.value == 0)
                Die();
        }
    }
}