using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes  
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] Health healthToDisplay;
        private Text healthLabel;
        void Start()
        {
            healthLabel = GetComponent<Text>();
        }

        void Update()
        {
            healthLabel.text = String.Format("{0:0}/{1:0}", healthToDisplay.GetHealth(),healthToDisplay.GetMaxHealth());
        }
    }
}
