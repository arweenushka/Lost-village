using Asset_Packs.GameDev.tv_Assets.Scripts.Inventories;
using Core;
using Movement;
using UnityEngine;

namespace Controllers
{
    //we use this class to calculate if we can pickup an object with distance close enough to do it
    public class ItemCollector:MonoBehaviour, IAction
    {
        private Pickup pickup;
 
 
        private void Update()
        {
            if (pickup == null) return;
            if (Vector3.Distance(transform.position, pickup.transform.position) > 2.0f)
            {
                GetComponent<Mover>().MoveTo(pickup.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().CancelAction();
                pickup.PickupItem();
                pickup = null;
            }
        }
 
        public void CollectItem(Pickup pickup)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            this.pickup = pickup;
        }
        
        public void CancelAction()
        {
            pickup = null;
        }
    }
}