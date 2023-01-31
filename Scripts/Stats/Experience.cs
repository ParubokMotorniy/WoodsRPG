using UnityEngine;
using System.Collections;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour,ISaveable
    {
        [SerializeField] float experiencePoints = 0;
        public event Action onXPGained;
        public object CaptureState()
        {
            return experiencePoints;
        }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onXPGained();
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }

        public float GetXP()
        {
            return experiencePoints;
        }
    }
}