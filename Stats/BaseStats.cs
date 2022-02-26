using System;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)] [SerializeField] int startLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifier = false;

        Experience experience;
        LazyValue<int> currentLevel;
        public int GetLevel() { return currentLevel.value; }
        public event Action onLevelUp;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
                experience.onExperienceGained += updateLevel;
        }

        private void OnDisable()
        {
            if (experience != null)
                experience.onExperienceGained -= updateLevel;
        }

        private void updateLevel()
        {
            int newLevel = CalculateLevel();
            if (currentLevel.value < newLevel)
            {
                currentLevel.value = newLevel;
                onLevelUp();
                Instantiate(levelUpParticleEffect, transform);
                print("Level Up!");
            }
        }

        public float GetStat(Stat stat)
        {
            return (getBaseStat(stat) + getAdditiveModifier(stat)) * (1 + getPrecentage(stat) / 100);
        }

        private float getBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, currentLevel.value);
        }

        private float getAdditiveModifier(Stat stat)
        {
            //if(!shouldUseModifier) return 0;

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float getPrecentage(Stat stat)
        {
            if (!shouldUseModifier) return 0;

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifier(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startLevel;

            float currentXP = experience.ExperiencePoint;
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelup, characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelup = progression.GetStat(Stat.ExperienceToLevelup, characterClass, level);
                if (XPToLevelup > currentXP)
                    return level;
            }
            return penultimateLevel + 1;
        }

        public float GetExperienceReward()
        {
            return progression.GetStat(Stat.ExperienceReward, characterClass, currentLevel.value);
        }
    }
}
