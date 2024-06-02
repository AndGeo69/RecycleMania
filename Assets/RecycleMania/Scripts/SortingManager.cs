using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SortingManager : MonoBehaviour
{
    private Inventory inventory;
    private HUD hud;
    private Image image;
    private Image imageNext;
    private Image imageSecondNext;
    private InventoryItemBase currentItem;
    private InventoryItemBase nextItem;
    private InventoryItemBase secondNextItem;
    private PlayerController player;
    private Transform SortingInfoPanel;
    private Transform SortingItemTemplate;
    private Transform SortingItemTemplate1;
    private Transform SortingItemTemplate2;
    public int currentFactoryHp;
    private Transform sortingPanelTransfrom;
    private bool isWorking;
    private int successInARowTimes = 0;

    public ShakeEffect shakeEffect; // Reference to the ShakeEffect component attached to the UI GameObject

    private void Start()
    {
        InitializeReferences();
        SetCorrectImage();
    }

    private void InitializeReferences()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory reference not found!");
            return;
        }

        hud = FindObjectOfType<HUD>();
        if (hud == null)
        {
            Debug.LogError("HUD reference not found!");
            return;
        }

        sortingPanelTransfrom = hud.transform.Find("SortingPanel");
        if (sortingPanelTransfrom == null)
        {
            Debug.LogError("SortingPanel not found in HUD!");
            return;
        }
        SortingInfoPanel = sortingPanelTransfrom.Find("SortingInfoPanel");

        SortingItemTemplate = sortingPanelTransfrom.Find("SortingItemTemplate");
        image = SortingItemTemplate.Find("SortingItemImage").GetComponent<Image>();

        SortingItemTemplate1 = sortingPanelTransfrom.Find("SortingItemTemplatePre1");
        imageNext = SortingItemTemplate1.Find("SortingItemImage").GetComponent<Image>();

        SortingItemTemplate2 = sortingPanelTransfrom.Find("SortingItemTemplatePre2");
        imageSecondNext = SortingItemTemplate2.Find("SortingItemImage").GetComponent<Image>();

        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("PlayerController reference not found!");
            return;
        }

        isWorking = true;  
    }


    private void updateFactoryHp(int amount) {
        currentFactoryHp += amount;
        var sortingHpPanel = sortingPanelTransfrom.Find("HPPanel");
        sortingHpPanel.Find("HPText").GetComponent<TMP_Text>().text = currentFactoryHp.ToString();
    }

    private void SetCurrentItem()
    {
        currentItem = inventory.GetFirstInventoryItem();
        nextItem = inventory.GetInventoryItem(1);
        secondNextItem = inventory.GetInventoryItem(2);
    }

    private void disablePanelsIfNecesery() { // must be called afeter setcurrentitem()
        if (nextItem == null) {
            SortingItemTemplate1.gameObject.SetActive(false);
        } else {
            SortingItemTemplate1.gameObject.SetActive(true);
        }

        if (secondNextItem == null) {
            SortingItemTemplate2.gameObject.SetActive(false);
        } else {
            SortingItemTemplate2.gameObject.SetActive(true);
        }
    }

    public void SetCorrectImage()
    {
        SetCurrentItem();

        disablePanelsIfNecesery();
        SetImage(currentItem, image);
        SetImage(nextItem, imageNext);
        SetImage(secondNextItem, imageSecondNext);
    }

    private void SetImage(InventoryItemBase item, Image targetImage)
    {
        if (item == null)
            return;

        Sprite sprite = GetSpriteFromType(item.ItemType);
        if (sprite == null)
            return;

        targetImage.sprite = sprite;
    }

    public Sprite GetSpriteFromType(EItemType type)
    {
        Texture2D texture;
        switch (type)
        {
            case EItemType.Glass:
                texture = Resources.Load<Texture2D>("glass-bottle");
                break;
            case EItemType.Organic_Waste:
                texture = Resources.Load<Texture2D>("organic");
                break;
            case EItemType.Aluminium:
                texture = Resources.Load<Texture2D>("drink-can");
                break;
            case EItemType.Plastic:
                texture = Resources.Load<Texture2D>("plastic-bottle");
                break;
            case EItemType.Paper:
                texture = Resources.Load<Texture2D>("paper-icon");
                break;
            default:
                texture = Resources.Load<Texture2D>("recycling-menu");
                break;
        }

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

   private KeyCode GetKeyPerItemType(InventoryItemBase item)
    {
        if (item == null)
            return KeyCode.Q;

        switch (item.ItemType)
        {
            case EItemType.Glass:
                return KeyCode.A;
            case EItemType.Organic_Waste:
                return KeyCode.Space;
            case EItemType.Aluminium:
                return KeyCode.S;
            case EItemType.Plastic:
                return KeyCode.W;
            case EItemType.Paper:
                return KeyCode.D;
            default:
                return KeyCode.Break;
        }
    }

    private string GetSortingMessage(EItemType itemType, KeyCode pressedKey)
    {
        switch (itemType)
        {
            case EItemType.Glass:
                if (pressedKey == KeyCode.D) return "Glass items cannot be placed into the paper recycling bin because they are made of different materials and require separate recycling processes. Glass is melted down at high temperatures, while paper is pulped and processed differently. Mixing these materials can contaminate the recycling stream, making it harder to recycle both effectively. Please place paper items in the paper recycling bin.";
                if (pressedKey == KeyCode.W) return "Glass items cannot be placed into the plastic recycling bin because glass and plastic require different recycling processes. Glass is melted at very high temperatures, while plastic is processed differently. Mixing plastic with glass can contaminate the glass recycling stream. Please place plastic items in the plastic recycling bin.";
                if (pressedKey == KeyCode.S) return "Glass items cannot be placed into the aluminium recycling bin because they are processed using different methods. Aluminium is melted and reformed, while glass requires a different melting process. Mixing these materials can contaminate the glass recycling stream. Please place aluminium items in the aluminium recycling bin.";
                if (pressedKey == KeyCode.Space) return "Glass items cannot be placed into the organic recycling bin because they do not decompose like organic materials. Organic waste includes food scraps and biodegradable materials, while glass needs to be recycled through melting. Please place organic items in the organic waste bin.";
                break;
            case EItemType.Organic_Waste:
                if (pressedKey == KeyCode.D) return "Organic waste items cannot be placed into the paper bin unless they are specifically labeled as compostable. Regular paper can contain inks and coatings that are not suitable for composting. It’s important to recycle paper properly to ensure it can be turned into new paper products. Please place paper items in the paper recycling bin.";
                if (pressedKey == KeyCode.W) return "Organic waste items cannot be placed into the plastic bin because they do not decompose naturally like organic waste. Organic waste includes food scraps and other biodegradable materials, while plastic can take hundreds of years to break down. Please place plastic items in the plastic recycling bin.";
                if (pressedKey == KeyCode.S) return "Organic waste items cannot be placed into the aluminium bin because they do not decompose naturally. Organic waste includes biodegradable materials, while aluminium is recycled through melting and reforming. Please place aluminium items in the aluminium recycling bin.";
                if (pressedKey == KeyCode.A) return "Organic waste items cannot be placed into the Glass bin because they do not decompose like organic materials. Organic waste includes food scraps and biodegradable materials, while glass needs to be recycled through melting. Please place glass items in the glass recycling bin.";
                break;
            case EItemType.Aluminium:
                if (pressedKey == KeyCode.D) return "Aluminium items cannot be placed into the paper recycling bin because they are processed in different ways. Paper is pulped and recycled into new paper products, while aluminium is melted and reformed. Mixing paper with aluminium can contaminate the aluminium recycling stream. Please place paper items in the paper recycling bin.";
                if (pressedKey == KeyCode.W) return "Aluminium items cannot be placed into the plastic recycling bin because they require different recycling processes. Plastic is melted and reformed into new products, while aluminium is melted and reformed through a different process. Mixing plastic with aluminium can contaminate the aluminium recycling stream. Please place plastic items in the plastic recycling bin.";
                if (pressedKey == KeyCode.Space) return "Aluminium items cannot be placed into the organice waste recycling bin because they decompose naturally, while aluminium does not. Organic waste should be composted to break down, while aluminium is melted and reformed. Please place organic items in the organic waste bin.";
                if (pressedKey == KeyCode.A) return "Aluminium items cannot be placed into the glass recycling bin because they require different recycling processes. Glass is melted at a higher temperature, while aluminium is melted and reformed differently. Mixing glass with aluminium can contaminate the aluminium recycling stream. Please place glass items in the glass recycling bin.";
                break;
            case EItemType.Plastic:
                if (pressedKey == KeyCode.D) return "Plastic items cannot be placed into the paper recycling bin because they are processed differently. Paper is pulped and recycled into new paper products, while plastic is melted and reformed. Mixing paper with plastic can contaminate the plastic recycling stream. Please place paper items in the paper recycling bin.";
                if (pressedKey == KeyCode.S) return "Plastic items cannot be placed into the aluminium recycling bin because they require different recycling processes. Aluminium is melted and reformed, while plastic is melted and processed differently. Mixing aluminium with plastic can contaminate the plastic recycling stream. Please place aluminium items in the aluminium recycling bin.";
                if (pressedKey == KeyCode.Space) return "Plastic items cannot be placed into the organice waste recycling bin because they decompose naturally, while plastic does not. Organic waste should be composted, while plastic is recycled through melting and reforming. Please place organic items in the organic waste bin.";
                if (pressedKey == KeyCode.A) return "Plastic items cannot be placed into the glass recycling bin because they require different recycling processes. Glass is melted at higher temperatures, while plastic is melted and processed differently. Mixing glass with plastic can contaminate the plastic recycling stream. Please place glass items in the glass recycling bin.";
                break;
            case EItemType.Paper:
                if (pressedKey == KeyCode.W) return "Plastic items cannot be placed into the paper recycling bin because they are made of different materials. Paper is pulped and turned into new paper products, while plastic is melted and reformed. Mixing plastic with paper can contaminate the paper recycling stream. Please place plastic items in the plastic recycling bin.";
                if (pressedKey == KeyCode.S) return "Aluminium items cannot be placed into the paper recycling bin because they are made of different materials. Paper is pulped and recycled into new paper products, while aluminium is melted and reformed. Mixing aluminium with paper can contaminate the paper recycling stream. Please place aluminium items in the aluminium recycling bin.";
                if (pressedKey == KeyCode.Space) return "Organic items cannot be placed into the paper recycling bin unless they are specifically labeled as compostable. Regular paper can contain inks and coatings that are not suitable for composting. Please place organic items in the organic waste bin.";
                if (pressedKey == KeyCode.A) return "Glass items cannot be placed into the paper recycling bin because they are processed very differently. Paper is pulped and turned into new paper products, while glass is melted down to form new glass items. Glass in the paper stream can cause contamination and damage recycling equipment. Please place glass items in the glass recycling bin.";
                break;
        }
        return "This item cannot be placed into this bin!";
    }

    private void playRandomSuccSound(bool certainPlay) {
        float chance = 0.4f;

        bool play = false;

        if (certainPlay) {
            play = true;
        } else {
            if (UnityEngine.Random.value < chance) {
                play = true;
            }
        }

        if (play) {
            SimpleSoundPlayer.PlayRandomSound(new string[]{"succ1", "succ3"});
        }
    }

    public void HandleItems(InventoryItemBase item)
    {
        if (currentItem == null)
            return;

        KeyCode expectedKey = GetKeyPerItemType(item);

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space))
        {
            SortingInfoPanel.gameObject.SetActive(false);
            if (Input.GetKeyDown(expectedKey))
            {
                OnCorrectButtonPressed((int)item.ItemType);
                SellThisItemAndReset(item);
                successInARowTimes++;
                playRandomSuccSound(successInARowTimes >= 3);
            }
            else
            {
                successInARowTimes = 0;
                if (shakeEffect.TriggerShake())
                {
                    SortingInfoPanel.gameObject.SetActive(true);
                    var sortingInfoComp = SortingInfoPanel.Find("SortingInfo");
                    KeyCode pressedKey = KeyCode.None;

                    if (Input.GetKeyDown(KeyCode.A)) pressedKey = KeyCode.A;
                    if (Input.GetKeyDown(KeyCode.S)) pressedKey = KeyCode.S;
                    if (Input.GetKeyDown(KeyCode.W)) pressedKey = KeyCode.W;
                    if (Input.GetKeyDown(KeyCode.D)) pressedKey = KeyCode.D;
                    if (Input.GetKeyDown(KeyCode.Space)) pressedKey = KeyCode.Space;

                    string message = GetSortingMessage(item.ItemType, pressedKey);
                    sortingInfoComp.GetComponent<TMP_Text>().text = message;
                }
                // updateFactoryHp(-1);
                Debug.Log("Wrong bin!");
                SimpleSoundPlayer.PlayWarningSound();
            }
        }
    }

    private IEnumerator DisablePanelWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        hud.transform.Find("SortingPanel").gameObject.SetActive(false);
        player.canInteract = true;
    }


    public void HandleInput()
    {
        // checkFactoryHp();
        if (isWorking) {
            SetCurrentItem();
            SetCorrectImage();
            HandleItems(currentItem);
            DisablePanel();
            inventory.CheckWinCondition();
        }
    }

    public void DisablePanel() {
        if (currentItem != null || nextItem != null || secondNextItem != null)
        {
            hud.transform.Find("SortingPanel").gameObject.SetActive(true);
            player.canInteract = false;
        }
        else
        {
            StartCoroutine(DisablePanelWithDelay(0.5f));
            SimpleSoundPlayer.PlaySound("succ2");
        }
    }

    public void SellThisItemAndReset(InventoryItemBase item)
    {
        player.IncreaseCurrency(player.Inventory.RemoveThisItem(item));
        hud.UpdateCurrency(player.currency);
        hud.UpdateRecycledTrash(player.Inventory.getTotalRecycledTrash());

        player.IncreaseTrashCount(-1);
        hud.UpdateTrash(player.trashCount);
        SetCorrectImage();
    }

    public RectTransform bottomRect;
    public RectTransform[] binPositions; // Positions of the bins where the bottom rectangle will animate to

    // Function to move the bottom rectangle to the position of the correct bin
    public void AnimateBottomRectToBin(int binIndex)
    {
        if (binIndex >= 0 && binIndex < binPositions.Length)
        {
            Vector3 targetPosition = binPositions[binIndex].position;
            RectTransform bottomRectCopy = Instantiate(bottomRect, transform);
            StartCoroutine(MoveBottomRectToBin(bottomRectCopy, targetPosition));
        }
    }

    private IEnumerator MoveBottomRectToBin(RectTransform rectTransform, Vector3 targetPosition)
    {
        float duration = 0.5f; 
        float elapsed = 0f;

        Vector3 initialPosition = rectTransform.position;

        while (elapsed < duration)
        {
            rectTransform.position = Vector3.Lerp(initialPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.position = targetPosition;
        Destroy(rectTransform.gameObject);
    }

    public void OnCorrectButtonPressed(int binIndex)
    {
        AnimateBottomRectToBin(binIndex);
    }

    private void Update()
    {
        HandleInput();
    }
}
