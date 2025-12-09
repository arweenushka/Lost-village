using System;
using Asset_Packs.GameDev.tv_Assets.Scripts.Inventories;
using GameDevTV.Core.UI.Dragging;
using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;
using Inventories.ActionItems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Asset_Packs.GameDev.tv_Assets.Scripts.UI.Inventories
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;

        // STATE
        int index;
        InventoryItem item;
        Inventory inventory;
        
        // CACHE
        ActionStore store;
        private ActionSlotUI actionSlotUI;

        // PUBLIC

        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            inventory.AddItemToSlot(index, item, number);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int GetNumber()
        {
            return inventory.GetNumberInSlot(index);
        }

        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index, number);
        }
        
        //called on event to equip item on double click
        public void EquipItemDirectlyFormInventory()
        {
            if (GetItem() is EquipableItem equipableItem)
            {
                Equipment equipment = inventory.GetComponent<Equipment>();
                EquipableItem equippedItem = equipment.GetItemInSlot(equipableItem.GetAllowedEquipLocation());
                equipment.RemoveItem(equipableItem.GetAllowedEquipLocation());
                equipment.AddItem(equipableItem.GetAllowedEquipLocation(), equipableItem);
                RemoveItems(1);
                if (equippedItem != null)
                {
                    AddItems(equippedItem, 1);
                }
            }
        }

        //called on event to consume item on double click
        public void ConsumeItemDirectlyFromInventory()
        {
            if (GetItem() == null || GetNumber() <1) return;
            if (GetItem() is ActionItem actionItem)
            {
                actionItem.Use(inventory.gameObject);
                if (actionItem.IsConsumable())
                {
                    RemoveItems(1);
                }
            }
        }
        
        //PRIVATE
        private void Awake()
        {
            store = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionStore>();
            actionSlotUI = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionSlotUI>();
            //store.storeUpdated += actionSlotUI.UpdateIcon;
        }
    }
}