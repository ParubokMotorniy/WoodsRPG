using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using UnityEngine.EventSystems;
using System;
using UnityEngine.AI;
using Cinemachine;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover playerMover;
        private Fighter playerFighter;
        private Health health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType cursorType;
            public Texture2D texture;
            public Vector2 hotspot;
        }
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1;
        [SerializeField] float sphereRadius = 0.75f;
        [SerializeField] LayerMask terrainMask;    
        [SerializeField] KeyCode cameraMovementKey;
        [SerializeField] CinemachineFreeLook freeLookCam;
        [SerializeField] private float camXSpeed = 0;
        [SerializeField] private float camYSpeed = 0;
        private void Awake()
        {
            playerMover = GetComponent<Mover>();
            health = GetComponent<Health>();
            playerFighter = GetComponent<Fighter>();
        }
        private void Update()
        {
            if (!Input.GetKey(cameraMovementKey))
            {
                /*if(freeLookCam.m_XAxis.m_MaxSpeed != 0)
                {
                    freeLookCam.m_XAxis.m_MaxSpeed = 0;
                    freeLookCam.m_YAxis.m_MaxSpeed = 0;
                }*/               
                freeLookCam.m_XAxis.m_InputAxisName = "";
                freeLookCam.m_YAxis.m_InputAxisName = "";
                freeLookCam.m_XAxis.m_InputAxisValue = 0;
                freeLookCam.m_YAxis.m_InputAxisValue = 0;
            } else
            {
                /*if (freeLookCam.m_XAxis.m_MaxSpeed == 0)
                {
                    freeLookCam.m_XAxis.m_MaxSpeed = camXSpeed;
                    freeLookCam.m_YAxis.m_MaxSpeed = camYSpeed;
                }*/
                freeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
                freeLookCam.m_YAxis.m_InputAxisName = "Mouse Y";
            }
        }
        void FixedUpdate()
        {
            if (InteractWithUI()) 
            {
                SetCursor(CursorType.UI);
                return; 
            }
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent()) return;
            if(InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }
        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(),sphereRadius);
            float[] distances = new float[hits.Length];
            for(int i = 0;i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }
        private bool InteractWithUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
        private bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!(playerMover.CanMoveTo(target))) return false;
                if (Input.GetMouseButton(0))
                {
                    playerMover.StartMoveAction(target,1);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }
        private bool RaycastNavMesh(out Vector3 target)
        {
            RaycastHit rayHit;
            target = Vector3.zero;
            bool hasHit = Physics.Raycast(GetMouseRay(), out rayHit,Mathf.Infinity,terrainMask);
            if (!hasHit) {return false; }

            NavMeshHit meshHit;
            bool pointFound = NavMesh.SamplePosition(rayHit.point, out meshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!pointFound) { return false; }
            target = meshHit.position;

            return true;
        }
        private void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach(CursorMapping mapping in cursorMappings)
            {
                if(mapping.cursorType == cursorType)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
