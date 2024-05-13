using TMPro;
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
    private Transform SortingInfoPanel;

    public ShakeEffect shakeEffect; // Reference to the ShakeEffect component attached to the UI GameObject

    private void Start() {
        
        inventory = FindObjectOfType<Inventory>();
        hud = FindObjectOfType<HUD>();
        
        var sortingTransfom = hud.transform.Find("SortingPanel");
        var sortingItemTransfom = sortingTransfom.Find("SortingItemTemplate");

        SortingInfoPanel = sortingTransfom.Find("SortingInfoPanel");

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
        
        KeyCode expectedKey = getKeyPerItemType();

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) ||
                     Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space)) {
            SortingInfoPanel.gameObject.SetActive(false);
            if (Input.GetKeyDown(expectedKey)) {
                SellThisItemAndReset();
            } else {
                if (shakeEffect.TriggerShake()) {
                    SortingInfoPanel.gameObject.SetActive(true);
                    var sortingInfoComp = SortingInfoPanel.Find("SortingInfo");
                    sortingInfoComp.GetComponent<TMP_Text>().text =
                             currentItem.ItemType.ToString() + " items cannot be tossed to this trash bin!!";
                }
                Debug.Log("Wrong bin!");
            }
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
