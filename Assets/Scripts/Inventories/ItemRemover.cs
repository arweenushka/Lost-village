using Asset_Packs.GameDev.tv_Assets.Scripts.Inventories;
using GameDevTV.Inventories;
using UnityEngine;

namespace Inventories
{
    public class ItemRemover : MonoBehaviour
    {
        [SerializeField] private InventoryItem itemToRemove;
        [SerializeField] private int number;
 
        public void RemoveItem()
        {
            Inventory.GetPlayerInventory().RemoveItem(itemToRemove, number);
            Inventory.GetPlayerInventory().RemoveFromSlot(0,1);
        }
    }
}
