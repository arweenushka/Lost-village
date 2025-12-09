using System.Collections;
using System.Collections.Generic;
using Asset_Packs.GameDev.tv_Assets.Scripts.Inventories;
using GameDevTV.Inventories;
using Inventories;
using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        Pickup pickup;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        public CursorType GetCursorType()
        {
            if (pickup.CanBePickedUp())
            {
                return CursorType.PickUp;
            }
            else
            {
                return CursorType.FullPickup;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
             if (Input.GetMouseButtonDown(0))
             {
                 //player can pick up only if stay close to pick up object, so we are sending are doing this check
                 //in ItemCollector class
                 ItemCollector itemCollector = callingController.GetComponent<ItemCollector>();
                 itemCollector.CollectItem(pickup);
             }
             return true;
        }
    }
}