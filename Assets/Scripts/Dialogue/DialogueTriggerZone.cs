using UnityEngine;
using Dialogue;
using Quests;
using Saving;

namespace Dialogue
{
    public class DialogueTriggerZone : MonoBehaviour, ISaveable
    {
        private AIConversant aiConversant;
        [SerializeField] private bool hasTriggered = false;

        private void Start()
        {
            // Get the AIConversant component from the parent NPC
            aiConversant = GetComponentInParent<AIConversant>();
            if (hasTriggered) gameObject.SetActive(false);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered) return;
            if (!other.CompareTag("Player") || aiConversant == null) return;

            var playerConversant = other.GetComponent<PlayerConversant>();
            if (playerConversant != null)
            {
                hasTriggered = true;
                playerConversant.MoveToAndStartDialogue(aiConversant, aiConversant.Dialogue);
                gameObject.SetActive(false);
            }
        }

        // ——— сохранение/загрузка ———
        public object CaptureState()
        {
            return hasTriggered;
        }

        public void RestoreState(object state)
        {
            if (state is bool b)
            {
                hasTriggered = b;
                if (hasTriggered) gameObject.SetActive(false);
            }
        }
    }
}
