using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Attributes
{
    public class RandomSoundGenerator : MonoBehaviour
    {
        [SerializeField] AudioClip[] clips = null;
        [SerializeField] AudioSource source = null;
        [Range(0,1)]
        [SerializeField] float minVolume, maxVolume;
        [Range(-3,3)]
        [SerializeField] float minPitch = 0, maxPitch = 1;
        [SerializeField] bool playOnAwake;
        private void Start()
        {
            if (playOnAwake)
            {
                GenerateSound();
            }
        }
        public void GenerateSound()
        {
            source.clip = clips[Random.Range(0, clips.Length - 1)];
            source.pitch = Random.Range(minPitch, maxPitch);
            source.volume = Random.Range(minVolume, maxVolume);
            source.Play();
        }
    }
}