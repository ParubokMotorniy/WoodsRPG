using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Optimization
{
    public class LightningOptimization : MonoBehaviour
    {
        [SerializeField] Light lightToDeactivate;

        private GameObject mainPlayer;

        void Start()
        {
            mainPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        void Update()
        {
            /*if (meshToCheckVisibilityOf.isVisible)
            {
                if (Vector3.Distance(mainPlayer.transform.position, transform.position) <= maxVisibleDistance)
                {
                    DeactivateObjects(true);
                    onBecameVisible.Invoke();
                }
                else
                {
                    DeactivateObjects(false);
                    onBecameInvisible.Invoke();
                }
            }
            else
            {
                if(Vector3.Distance(mainPlayer.transform.position, transform.position) <= maxInvisibleDistance)
                {
                    DeactivateObjects(true);
                    onBecameVisible.Invoke();
                }
                else
                {
                    DeactivateObjects(false);
                    onBecameInvisible.Invoke();
                }
            }*/
            //CheckIfLightIsDrawn();
        }
        private bool CheckIfLightIsDrawn()
        {
            Vector3 direction = mainPlayer.transform.position - lightToDeactivate.gameObject.transform.position;
            Vector3 lightMarginPoint = lightToDeactivate.gameObject.transform.position + direction.normalized * lightToDeactivate.range;
            Vector3 viewPortLightPoint = Camera.main.WorldToViewportPoint(lightMarginPoint);
            if(viewPortLightPoint.z > 0 && viewPortLightPoint.x <= 1 && viewPortLightPoint.x >= 0 && viewPortLightPoint.y <= 1 && viewPortLightPoint.y >= 0)
            {
                return true;
            }
            return false;
        }
        public void CheckLightningVisibility()
        {
            if (!CheckIfLightIsDrawn())
            {
                lightToDeactivate.enabled = false;
                Debug.Log("Disabling Lightning Because Of Visibility");
            }
        }
        public void ActivateLightning()
        {
            lightToDeactivate.enabled = true;
        }
    }

}