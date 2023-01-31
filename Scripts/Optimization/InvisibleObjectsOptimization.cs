using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Optimization
{
    public class InvisibleObjectsOptimization : MonoBehaviour
    {
        [SerializeField] float maxVisibleDistance;
        [SerializeField] GameObject[] objectsToDeactivate;
        [SerializeField] MeshRenderer meshToCheckVisibilityOf;
        [Range(0,10)]
        [SerializeField] float visibilityMarginX;
        [Range(0,10)]
        [SerializeField] float visibilityMarginY;
        [Range(-100, 0)]
        [SerializeField] float visibilityMarginZ;

        public UnityEvent onBecameInvisible;
        public UnityEvent onBecameVisible;
        void Update()
        {
            Vector3 viewPortPosition = Camera.main.WorldToViewportPoint(transform.position);
            if(viewPortPosition.z >= visibilityMarginZ && viewPortPosition.x >= -visibilityMarginX && viewPortPosition.x <= (1 + visibilityMarginX) && viewPortPosition.y >= -visibilityMarginY && viewPortPosition.y <= (1 + visibilityMarginY) && Vector3.Distance(transform.position, Camera.main.transform.position) <= maxVisibleDistance)
            {
                if(meshToCheckVisibilityOf.isVisible)
                {
                    onBecameVisible.Invoke();
                    DeactivateObjects(true);
                    Debug.Log("Enabling Because Object Is Within Frame's Boundaries");
                } else
                {
                    Vector3 direction = Camera.main.transform.position - transform.position;
                    bool obstaclesCheck = Physics.Raycast(transform.position, direction.normalized, maxVisibleDistance);
                    if (obstaclesCheck)
                    {
                        onBecameInvisible.Invoke();
                        DeactivateObjects(false);
                    } else
                    {
                        onBecameVisible.Invoke();
                        DeactivateObjects(true);
                        Debug.Log("Enabling Because Object Is Within Frame's Boundaries");
                    }
                }
            } else
            {
                onBecameInvisible.Invoke();
                DeactivateObjects(false);
            }

        }
        private void DeactivateObjects(bool activeness)
        {
            foreach(GameObject victim in objectsToDeactivate)
            {
                victim.SetActive(activeness);
            }
        }
    }
}

