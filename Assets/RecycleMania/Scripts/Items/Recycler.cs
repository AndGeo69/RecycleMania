using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recycler : MonoBehaviour
{
    public HUD hud;


    public void ShowSortingPanel(PlayerController player) {
        if (player.Inventory.hasItems()) {
            if (!hud.SortingPanel.activeInHierarchy) {
                hud.SortingPanel.SetActive(true);
            }
            
            player.canInteract = false;
        }
    }

    public void CloseSortingPanel(PlayerController player) {
        if (hud.SortingPanel.activeInHierarchy) {
            hud.SortingPanel.SetActive(false);
        }

        player.canInteract = true;
    }

    public void SellAllItems(PlayerController player) {
        player.IncreaseCurrency(player.Inventory.RemoveAllItems() * 10);
        hud.UpdateCurrency(player.currency);
        hud.UpdateRecycledTrash(player.Inventory.getTotalRecycledTrash());
        
        player.ResetTrashCount();
        hud.UpdateTrash(player.trashCount);
        
        //hud.ResetInventorySlots();
    }
}
