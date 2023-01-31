using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour,IAction,ISaveable,IModifierProvider
    {
        [SerializeField]
        WeaponConfig defaultWeaponConfig = null;
        [SerializeField]
        Transform rightHandTransform = null;
        [SerializeField]
        Transform leftHandTransform = null;
        private Health target = null;
        private Mover mover;
        private float timeSinceLastAttack = 0;
        private Animator characterAnimator;

        private WeaponConfig currentWeaponConfig;
        private LazyValue<Weapon> currentWeapon;
        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }
        private void Awake()
        {
            mover = GetComponent<Mover>();
            characterAnimator = GetComponent<Animator>();
            currentWeaponConfig = defaultWeaponConfig;
            currentWeapon = new LazyValue<Weapon>(GetInitialWeapon);
        }
        
        void Start()
        {
            currentWeapon.ForceInit();
        }
        public void Cancel()
        {
            target = null;
            mover.Cancel();
            StopAttack();
        }
        private Weapon GetInitialWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }
        private void StopAttack()
        {
            characterAnimator.SetTrigger("stopAttack");
            characterAnimator.ResetTrigger("Attack");
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead()) return;
            if (!GetIsInRange(target.transform))
            {
                mover.MoveTo(target.transform.position,1);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }
        public bool CurrentEnemyCanAttack(GameObject combatTarget)
        {
            if(combatTarget == null) { return false; }
            if (!mover.CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform)) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }
        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetRange();
        }
        private void AttackBehaviour()
        {
            transform.LookAt(target.transform.position);
            if (timeSinceLastAttack >= currentWeaponConfig.GetAttackDelay())
            {
                TriggerAtack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAtack()
        {
            characterAnimator.ResetTrigger("stopAttack");
            characterAnimator.SetTrigger("Attack");
        }

        void Hit()
        {
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (target == null) return;
            if(currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }
            if (currentWeaponConfig.IsProjectile())
            {
                currentWeaponConfig.SpawnProjectile(rightHandTransform, leftHandTransform,gameObject, target,damage);
            }
            else
            {
                target.TakeDamage(damage, gameObject);
            }
        }
        void Shoot()
        {
            Hit();
        }
        [ContextMenu("Pick New Weapon")]
        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.SpawnWeapon(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetCurrentTarget()
        {
            return target;
        }
        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            WeaponConfig weapon = Resources.Load<WeaponConfig>((string) state);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }
                    
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }
    }
}