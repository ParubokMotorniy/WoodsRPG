using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.SceneManagement
{
    public class TreeColliderSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class ColliderToTreeMapping
        {
            public GameObject treePrefab;
            public GameObject treeCollider;
        }
        public List<ColliderToTreeMapping> colliderToTreeMappings = new List<ColliderToTreeMapping>();
        public IEnumerator SpawnTreeColliders()
        {
            TreeInstance[] treeInstances = Terrain.activeTerrain.terrainData.treeInstances;
            TreePrototype[] treePrototypes = Terrain.activeTerrain.terrainData.treePrototypes;
            GameObject collidersParent = GameObject.Find("Tree Colliders");

            foreach(TreeInstance instance in treeInstances)
            {
                TreePrototype instancePrototype = treePrototypes[instance.prototypeIndex];
                foreach(ColliderToTreeMapping treeMapping in colliderToTreeMappings)
                {
                    if(treeMapping.treePrefab == instancePrototype.prefab)
                    {
                        GameObject treeCollider = Instantiate(treeMapping.treeCollider, Vector3.Scale(Terrain.activeTerrain.terrainData.size, instance.position) + Terrain.activeTerrain.transform.position, Quaternion.identity);

                        treeCollider.transform.localScale = new Vector3(treeCollider.transform.localScale.x * instance.widthScale, treeCollider.transform.localScale.y * instance.heightScale, treeCollider.transform.localScale.z * instance.widthScale);
                        treeCollider.transform.Rotate(Vector3.up, instance.rotation * Mathf.Rad2Deg);
                        treeCollider.transform.parent = collidersParent.transform;
                    }
                }
            }
            yield return null;
        }
    }
}

