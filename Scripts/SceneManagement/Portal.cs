using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Core;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum TeleportIdentifier
        {
            A,B,C,D,E,F,G,H,I
        }
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] float fadeInTime, fadeOutTime, fadeWaitTime;
        [SerializeField] Transform playerPosition;
        [SerializeField] TeleportIdentifier teleportIdentifier;
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }
        
        private IEnumerator Transition()
        {
            if(sceneToLoad < 0)
            {
                Debug.LogError("Scene To Load Is Not Set!");
                yield break;
            }
            DontDestroyOnLoad(gameObject);
            Fader fader = FindObjectOfType<Fader>();
            PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            player.enabled = false;
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();

            yield return fader.FadeOut(fadeOutTime);

            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerController newPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            newPlayer.enabled = false;

            wrapper.Load();
            
            Portal nextLevelPortal = GetNextPortal();
            UpdatePlayer(nextLevelPortal);

            Debug.Log("Spawning trees");
            TreeColliderSpawner treeSpawner = FindObjectOfType<TreeColliderSpawner>();
            yield return treeSpawner.StartCoroutine(treeSpawner.SpawnTreeColliders());

            yield return new WaitForSeconds(fadeWaitTime);
            Debug.Log("Fading in");
            fader.FadeIn(fadeInTime);

            newPlayer.enabled = true;

            wrapper.Save();

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal nextLevelPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(nextLevelPortal.playerPosition.position);
            player.transform.rotation = nextLevelPortal.playerPosition.rotation;
        }
        private Portal GetNextPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if(portal != this && portal.teleportIdentifier == teleportIdentifier)
                {
                    return portal;
                }
            }
            return null;
        }
    }
}

