using Attributes;
using Combat;
using Controllers;
using UnityEngine;
using Dialogue;
using Quests;

namespace Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue dialogue;
        [SerializeField] string conversantName;
		[SerializeField] Quest npcQuest; // NEW: quest given by this NPC
        
        private Health health;
        private Fighter fighter;
        private PlayerConversant playerConversant;

		public Dialogue Dialogue => dialogue;
		public Quest NpcQuest => npcQuest;

        private void Awake()
        {
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
        }

        private void Start()
        {
            // Find the PlayerConversant component on the player
            playerConversant = FindObjectOfType<PlayerConversant>();
        }
        
        public CursorType GetCursorType()
        {
            //return CursorType.Dialogue;
			return CanInteract() ? CursorType.Dialogue : CursorType.None;
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            /*if (!enabled || dialogue == null || (TryGetComponent(out health) && health.IsDead()))
            {
                return false;
            }*/
            if (!CanInteract()) return false;

            if (Input.GetMouseButtonDown(0))
            {
                playerController.GetComponent<PlayerConversant>().MoveToAndStartDialogue(this, dialogue);
            }
            return true;
        }

        public string GetAIConversantName()
        {
            return conversantName;
        }
        // ===== helper methods =====

        private bool CanInteract()
        {
            // original guards
            if (!enabled || dialogue == null || (TryGetComponent(out health) && health.IsDead()))
            {
                return false;
            }

            // NEW: block interaction if NPC quest is taken and completed
            if (npcQuest != null)
            {
                var playerGO = GameObject.FindGameObjectWithTag("Player");
                if (playerGO != null)
                {
                    var questList = playerGO.GetComponent<QuestList>();
                    if (questList != null)
                    {
                        var status = GetQuestStatus(questList, npcQuest);
                        if (status != null && status.IsComplete())
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private QuestStatus GetQuestStatus(QuestList questList, Quest quest)
        {
            foreach (var s in questList.GetStatuses())
            {
                if (s.GetQuest() == quest) return s;
            }
            return null;
        }
    }
}