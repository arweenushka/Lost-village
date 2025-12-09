using Attributes;
using UnityEngine;

namespace Combat
{
    public class AttackOnHit : MonoBehaviour
    {
        [Tooltip("Optional: group to wake up together (e.g., nearby guards)")]
        [SerializeField] private AggroGroup aggroGroup;

        private Health health;
        private Fighter fighter;

        private void Awake()
        {
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>(); // may be disabled initially
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.onDamageTaken += HandleDamaged; // ensure Health exposes this event
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.onDamageTaken -= HandleDamaged;
            }
        }

        private void HandleDamaged(GameObject attacker, float amount)
        {
            if (health.IsDead()) return;
            if (attacker == null) return;

            // Wake up this NPC’s Fighter
            EnableFighter(true);

            // Optional: wake up group (guards etc.)
            if (aggroGroup != null)
            {
                aggroGroup.Activate(true);
            }

            // Optional: immediately target the attacker
            var attackerHealth = attacker.GetComponent<Health>();
            if (fighter != null && attackerHealth != null && !attackerHealth.IsDead())
            {
                fighter.Attack(attacker);
            }
        }

        private void EnableFighter(bool enable)
        {
            if (fighter == null) return;

            // Enable both Fighter and CombatTarget, so player can lock onto them after they wake
            var target = fighter.GetComponent<CombatTarget>();
            if (target != null) target.enabled = enable;

            fighter.enabled = enable;
        }
    }
}
