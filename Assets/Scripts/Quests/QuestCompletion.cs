using UnityEngine;

namespace Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] Quest quest;
        [SerializeField] string objective;

        public void CompleteObjective()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();

			//check whether the player has the quest
			if (questList.HasQuest(quest)) questList.CompleteObjective(quest, objective);
        }
    }
}
