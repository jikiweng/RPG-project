using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoint = 0;
        public float ExperiencePoint { get { return experiencePoint; } }
        public event Action onExperienceGained;

        public void GetExperience(float experience)
        {
            experiencePoint += experience;
            onExperienceGained();
        }

        public object CaptureState()
        {
            return experiencePoint;
        }

        public void RestoreState(object state)
        {
            experiencePoint = (float)state;
        }
    }
}