using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/Make New Progression Sheet", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses;
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;
        [System.Serializable]
        public class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }
        [System.Serializable]
        public class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            return lookupTable[characterClass][stat].Length;
        }
        public float GetStat(Stat stat,CharacterClass characterClass,int level)
        {
            BuildLookup();
            if (lookupTable.ContainsKey(characterClass))
            {
                if (lookupTable[characterClass].ContainsKey(stat))
                {
                    float[] levels = lookupTable[characterClass][stat];
                    if (levels.Length < level) return 0;
                    return levels[level - 1];
                }
                return 0;
            }
            return 0;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            foreach(ProgressionCharacterClass progression in characterClasses)
            {
                Dictionary<Stat, float[]> stats = new Dictionary<Stat, float[]>();
                foreach(ProgressionStat stat in progression.stats)
                {
                    stats[stat.stat] = stat.levels; 
                }
                lookupTable[progression.characterClass] = stats;
            }
        }
    }
}