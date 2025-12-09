using Asset_Packs.GameDev.tv_Assets.Scripts.Inventories;
using Attributes;
using UnityEngine;

namespace Inventories.ActionItems
{
    [CreateAssetMenu(menuName = ("Inventories/Action Item/Health Potion Item"))]
    public class HealthPotionItem : ActionItem
    {
        private Health health;

        public override void Use(GameObject user)
        {
            Debug.Log("Taking health potion");
            health = user.GetComponent<Health>();
            health.Heal(10);
            
            //if we want to increase health only if it is less then full
           /* if (health.GetHealthFraction() < 1.0f)
            {
                Debug.Log("Taking health potion");
                health.Heal(10);
            }*/
        }
    }
}