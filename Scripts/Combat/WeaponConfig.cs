using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon",menuName = "Weapons/Make New Weapon")]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField]
        AnimatorOverrideController animatorOverride = null;
        [SerializeField]
        Weapon weaponPrefab = null;
        [SerializeField]
        private float weaponRange = 0;
        [SerializeField]
        private float weaponPercentageBonus = 0;
        [SerializeField]
        private float timeBetweenAttacks = Mathf.Infinity;
        [SerializeField]
        private float weaponDamage = 0;
        [SerializeField]
        private bool isRightHanded = true;
        [SerializeField]
        private Projectile projectile = null;
        const string weaponName = "Weapon";
        public Weapon SpawnWeapon(Transform rightHand,Transform leftHand,Animator animatorToOverride)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;
            if (weaponPrefab != null)
            {
                Transform hand = GetTransform(rightHand, leftHand);
                weapon =  Instantiate(weaponPrefab, hand);
                weapon.name = weaponName;
            }
            var overrideController = animatorToOverride.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animatorToOverride.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animatorOverride.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
        }
        private void DestroyOldWeapon(Transform rightHand,Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;
            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }
        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform hand;
            if (isRightHanded) hand = rightHand;
            else hand = leftHand;
            return hand;
        }

        public void SpawnProjectile(Transform rightHand, Transform leftHand,GameObject instigator,Health target,float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target,calculatedDamage,instigator);
        }
        public bool IsProjectile() {
            return projectile != null;
        }
        public float GetDamage()
        {
            return weaponDamage;
        }
        public float GetRange()
        {
            return weaponRange;
        }
        public float GetAttackDelay()
        {
            return timeBetweenAttacks;
        }
        public float GetPercentageBonus()
        {
            return weaponPercentageBonus;
        }
    }
}