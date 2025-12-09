using Attributes;
using Controllers;
using GameDevTV.Inventories;
using UnityEngine;

namespace Asset_Packs.GameDev.tv_Assets.Scripts.Inventories
{
    /// <summary>
    /// An inventory item that can be placed in the action bar and "Used".
    /// </summary>
    /// <remarks>
    /// This class should be used as a base. Subclasses must implement the `Use`
    /// method.
    /// </remarks>
    ///
    /// comment this as it is an abstract class and onli his child shoud be created in editor
    //[CreateAssetMenu(menuName = ("GameDevTV/GameDevTV.UI.InventorySystem/Action Item"))]
    public abstract class ActionItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("Does an instance of this item get consumed every time it's used.")]
        [SerializeField] bool consumable = false;

        // PUBLIC

        /// <summary>
        /// Trigger the use of this item. Override to provide functionality.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public virtual void Use(GameObject user)
        {
            Debug.Log("Abstract action");
        }

        public bool IsConsumable()
        {
            return consumable;
        }
    }
}