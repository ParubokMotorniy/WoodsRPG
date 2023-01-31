using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            Fighter playerFighter = playerController.GetComponent<Fighter>();
            if (!playerFighter.CurrentEnemyCanAttack(this.gameObject))
            {
                return false;
            }
            if (playerFighter.CurrentEnemyCanAttack(this.gameObject) && Input.GetMouseButton(0))
            {
                playerFighter.Attack(this.gameObject);

            }
            return true;
        }
    }
}