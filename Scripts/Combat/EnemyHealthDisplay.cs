using System;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] Fighter whoseEnemiesToDisplay;
        private Text healthLabel;
        private void Awake()
        {
            healthLabel = GetComponent<Text>();
        }

        void Update()
        {
            if(whoseEnemiesToDisplay!=null && whoseEnemiesToDisplay.GetCurrentTarget() != null)
            {
                healthLabel.text = String.Format("{0:0}/{1:0}", whoseEnemiesToDisplay.GetCurrentTarget().GetHealth(),whoseEnemiesToDisplay.GetCurrentTarget().GetMaxHealth());
            } else
            {
                healthLabel.text = "No Enemy";
            }
        }
    }
}
