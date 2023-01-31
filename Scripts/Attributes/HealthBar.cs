using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform bar = null;
        [SerializeField] Health healthToDisplay = null;
        [SerializeField] Canvas canvasToTweak = null;
        void Update()
        {
            if(Mathf.Approximately(healthToDisplay.GetHealthFraction(),0) || Mathf.Approximately(healthToDisplay.GetHealthFraction(), 1))
            {
                canvasToTweak.enabled = false;
                return;
            }
            canvasToTweak.enabled = true;
            bar.transform.localScale = new Vector3(healthToDisplay.GetHealthFraction(), bar.transform.localScale.y, bar.transform.localScale.z);
        }
    }
}