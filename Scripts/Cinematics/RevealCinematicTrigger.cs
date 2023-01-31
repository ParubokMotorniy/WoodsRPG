using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class RevealCinematicTrigger : MonoBehaviour,ISaveable
    {
        private bool alreadyTriggered = false;
        private void OnTriggerEnter(Collider other)
        {
            if(!alreadyTriggered && other.tag == "Player" && other.GetComponent<Fighter>().GetCurrentTarget() == null )
            {
                GetComponent<PlayableDirector>().Play();
                alreadyTriggered = true;
            }
        }

        public object CaptureState()
        {
            return alreadyTriggered;
        }

        public void RestoreState(object state)
        {
            alreadyTriggered = (bool)state;
        }
    }
}
