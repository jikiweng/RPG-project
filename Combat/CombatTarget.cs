using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    //check if the attached gameobject has Health component
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            if (!playerController.GetComponent<Fighter>().CanAttack(gameObject)) return false;

            if (Input.GetMouseButton(0))
                playerController.GetComponent<Fighter>().Attack(gameObject);

            return true;
        }
    }
}