using Asset_Packs.GameDev.tv_Assets.Scripts.Inventories;
using UnityEngine;

namespace Inventories.ActionItems
{
    [CreateAssetMenu(menuName = ("Inventories/Action Item/Mana Potion Item"))]
    public class ManaPotionItem : ActionItem
    {
        public override void Use(GameObject user)
        {
            Debug.Log("Add + 10 mana");
        }
    }
}