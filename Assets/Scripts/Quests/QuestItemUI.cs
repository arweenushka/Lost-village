using TMPro;
using UnityEngine;

namespace Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI progress;

        private QuestStatus status;
        public void Setup(QuestStatus status)
        {
            this.status = status;
            if (status.IsComplete())
            {
                title.text = $"<color=green><s>{status.GetQuest().GetTitle()}</s></color>";
                progress.text = $"<color=green>{status.GetCompletedCount()}/{status.GetQuest().GetObjectiveCount()}</color>";
            }
            else
            {
                title.text = status.GetQuest().GetTitle();
                progress.text = status.GetCompletedCount() + "/" + status.GetQuest().GetObjectiveCount();
            }
        }
        
        public QuestStatus GetQuestStatus()
        {
            return status;
        }
    }
}
