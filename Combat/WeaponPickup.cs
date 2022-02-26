using RPG.Control;
using UnityEngine;
using RPG.Movement;
using RPG.Attributes;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weaponPickup = null;
        [SerializeField] float healthToRestore = 0;
        [SerializeField] bool canPickup = false;

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            if (Input.GetMouseButtonDown(0))
                playerController.GetComponent<Mover>().MoveToCoursor(transform.position, 1f);
            return true;
        }

        void OnTriggerEnter(Collider other)
        {
            Fighter fighter = other.GetComponent<Fighter>();
            Transform transform = other.GetComponent<Transform>();

            if (other.tag != "Player" || !canPickup) return;

            //if the pickup is a heal item, restore the health
            if (healthToRestore > 0)
                other.GetComponent<Health>().Heal(healthToRestore);

            if (weaponPickup != null)
            {
                if (fighter.CurrentWeaponConfig.PickupPrefab != null)
                    Instantiate(fighter.CurrentWeaponConfig.PickupPrefab, transform.position, transform.rotation);
                fighter.EquipWeapon(weaponPickup);
            }
            
            Destroy(gameObject);
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag != "Player") return;
            canPickup = true;
        }
    }
}
