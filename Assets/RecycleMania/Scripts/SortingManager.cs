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
    private Transform SortingInfoPanelSecondNext;

    private int totalFactoryHp;
    public int currentFactoryHp;
    private Transform sortingPanelTransfrom;
    private bool isWorking;

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

        totalFactoryHp = currentFactoryHp =  10;     
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
            }
            else
            {
                if (shakeEffect.TriggerShake())
                {
                    SortingInfoPanel.gameObject.SetActive(true);
                    var sortingInfoComp = SortingInfoPanel.Find("SortingInfo");
                    sortingInfoComp.GetComponent<TMP_Text>().text =
                        item.ItemType.ToString() + " items cannot be tossed to this trash bin!!";
                }
                updateFactoryHp(-1);
                Debug.Log("Wrong bin!");
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
        }
    }

    private void checkFactoryHp() {
        if (currentFactoryHp <= 0) {
            setPanelsActive(false);
            isWorking = false;
        } else {
            setPanelsActive(true);
            isWorking = true;
        }
    }

    private void setPanelsActive(bool value) {
        SortingItemTemplate.gameObject.SetActive(value);
        SortingItemTemplate1.gameObject.SetActive(value);
        SortingItemTemplate2.gameObject.SetActive(value);
        sortingPanelTransfrom.Find("SortingHelpContent").gameObject.SetActive(value);
        sortingPanelTransfrom.Find("FactoryDownText").gameObject.SetActive(!value);
    }

    public void DisablePanel() {
        if (currentItem != null || nextItem != null || secondNextItem != null)
        {
            hud.transform.Find("SortingPanel").gameObject.SetActive(true);
            player.canInteract = false;
        }
        else
        {
            // Start the coroutine to disable the panel after a delay
            StartCoroutine(DisablePanelWithDelay(0.5f)); // Adjust the delay time as needed
        }
    }

    public void SellThisItemAndReset(InventoryItemBase item)
    {
        player.IncreaseCurrency(player.Inventory.RemoveThisItem(item));
        hud.UpdateCurrency(player.currency);
        hud.UpdateRecycledTrash(player.Inventory.getTotalRecycledTrash());

        player.ResetTrashCount();
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
        float duration = 0.5f; // Adjust as needed
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

    // Call this function when the player presses the correct button
    public void OnCorrectButtonPressed(int binIndex)
    {
        AnimateBottomRectToBin(binIndex);
    }

    private void Update()
    {
        HandleInput();
    }
}
