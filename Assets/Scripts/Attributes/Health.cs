using System;
using Core;
using GameDevTV.Inventories;
using Saving;
using Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        //was 100 by default here. but to avoid defect with saving system changed to -1f to compare
        [SerializeField] private float health = -1f;
        [SerializeField] private float regenerationPercentage = 70f;

        public event Action<GameObject, float> onDamageTaken;
        [SerializeField] private UnityEvent onDie;
        [SerializeField] private TakeDamageEvent takeDamage;
        [SerializeField] private GameObject healingParticleEffect = null;
        [SerializeField] private UnityEvent onHealing;
        //used to be able to do dynamic update of damage text and as serialaz field cound not have type UnityEvent<float> by default
        //so we change it for sub class with such type
        [Serializable]
        public class TakeDamageEvent: UnityEvent<float>
        {
            
        }

        private bool isDead;
        private Animator animator;
        private BaseStats baseStats;
        private Equipment equipment;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            health = GetComponent<BaseStats>().GetStat(Stat.Health);
            baseStats= GetComponent<BaseStats>();              
            //if we have a weapon that update it depends on the config
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
               // equipment.equipmentUpdated += UpdateWeapon;
            }
        }
        
        private void OnEnable() {
            //regenerate health in case of level up
            baseStats.onLevelUp += RegenerateHealth;
            
            if (equipment) equipment.equipmentUpdated += HandleEquipmentUpdated; ;
        }

        private void OnDisable() {
            baseStats.onLevelUp -= RegenerateHealth;
            
            if (equipment) equipment.equipmentUpdated -= HandleEquipmentUpdated;
        }
        
        private void HandleEquipmentUpdated()
        {
            health = Mathf.Min(health, baseStats.GetStat(Stat.Health));
        }
        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject attackInitiator, float damage)
        {
            print(gameObject.name + " took damage: " + damage);
            health = Mathf.Max(health - damage, 0);
            // Notify listeners (retaliation, hit SFX, etc.)
            onDamageTaken?.Invoke(attackInitiator, damage);
            //call event tp show damage text above character
            takeDamage.Invoke(damage);
            //then here we can show health in a health bar
            if (health == 0)
            {
                onDie.Invoke();
                //show death animation
                Die();
                //when enemy diy increase experience point for player
                IncreaseExperience(attackInitiator);
            }
           /* else
            {
                //call event tp show damage text above character
                takeDamage.Invoke(damage);
            }*/
        }
        
        public float GetHealthPoints()
        {
            return health;
        }

        public float GetMaxHealthPoints()
        {
            return baseStats.GetStat(Stat.Health);
        }
        
        private void IncreaseExperience(GameObject attackInitiator)
        {
            Experience experience = attackInitiator.GetComponent<Experience>();
            if (experience == null) return;
            
            experience.GainExperience(baseStats.GetStat(Stat.ExperiencePoints));
        }

        
        //TODO check in dev code
        private void RegenerateHealth()
        {
            //regenerate health to some percentage of max health
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage/100);
            health = Mathf.Max(health, regenHealthPoints);
            health = baseStats.GetStat(Stat.Health);
        }
        
        private void HealingEffect()
        {
            Instantiate(healingParticleEffect, transform);
        }
        
        //heal player after taking heal object on the scene
        public void Heal(float healthToRestore)
        {
            //heal health but no to max health
            health = Mathf.Min(health + healthToRestore, GetMaxHealthPoints());
            HealingEffect();
            onHealing.Invoke();
        }
        
        public float GetHealthPercentage()
        {
            return 100 * GetHealthFraction();
        }
        
        public float GetHealthFraction()
        {
            return health / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        //show death animation
        private void Die()
        {
            //if already dead then do nothing
            if (isDead) return;
            isDead = true;
            //show death animation
            animator.SetTrigger("die");
            
            //if dead cancel any action
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        //method inherited from ISaveble interface that is used for Saving statements in the game.(downloaded from the Saving package)
        public object CaptureState()
        {
            return health;
        }
        
        //method inherited from ISaveble interface that is used for Saving statements in the game.(downloaded from the Saving package)
        public void RestoreState(object state)
        {
            health = (float) state;
            //duplicated from TakeDamage method to do not make enemies alive again after loading savepoint
            if (health == 0)
            {
                //show death animation
                Die();
            }
        }
    }
}