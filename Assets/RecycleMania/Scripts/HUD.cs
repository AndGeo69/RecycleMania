using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public Inventory Inventory;
    public GameObject currencyPanel;
    public GameObject trashPanel;

    public GameObject MessagePanel;
    
    public GameObject RecycledTrashText;

    public GameObject Info;

     public GameObject SortingPanel;

	// Use this for initialization
	void Start () {
        Inventory.ItemAdded += InventoryScript_ItemAdded;
        Inventory.ItemRemoved += Inventory_ItemRemoved;
        Inventory.GameWon += Game_Won;
        UpdateOutOfTrashText(Inventory.getRecycledTrashRequiredToWin());
	}

    public void UpdatePanel(GameObject panel, String textPanelName, int amount) {
        Text currencyText = panel.transform.Find(textPanelName).GetComponent<Text>();
        currencyText.text = amount.ToString();
    }

    public void UpdateCurrency(int currency) {
        UpdatePanel(currencyPanel, "CurrencyText", currency);
    }

    public void UpdateTrash(int amount) {
        UpdatePanel(trashPanel, "TrashText", amount);
    }

    public void UpdateRecycledTrash(int amount) {
        UpdateTextMeshProUGUIPanel(RecycledTrashText, "RecycledTrashValueText", amount);
    }

    public void UpdateOutOfTrashText(int amount) {
        UpdateTextMeshProUGUIPanel(RecycledTrashText, "OutOfTrashText", amount);
    }

    public void UpdateTextMeshProUGUIPanel(GameObject panel, String textPanelName, int amount) {
        TextMeshProUGUI currencyText = panel.transform.Find(textPanelName).GetComponent<TextMeshProUGUI>();
        currencyText.text = amount.ToString();
    }

    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        OpenMessagePanelTimed("Inventory full, upgrade to increase capacity");
    }

    private void Game_Won(object sender, EventArgs e) {
        SceneManager.LoadScene("GameWonScene");
        Debug.Log("Game Won!!");
    }

    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");

        int index = -1;
        foreach (Transform slot in inventoryPanel)
        {
            index++;

            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform textTransform = slot.GetChild(0).GetChild(1);

            Image image = imageTransform.GetComponent<Image>();
            Text txtCount = textTransform.GetComponent<Text>();

            // ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

            // We found the item in the UI
            // if (itemDragHandler.Item == null)
            //     continue;

            // Found the slot to remove from
            if(e.Item.Slot.Id == index)
            {
                int itemCount = e.Item.Slot.Count;
                // itemDragHandler.Item = e.Item.Slot.FirstItem;

                if(itemCount < 2)
                {
                    txtCount.text = "";
                }
                else
                {
                    txtCount.text = itemCount.ToString();
                }

                if(itemCount == 0)
                {
                    image.enabled = false;
                    image.sprite = null;
                }
                break;
            }
           
        }
    }

    public void ResetInventorySlots() {
        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach (Transform slot in inventoryPanel)
        {
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform textTransform = slot.GetChild(0).GetChild(1);

            Image image = imageTransform.GetComponent<Image>();
            Text txtCount = textTransform.GetComponent<Text>();

            txtCount.text = "";
            image.enabled = false;
            image.sprite = null;

            break;
        }
    }

    private bool mIsMessagePanelOpened = false;

    public bool IsMessagePanelOpened
    {
        get { return mIsMessagePanelOpened; }
    }

    public void OpenMessagePanel(InteractableItemBase item)
    {
        MessagePanel.SetActive(true);

        Text mpText = MessagePanel.transform.Find("Text").GetComponent<Text>();

        if (item.InteractText != "") {
            mpText.text = item.InteractText;
        }
        
        mIsMessagePanelOpened = true;
    }


    public void OpenMessagePanelTimed(String text) {
        OpenMessagePanelTimed(text, 1);
    }

    public void OpenMessagePanelTimed(String text, float seconds) {
        if (isOpenedTimed) {return;}
        
        OpenMessagePanel(text);

        StartCoroutine(CloseAfterSeconds(seconds));
    }

    private bool isOpenedTimed = false;
    IEnumerator CloseAfterSeconds(float seconds)
    {
        isOpenedTimed = true;
        yield return new WaitForSeconds(seconds);
        CloseMessagePanel();
        isOpenedTimed = false;
    }

    public void OpenMessagePanel(string text)
    {
        MessagePanel.SetActive(true);

        Text mpText = MessagePanel.transform.Find("Text").GetComponent<Text>();
        mpText.text = text;


        mIsMessagePanelOpened = true;
    }

    public void CloseMessagePanel()
    {
        MessagePanel.SetActive(false);

        mIsMessagePanelOpened = false;
    }

    public static explicit operator HUD(GameObject v)
    {
        throw new NotImplementedException();
    }
}
