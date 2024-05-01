using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recycler : MonoBehaviour
{
    public HUD hud;

    public void SellAllItems(PlayerController player) {
        player.IncreaseCurrency(player.Inventory.RemoveAllItems() * 10);
        hud.UpdateCurrency(player.currency);
        
        player.ResetTrashCount();
        hud.UpdateTrash(player.trashCount);
        
        //hud.ResetInventorySlots();
    }
}
