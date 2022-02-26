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
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        private Health target;
        public Health Target { get { return target; } }

        WeaponConfig currentWeaponConfig = null;
        public WeaponConfig CurrentWeaponConfig { get { return currentWeaponConfig; } }
        LazyValue<Weapon> currentWeapon;

        //the first attack should happen immediately
        float timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(getInitialValue);
        }

        private Weapon getInitialValue()
        {
            return EquipWeapon(defaultWeapon);
        }

        private void Start()
        {
            EquipWeapon(currentWeaponConfig);
        }

        private void Update()
        {
            //take record for the time
            timeSinceLastAttack += Time.deltaTime;

            //if you do not click on an enemy or the enemy is dead, skip
            if (target == null || target.IsDead()) return;
            //if the enemy is way too far from the player, get close to enemy first
            if (Vector3.Distance(transform.position, target.transform.position) >= currentWeaponConfig.WeaponRange)
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            else
            {
                //stop moving and stary to attack
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public Weapon EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            Animator animator = GetComponent<Animator>();
            currentWeapon.value = weapon.SpawnWeapon(rightHandTransform, leftHandTransform, animator);
            return currentWeapon.value;
        }

        private void AttackBehaviour()
        {
            //face to the target
            transform.LookAt(target.transform);
            //control the cooldown time between 2 attacks 
            if (timeSinceLastAttack == 0 || timeSinceLastAttack >= timeBetweenAttacks)
            {
                GetComponent<Animator>().ResetTrigger("stopAttack");
                //This will trigger the Hit() event
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0f;
            }
        }

        //Animation Event
        void Hit()
        {
            if (target == null) { return; }
            if (currentWeapon != null)
                currentWeapon.value.OnHit();
            target.TakeDamage(gameObject, GetComponent<BaseStats>().GetStat(Stat.Damage));
        }

        //Animation Event
        void Shoot()
        {
            if (target == null) { return; }
            if (currentWeaponConfig.HasProjectile())
                currentWeaponConfig.LaunchProjectile(gameObject, leftHandTransform, target,
                GetComponent<BaseStats>().GetStat(Stat.Damage));
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            //if the combat target is too far/dead & the combat target is out of the weapon range, reurn false
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
            Vector3.Distance(transform.position, combatTarget.transform.position) >= currentWeaponConfig.WeaponRange)
                return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Cancel()
        {
            target = null;
            timeSinceLastAttack = 0;
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
            GetComponent<Mover>().Cancel();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
                yield return currentWeaponConfig.WeaponDamage;
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if (stat == Stat.Damage)
                yield return currentWeaponConfig.PercentageBonus;
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}