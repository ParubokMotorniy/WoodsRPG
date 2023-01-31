using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour,ISaveable
    {
        [SerializeField] EventWrapper takeDamage;
        [SerializeField] UnityEvent onDie;
        float capsuleHeight,capsuleRadius;
        LazyValue<float> healthPoints;
        private bool isDead = false;
        [System.Serializable]
        public class EventWrapper : UnityEvent<float>
        {

        }
        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }
        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        private void Start()
        {
            healthPoints.ForceInit();
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }
        private void OnEnabled()
        {
            Debug.Log("I am activated");
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }
        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            if(tag == "Player")
            {
                Debug.LogFormat("Restored {0} healthpoints", (float)state);
            }
            if((float)state == 0)
            {
                Die();
            }
        }
        public bool IsDead()
        {
            return isDead;
        }
        public void TakeDamage(float damage,GameObject instigator)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage,0);
            if(healthPoints.value <= 0 )
            {
                AwardExperience(instigator);
                onDie.Invoke();
                Die();
            } else
            {
                takeDamage.Invoke(damage);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public float GetHealthPercentage()
        {
            return 100*(GetHealthFraction());
        }
        public float GetHealthFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        public float GetHealth()
        {
            return healthPoints.value;
        }
        public float GetMaxHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        private void Die()
        {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            Debug.DrawRay(transform.position, Vector3.up * 10, Color.red, 25);
            Debug.DrawRay(transform.position + Vector3.up * 7 - transform.TransformDirection(transform.right) * 3f, transform.TransformDirection(transform.right) * 6, Color.red, 25);
            MakeBodyObstcale();
        }
        private void MakeBodyObstcale()
        {
            GetComponent<NavMeshAgent>().enabled = false;
            NavMeshObstacle obstacle = gameObject.AddComponent<NavMeshObstacle>();
            obstacle.carving = true;
            obstacle.shape = NavMeshObstacleShape.Capsule;
            obstacle.radius = capsuleRadius;
            obstacle.height = capsuleHeight;
        }
        public void Heal(float healthToRestore)
        {

            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealth());
        }
        public void RegenerateHealth()
        {
            float regenerationAmount = GetComponent<BaseStats>().GetStat(Stat.Health);
            Debug.Log("Health = "+healthPoints);
            Debug.Log("Amount = "+regenerationAmount);
            Debug.Log("Heal Result = "+ Mathf.Max(healthPoints.value, regenerationAmount));
            healthPoints.value = Mathf.Max(healthPoints.value, regenerationAmount);
        }
    }
}
