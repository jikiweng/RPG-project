using UnityEngine;
using System.Collections.Generic;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            buildLookup();
            float[] levels = lookupTable[characterClass][stat];
            if (levels.Length < level||level<=0) return 0;
            return levels[level - 1];
        }

        public int GetLevels(Stat stat,CharacterClass characterClass)
        {
            buildLookup();
            float[] levels=lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void buildLookup()
        {
            if (lookupTable != null) return;
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var stateLookupTable = new Dictionary<Stat, float[]>();
                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    stateLookupTable[progressionStat.stat] = progressionStat.levels;
                }
                lookupTable[progressionClass.characterClass] = stateLookupTable;
            }
        }
    }
}