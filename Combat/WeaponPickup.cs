using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weaponPickup = null;

        void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player") return;
            if(other.GetComponent<Fighter>().CurrentWeapon.EquippedPrefab !=null)
                Instantiate(other.GetComponent<Fighter>().CurrentWeapon.EquippedPrefab,
                GetComponent<Transform>().position,GetComponent<Transform>().rotation);
                
            other.GetComponent<Fighter>().EquipWeapon(weaponPickup);
            Destroy(gameObject);
        }
    }
}
