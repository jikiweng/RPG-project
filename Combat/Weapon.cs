using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float weaponRange = 2.0f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "weapon";
        
        public GameObject EquippedPrefab { get { return equippedPrefab; } }
        public float WeaponRange { get { return weaponRange; } }
        public float WeaponDamage { get { return weaponDamage; } }

        public void SpawnWeapon(Transform rightHand, Transform leftHand, Animator animator)
        {
            destroyOldWeapon(rightHand, leftHand);
            Transform handTransform = isRightHanded ? rightHand : leftHand;
            if (equippedPrefab != null)
            {
                GameObject weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
                animator.runtimeAnimatorController = animatorOverride;
            else if (overrideController != null)
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
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
        public void LaunchProjectile(GameObject instigator,Transform leftHand, Health target)
        {
            Projectile projectileInstance = Instantiate(projectile, leftHand.position, Quaternion.identity);
            projectileInstance.AimAt(instigator, target, weaponDamage);
        }
    }
}
