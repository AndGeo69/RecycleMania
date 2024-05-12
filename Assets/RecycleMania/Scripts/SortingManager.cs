using UnityEngine;
using UnityEngine.UI;

public class SortingManager : MonoBehaviour
{

    private Inventory inventory;
    private HUD hud;
    private Image image;
    private bool hasInventoryItemSet = false;
    
    private InventoryItemBase currentItem;
    private PlayerController player;

    private void Start() {
        
        inventory = FindObjectOfType<Inventory>();
        hud = FindObjectOfType<HUD>();
        
        var sortingTransfom = hud.transform.Find("SortingPanel");
        var sortingItemTransfom = sortingTransfom.Find("SortingItemTemplate");
        image = sortingItemTransfom.Find("SortingItemImage").GetComponent<Image>();

        player = FindObjectOfType<PlayerController>();

        SetCorrectImage();
    }

    public void SetCorrectImage() {
        if (!hasInventoryItemSet) {
            Sprite sprite = getImageFromType();
            if (sprite == null) {
                // no next item, should show a well done msg and close dialog.
                hud.transform.Find("SortingPanel").gameObject.SetActive(false);
                player.canInteract = true;
                Debug.Log("no next item or image not found..");
                return;
            }
            image.sprite = sprite;
            hasInventoryItemSet = true;
        }
    }

    private void SetCurrentItem() {
         currentItem = inventory.GetFirstInventoryItem();
    }

    public Sprite getImageFromType() {

        Texture2D texture;
        SetCurrentItem();
        if (currentItem == null) {
            return null;
        }

        EItemType type = currentItem.ItemType;

        if (type == EItemType.Default) {
            texture = Resources.Load<Texture2D>("lead_money");
        }

        texture = type switch
        {
            EItemType.Glass => Resources.Load<Texture2D>("glass-bottle"),
            EItemType.Organic_Waste => Resources.Load<Texture2D>("organic"),
            EItemType.Aluminium => Resources.Load<Texture2D>("drink-can"),
            _ => Resources.Load<Texture2D>("leaf_money"),
        };

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);;
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
        SetCurrentItem();
        SetCorrectImage();
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
