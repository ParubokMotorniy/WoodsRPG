using System;
using GameDevTV.Utils;
using UnityEngine;
namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffect;
        public event Action onLevelUp;
        LazyValue<int> currentLevel;
        private Experience XP;
        private void Awake()
        {
            XP = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }
        private void Start()
        {
            onLevelUp += LevelUp;
            currentLevel.ForceInit();
        }
        private void OnEnable()
        {
            if (XP != null)
            {
                XP.onXPGained += UpdateLevel;
            }
        }
        private void OnDisable()
        {
            if (XP != null)
            {
                XP.onXPGained -= UpdateLevel;
            }
        }
        public float GetStat(Stat stat)
        {
            return (progression.GetStat(stat,characterClass, GetCurrentLevel()) + GetAdditiveModifier(stat)) * (1+GetPercentageModifier(stat)/100);
        }

        private float GetPercentageModifier(Stat stat)
        {
            float sum = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                if (provider.GetPercentageModifiers(stat) == null) continue;
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    sum += modifier;
                }
            }
            return sum;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            float sum = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                if (provider.GetAdditiveModifiers(stat) == null) continue;
                foreach(float modifier in provider.GetAdditiveModifiers(stat))
                {
                    sum += modifier;
                }
            }
            return sum;
        }

        public int GetCurrentLevel()
        {
            return currentLevel.value;
        }
        private int CalculateLevel()
        {
            if (characterClass != CharacterClass.Player) return startingLevel;
            float currentXP = GetComponent<Experience>().GetXP();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, CharacterClass.Player);
            for(int level = 1;level <= penultimateLevel;level ++)
            {
                float experienceToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp,CharacterClass.Player,level);
                if(experienceToLevelUp > currentXP)
                {
                    if(level >= startingLevel) return level;
                }
            }
            return penultimateLevel + 1;
        }
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                onLevelUp();
                print("Leveled Up");
            }
        }

        private void LevelUp()
        {
            Instantiate(levelUpEffect, transform);
        }
    }
    
}
