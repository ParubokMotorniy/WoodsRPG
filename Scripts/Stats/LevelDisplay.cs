using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats playerStats;
        private Text levelLabel;
        void Start()
        {
            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
            levelLabel = GetComponent<Text>();
        }

        void Update()
        {
            levelLabel.text = playerStats.GetCurrentLevel().ToString();
        }
    }
}

