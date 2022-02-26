using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats baseStats;
        Experience experience;

        private void Awake()
        {
            baseStats=GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            gameObject.GetComponent<Text>().text = String.Format("Level: {0}{1}Experience: {2:0}",
             baseStats.GetLevel(), Environment.NewLine, experience.ExperiencePoint);
        }
    }
}