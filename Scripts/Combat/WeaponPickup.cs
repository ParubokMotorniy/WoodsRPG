using System.Collections;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;
namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour,IRaycastable
    {
        [SerializeField]
        WeaponConfig weapon = null;
        [SerializeField]
        float healthToRestore = 0;
        [SerializeField]
        float hideTime = 3;
        [SerializeField]
        float pickupDistance = 3;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player") return;
            PickUp(other.gameObject);
        }

        private void PickUp(GameObject subject)
        {
            if(weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            } 
            if(healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            StartCoroutine(HideForSeconds(hideTime));
        }

        private IEnumerator HideForSeconds(float time)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(time);
            ShowPickup(true);
        }
        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            if(Vector3.Distance(playerController.transform.position, transform.position) <= pickupDistance)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PickUp(playerController.gameObject);
                    Debug.Log("CanPickUp");
                }
                return true;
            }
            return false;
        }

        public CursorType GetCursorType()
        {
            return CursorType.PickUp;
        }
    }
}

