using Asset_Packs.GameDev.tv_Assets.Scripts.Inventories;
using UnityEngine;

namespace Inventories.ActionItems
{
    [CreateAssetMenu(menuName = ("Inventories/Action Item/Scroll Item"))]
    public class ScrollItem : ActionItem
    {
        public override void Use(GameObject user)
        {
            Debug.Log("Using scroll");
        }
    }
}