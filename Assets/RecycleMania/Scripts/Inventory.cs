using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 9;

    private int capacity = 10;
    public int getCapacity() {
        return capacity;
    }

    [SerializeField] 
    private int RecycledTrashRequiredToWin = 150;

    private int TotalRecycledTrash;

    public int getTotalRecycledTrash() {return TotalRecycledTrash;}
    public int getRecycledTrashRequiredToWin() {return RecycledTrashRequiredToWin;}

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
    public event EventHandler<EventArgs> GameWon;

    public Inventory()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            mSlots.Add(new InventorySlot(i));
        }
        TotalRecycledTrash = 0;
    }

    public void CheckWinCondition() {
        if (getTotalRecycledTrash() >= getRecycledTrashRequiredToWin()) {
            GameWon(this, new EventArgs());
        }
    }

    public bool hasItems() {
        return allItems.Count > 0;

    }

    public InventoryItemBase GetFirstInventoryItem() {
        if (allItems.Count > 0) {
            return allItems[0];
        }
        return null;
    }

    public InventoryItemBase GetInventoryItem(int position) {
        if (allItems.Count > position) {
            return allItems[position];
        }
        return null;
    }

    public bool AddItem(InventoryItemBase item)
    {
        if (CanAddMoreItems()) {
            allItems.Add(item);
            return true;
        } else {
            SimpleSoundPlayer.PlayWarningSound();
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
        TotalRecycledTrash += itemCount;
        CheckWinCondition();

        if (itemCount > 0) {
            allItems.Clear();
        }

        return itemCount;
    }

    public int RemoveThisItem(InventoryItemBase thisItem)
    {
        if (thisItem == null || !allItems.Contains(thisItem))
        {
            return 0;
        }

        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] == thisItem)
            {
                TotalRecycledTrash++;
                allItems.RemoveAt(i);
                return 10; // Return 10 if the item is successfully removed
            }
        }

        return 0; // Return 0 if the item is not found
    }   
}
