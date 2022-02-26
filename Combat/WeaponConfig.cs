using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "WeaponConfigs/Make New WeaponConfig", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] GameObject pickupPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float weaponRange = 2.0f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "weapon";

        public GameObject PickupPrefab { get { return pickupPrefab; } }
        public float WeaponRange { get { return weaponRange; } }
        public float WeaponDamage { get { return weaponDamage; } }
        public float PercentageBonus { get { return percentageBonus; } }

        public Weapon SpawnWeapon(Transform rightHand, Transform leftHand, Animator animator)
        {
            destroyOldWeapon(rightHand, leftHand);
            Transform handTransform = isRightHanded ? rightHand : leftHand;
            Weapon weapon = null;
            if (equippedPrefab != null)
            {
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
                animator.runtimeAnimatorController = animatorOverride;
            else if (overrideController != null)
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            
            return weapon;
        }

        private void destroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon;

            if (rightHand.Find(weaponName) != null)
                oldWeapon = rightHand.Find(weaponName);
            else if (leftHand.Find(weaponName) != null)
                oldWeapon = leftHand.Find(weaponName);
            else return;

            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }
        public void LaunchProjectile(GameObject instigator, Transform leftHand, Health target, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, leftHand.position, Quaternion.identity);
            projectileInstance.AimAt(instigator, target, calculatedDamage);
        }
    }
}
