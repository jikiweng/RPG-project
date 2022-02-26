using System;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            if (fighter.Target == null)
                GetComponent<Text>().text = "N/A";
            else
            {
                Health health = fighter.Target;
                gameObject.GetComponent<Text>().text = String.Format("Enemy: {0:0}/{1:0}",
             health.HealthPoints, health.GetMaxHealth());
            }
        }
    }
}