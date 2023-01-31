using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health target = null;
        [SerializeField] float movementSpeed;
        float damage = 0;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10;
        [SerializeField] float lifeAfterImpact = 0;
        [SerializeField] GameObject[] destroyOnHit;
        [SerializeField] UnityEvent onHit; 
        private GameObject instigator;
        public void SetTarget(Health target, float damage,GameObject instigator)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            transform.LookAt(FindAimPoint());
            Destroy(gameObject, maxLifeTime);
        }
        void Update()
        {
            if (target == null) return;
            if (isHoming && !target.IsDead())
            {
                transform.LookAt(FindAimPoint());
            }
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
        }

        private Vector3 FindAimPoint()
        {
            CapsuleCollider capsuleCollider = target.GetComponent<CapsuleCollider>();
            if (capsuleCollider == null) return target.transform.position;
            return (target.transform.position + Vector3.up * (capsuleCollider.height / 2));
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            onHit.Invoke();
            movementSpeed = 0;
            target.TakeDamage(damage,instigator);
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation);
            }
            foreach (GameObject obj in destroyOnHit)
            {
                Destroy(obj);
            }
            Destroy(this.gameObject, lifeAfterImpact);
        }
    }
}

