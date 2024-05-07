using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 9;

    private int capacity = 10;
    public int getCapacity() {
        return capacity;
    }

    public void IncreaseCapacity(int amount) {
        capacity += amount;
    }

    private IList<InventorySlot> mSlots = new List<InventorySlot>();
    public IList<InventorySlot> getInvetorySlots() {
        return mSlots;
    }

    private IList<InventoryItemBase> allItems = new List<InventoryItemBase>();

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;
    // public event EventHandler<InventoryEventArgs> ItemUsed;

    public Inventory()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            mSlots.Add(new InventorySlot(i));
        }
    }

    private InventorySlot FindStackableSlot(InventoryItemBase item)
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.IsStackable(item))
                return slot;
        }
        return null;
    }

    private InventorySlot FindNextEmptySlot()
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.IsEmpty)
                return slot;
        }
        return null;
    }

    public bool AddItem(InventoryItemBase item)
    {
        if (CanAddMoreItems()) {
            allItems.Add(item);
            return true;
        } else {
            ItemAdded(this, new InventoryEventArgs(item));
            return false;
        }
    }

    public bool CanAddMoreItems() {
        return allItems.Count < capacity;
         
    }

    public void RemoveItem(InventoryItemBase item)
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.Remove(item))
            {
                if (ItemRemoved != null)
                {
                    ItemRemoved(this, new InventoryEventArgs(item));
                }
                break;
            }

        }
    }

    public int RemoveAllItems() {
        int itemCount = allItems.Count;

        if (itemCount > 0) {
            allItems.Clear();
        }

        return itemCount;

        // if (mSlots.Count > 0) {
        //     foreach (InventorySlot slot in mSlots) {               
        //         itemCount += slot.RemoveStackItems();
        //     }
        // }
        // return itemCount;
    }
}
