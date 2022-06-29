using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class InventoryItemData
    {
        public GameObject slot;
        public GameObject needItem;
        public BoxCollider2D rightPlace;
        public bool isFull;
    }

    public List<InventoryItemData> inventoryItem = new List<InventoryItemData>();
    public int totalNumberFillSlots = 0;

    public delegate void AllowDrag();
    public static event AllowDrag AllowDragEvent;

    private void OnEnable()
    {
        MoveToInventory.FillingSlotInventoryEvent += FillSlot;
    }

    private void OnDisable()
    {
        MoveToInventory.FillingSlotInventoryEvent -= FillSlot;
    }

    private void Start()
    {
        for (int i = 0; i < inventoryItem.Count; i++)
        {
            inventoryItem[i].rightPlace.enabled = false;
        }
    }

    private void FillSlot()
    {
        totalNumberFillSlots++;

        if (totalNumberFillSlots > 2)
        {
            AllowDragEvent();

            for (int i = 0; i < inventoryItem.Count; i++)
            {
                inventoryItem[i].rightPlace.enabled = true;
            }
        }
    }
}


