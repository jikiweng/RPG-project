using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)] [SerializeField] int startLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect=null;

        int currentLevel = 0;
        public int GetLevel()
        { return currentLevel; }

        private void Start()
        {
            currentLevel = CalculateLevel();
            Experience experience = GetComponent<Experience>();
            if (experience != null)
                experience.OnExperienceGained += updateLevel;
        }

        private void updateLevel()
        {
            int newLevel = CalculateLevel();
            if (currentLevel < newLevel)
            {
                currentLevel = newLevel;
                Instantiate(levelUpParticleEffect,transform);
                print("Level Up!");
            }
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, currentLevel);
        }

        public int CalculateLevel()
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
            return progression.GetStat(Stat.ExperienceReward, characterClass, currentLevel);
        }
    }
}
