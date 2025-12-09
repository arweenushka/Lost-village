using UnityEngine;
using UnityEngine.Audio;

namespace Controllers
{
    public class CombatAudioController : MonoBehaviour
    {
        [SerializeField] private AudioMixerSnapshot defaultSnapshot;
        [SerializeField] private AudioMixerSnapshot combatSnapshot;
        [SerializeField] private float transitionTime = 0.5f;

        private int combatRefs = 0; // reference count for overlapping combat

        void Awake()
        {
            if (defaultSnapshot != null) defaultSnapshot.TransitionTo(0f);
        }
        public void EnterCombat()
        {
            combatRefs++;
            Debug.Log($"EnterCombat refs={combatRefs}");
            combatSnapshot.TransitionTo(transitionTime);
        }

        public void ExitCombat()
        {
            combatRefs = Mathf.Max(0, combatRefs - 1);
            Debug.Log($"ExitCombat refs={combatRefs}");
            if (combatRefs == 0) defaultSnapshot.TransitionTo(transitionTime);
        }
    }
}