using UnityEngine;
using System.Collections;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}