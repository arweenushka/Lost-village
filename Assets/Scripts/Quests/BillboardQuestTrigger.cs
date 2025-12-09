using UnityEngine;

namespace Quests
{
    //to use if use quest board in the game
    public class BillboardQuestTrigger : MonoBehaviour
    {
        [SerializeField] private QuestGiver questGiver;

        private void OnMouseDown()
        {
            if (questGiver != null)
            {
                Debug.Log("Quest is givens");
                questGiver.GiveQuest();
            }
        }
    }
}