using UnityEngine;
using UnityEngine.UI;

public class SortingManager : MonoBehaviour
{

    private Inventory inventory;
    private HUD hud;
    private Image image;
    private bool hasInventoryItemSet = false;
    
    private InventoryItemBase currentItem;

    private void Start() {
        
        inventory = FindObjectOfType<Inventory>();
        hud = FindObjectOfType<HUD>();
        
        var sortingTransfom = hud.transform.Find("SortingPanel");
        var sortingItemTransfom = sortingTransfom.Find("SortingItemTemplate");
        image = sortingItemTransfom.Find("SortingItemImage").GetComponent<Image>();

        SetCorrectImage();
    }

    public void SetCorrectImage() {
        if (!hasInventoryItemSet) {
            image.sprite = getImageFromType();
            hasInventoryItemSet = true;
        }
    }

    public Sprite getImageFromType() {

        Sprite sprite;
        currentItem = inventory.GetFirstInventoryItem();
        if (currentItem == null) {
            sprite = Resources.Load<Sprite>("I/mages/coin/star coin rotate 1.png");
        }

        EItemType type = currentItem.ItemType;

        if (type == EItemType.Default) {
            sprite = Resources.Load<Sprite>("/Images/coin/star coin rotate 1.png");
        }

        sprite = type switch
        {
            EItemType.Glass => Resources.Load<Sprite>("/Images/glass-bottle"),
            EItemType.Organic_Waste => Resources.Load<Sprite>("/Images/organic"),
            EItemType.Aluminium => Resources.Load<Sprite>("/Images/drink-can"),
            _ => Resources.Load<Sprite>("/Images/coin/star coin rotate 1"),
        };

        return sprite;
    }

    private KeyCode getKeyPerItemType() {
        if (currentItem == null) {return KeyCode.Q;}

        return currentItem.ItemType switch
        {
            EItemType.Glass => KeyCode.A,
            EItemType.Organic_Waste => KeyCode.Space,
            EItemType.Aluminium => KeyCode.S,
            _ => KeyCode.W,
        };

    }

    public void HandleInput() {
        if (currentItem == null) {return;}
        
        if (getKeyPerItemType() == KeyCode.W) {
            // show wrong message, loose hp or smth
            Debug.Log("Wrong bin !");
            return;
        }

        if (getKeyPerItemType() == KeyCode.Q) {
            // show wrong message, loose hp or smth
            Debug.Log("No current item !");
            return;
        }

        if (Input.GetKeyDown(KeyCode.A) && getKeyPerItemType() == KeyCode.A) {
            SellThisItemAndReset();
        } else if (Input.GetKeyDown(KeyCode.S)  && getKeyPerItemType() == KeyCode.S) {
            SellThisItemAndReset();
        } else if (Input.GetKeyDown(KeyCode.D)  && getKeyPerItemType() == KeyCode.D) {
            SellThisItemAndReset();
        } else if (Input.GetKeyDown(KeyCode.Space)  && getKeyPerItemType() == KeyCode.Space) {
            SellThisItemAndReset();
        }
    }

    public void SellThisItemAndReset() {
        PlayerController player = FindObjectOfType<PlayerController>();

        player.IncreaseCurrency(player.Inventory.RemoveThisItem(currentItem));
        hud.UpdateCurrency(player.currency);
        hud.UpdateRecycledTrash(player.Inventory.getTotalRecycledTrash());
        
        player.ResetTrashCount();
        hud.UpdateTrash(player.trashCount);
        hasInventoryItemSet = false;

        SetCorrectImage();
    }

    private void Update() {
        HandleInput();
    }
}
