using UnityEngine;
using System.Collections;
using RPG.Saving;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float loadFadeInTime;
        private SavingSystem globalSavingSystem;
        const string defaultSavingFile = "save";
        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }
        IEnumerator LoadLastScene()
        {
            globalSavingSystem = GetComponent<SavingSystem>();

            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediately();
            yield return globalSavingSystem.LoadLastScene(defaultSavingFile);

            TreeColliderSpawner treeSpawner = FindObjectOfType<TreeColliderSpawner>();
            yield return treeSpawner.StartCoroutine(treeSpawner.SpawnTreeColliders());

            Debug.Log("Fading In");
            yield return fader.FadeIn(loadFadeInTime);
        }
        public void Save()
        {
            globalSavingSystem.Save(defaultSavingFile);
        }

        public void Load()
        {
            globalSavingSystem.Load(defaultSavingFile);
        }
        public void Delete()
        {
            globalSavingSystem.Delete(defaultSavingFile);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }
       //System/////////////////////////////////////////////
        public void SaveGlobal(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }
        public void LoadGlobal(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }
        private void SaveFile(string saveFile, object state)
        {
            string filePath = GetPathFromSaveFile(saveFile);
            Debug.Log("Saving To " + filePath);
            using (FileStream fileStream = File.Open(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream,state);
            }
        }

        private Dictionary<string,object> LoadFile(string saveFile)
        {
            string filePath = GetPathFromSaveFile(saveFile);
            Debug.Log("Loading From " + filePath);
            if (!File.Exists(filePath))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream fileStream = File.Open(filePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string,object>) formatter.Deserialize(fileStream);
            }
        }

        private void CaptureState(Dictionary<string,object> state)
        {
            foreach(SaveableEntity entity in FindObjectsOfType<SaveableEntity>())
            {
                state[entity.GetUniqueIdentifier()] = entity.CaptureState();
            }
        }

        private void RestoreState(Dictionary<string,object> state)
        {
            foreach(SaveableEntity entity in FindObjectsOfType<SaveableEntity>())
            {
                if (state.ContainsKey(entity.GetUniqueIdentifier()))
                {
                    entity.RestoreState(state[entity.GetUniqueIdentifier()]);
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
        ///Entity/////////////////////////////////////////////////
        public string GetUniqueIdentifier()
        {
            return "";
        }
        public object CaptureEntityState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach(ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
        }
        public void RestoreEntityState(object state)
        {
            Dictionary<string, object> savedState = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                if (savedState.ContainsKey(saveable.GetType().ToString()))
                {
                    saveable.RestoreState(savedState[saveable.GetType().ToString()]);
                }
            }
        }
    }
}