using System.Collections.Generic;
using Asset_Packs.GameDev.tv_Assets.Scripts.Inventories;
using Attributes;
using Controllers;
using Core;
using GameDevTV.Inventories;
using Movement;
using Saving;
using Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        private Health target;
        private Equipment equipment;

        private Mover mover;

        //config script object of weapon
        private WeaponConfig currentWeaponConfig;

        //equipped weapon(not unarmed of fireball)
        private Weapon currentWeapon;

        //could be changed depend of the type of weapon.2hands sword is slower weapon but more powerful
        [SerializeField] private float timeBetweenAttacks = 1.3f;
        private ActionScheduler actionScheduler;

        private Animator animator;

        //Mathf.Infinity is used to start attack immediately
        private float timeSinceLastAttack = Mathf.Infinity;

        //where to place weapon(right hand in our case)
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;

        [FormerlySerializedAs("defaultWeapon")] [SerializeField]
        private WeaponConfig defaultWeaponConfig = null;

        // Background music
        [SerializeField]
        private CombatAudioController combatAudio; // assign in Inspector (e.g., on a singleton GameObject)

        // NEW: only player should drive music ducking
        private bool IsPlayer => CompareTag("Player");

        // NEW: track whether this Fighter has already called EnterCombat
        private bool audioEntered = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            mover = GetComponent<Mover>();
            currentWeaponConfig = defaultWeaponConfig;
            currentWeapon = SetupDefaultWeapon();
            //if we have a weapon that update it depends on the config
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }

            // NEW: auto-find controller so you don't have to assign on each scene
            if (combatAudio == null)
            {
                combatAudio = FindObjectOfType<CombatAudioController>();
            }
        }

        public WeaponConfig GetCurrentConfig()
        {
            return currentWeaponConfig;
        }

        // Update is called once per frame
        private void Update()
        {
            //every single frame update increment time since last attack
            timeSinceLastAttack += Time.deltaTime;

            //if there is no target then do nothing
            if (target == null) return;

            //if abject already is dead then do nothing
            if (target.IsDead())
            {
                if (IsPlayer && combatAudio != null && audioEntered)
                {
                    combatAudio.ExitCombat();
                    audioEntered = false;
                }

                target = null;
                return;
            }

            //continue to  move for attack, until reached the point where distance is enough for attack
            if (!IsCloseEnoughToAttack(target.transform))
            {
                //speed for attack is normal speed, so it will multiply on 1f
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.CancelAction();

                //show attack animation
                AttackBehaviour();
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }

        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
            return weaponConfig.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            currentWeaponConfig = weaponConfig;
            if (weaponConfig == null) return;
            currentWeapon = AttachWeapon(weaponConfig);
        }

        private void UpdateWeapon()
        {
            var weaponConfig = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weaponConfig == null)
            {
                EquipWeapon(defaultWeaponConfig);
            }
            else
            {
                EquipWeapon(weaponConfig);
            }
        }

        //get target to use it then for target health display
        public Health GetTarget()
        {
            return target;
        }

        //show attack animation
        private void AttackBehaviour()
        {
            //look directly to the target(enemy) before  to attack it
            transform.LookAt(target.transform);

            //attack only when time since last attack is higher then time between attacks
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                //reset time since last attack
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            //reset the trigger before attack
            animator.ResetTrigger("attack");
            //!!!!this will trigger Hit() event. in our case we can use hits even to take damage
            animator.SetTrigger("attack");
        }

        //animation event
        private void Hit()
        {
            //check if target is exist
            if (target == null) return;

            /*You can, if you wish, make WeaponConfig inherit from StatsEquipableItem instead
            of EquipableItem.  This opens up the stats, but it creates a new problem:
            WeaponConfig's IModifierProvider will "hide" StatsEquipableItem, meaning either we
            recreate the logic of returning the modifiers or we remove the IModifierProvider
            implementation from WeaponConfig.
            What this means is that in Hit() your damage needs to be currentWeaponConfig.GetDamage()
            + baseStats.GetStat(Stat.Damage) instead of just baseStats.GetStat(Stat.Damage).*/

            //damage = level damage + weapon damage
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (currentWeapon != null)
            {
                currentWeapon.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                //get health of the object to attack and minus damage from it
                //target.TakeDamage(gameObject, currentWeapon.WeaponDamage);
                target.TakeDamage(gameObject, damage);
            }
        }

        //shoot and hit are diff steps of animation depends of the weapon. we are going to run same code not depends of animation names
        void Shoot()
        {
            Hit();
        }

        //check if distance between object that attacks and object to attack is close enough to be able to attack with weapon 
        private bool IsCloseEnoughToAttack(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.WeaponRange;
        }

        //check if we can attack the object (could be enemy or player as we use method for both)
        public bool CanAttack(GameObject combatTarget)
        {
            //atack only if there is target to attack and it is not dead
            if (combatTarget == null) return false;
            //can attack if close enough to do it
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !IsCloseEnoughToAttack(combatTarget.transform))
            {
                return false;
            }

            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        //(could be used by enemy or player as we use method for both)
        public void Attack(GameObject combatTarget)
        {
            //stop previous action and start new one
            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();

            // NEW: only the player triggers audio; only once per engagement
            if (IsPlayer && combatAudio != null && target != null && !target.IsDead() && !audioEntered)
            {
                combatAudio.EnterCombat();
                audioEntered = true;
            }
        }

        //stop previous action of the object( from interface)
        public void CancelAction()
        {
            StopAttack();
            // NEW: only if we previously entered
            if (IsPlayer && combatAudio != null && audioEntered)
            {
                combatAudio.ExitCombat();
                audioEntered = false;
            }

            target = null;
            //added to fix the issue with movement during to the cinematic scene. if some movement was in progress in that time
            mover.CancelAction();
        }

        private void StopAttack()
        {
            //reset the trigger after attack
            animator.ResetTrigger("stopAttack");
            //stop to show death animation if player already died
            animator.SetTrigger("stopAttack");
        }

        private void OnDisable()
        {
            // NEW: safety if player Fighter is disabled while in combat
            if (IsPlayer && combatAudio != null && audioEntered)
            {
                combatAudio.ExitCombat();
                audioEntered = false;
            }
        }

        //saving system
        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        //saving system
        public void RestoreState(object state)
        {
            //resources folder in hierarchy used for loading weapon in different scenes without reseting
            string weaponName = (string)state;
            WeaponConfig weaponConfig = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weaponConfig);
        }
    }
}