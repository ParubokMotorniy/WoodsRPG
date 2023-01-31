using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] Experience experienceToDisplay;
        private Text healthLabel;
        void Start()
        {
            healthLabel = GetComponent<Text>();
        }
        void Update()
        {
            healthLabel.text = String.Format("{0:0}", experienceToDisplay.GetComponent<Experience>().GetXP().ToString());
        }
    }
}
