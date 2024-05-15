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
    private Transform SortingInfoPanelNext;
    private Transform SortingInfoPanelSecondNext;

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

        var sortingTransfom = hud.transform.Find("SortingPanel");
        if (sortingTransfom == null)
        {
            Debug.LogError("SortingPanel not found in HUD!");
            return;
        }
        SortingInfoPanel = sortingTransfom.Find("SortingInfoPanel");

        var sortingItemTransfom = sortingTransfom.Find("SortingItemTemplate");
        image = sortingItemTransfom.Find("SortingItemImage").GetComponent<Image>();

        var sortingItemTransfomNext = sortingTransfom.Find("SortingItemTemplatePre1");
        imageNext = sortingItemTransfomNext.Find("SortingItemImage").GetComponent<Image>();

        var sortingItemTransfomSecondNext = sortingTransfom.Find("SortingItemTemplatePre2");
        imageSecondNext = sortingItemTransfomSecondNext.Find("SortingItemImage").GetComponent<Image>();

        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("PlayerController reference not found!");
            return;
        }
    }

    private void SetCurrentItem()
    {
        currentItem = inventory.GetFirstInventoryItem();
        nextItem = inventory.GetInventoryItem(1);
        secondNextItem = inventory.GetInventoryItem(2);
    }

    public void SetCorrectImage()
    {
        SetCurrentItem();
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
            default:
                return KeyCode.W;
        }
    }

    public void HandleItems(InventoryItemBase item)
    {
        if (currentItem == null)
            return;

        KeyCode expectedKey = GetKeyPerItemType(item);

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) ||
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
                Debug.Log("Wrong bin!");
            }
        }
    }

    public void HandleInput()
    {
        SetCurrentItem();
        SetCorrectImage();
        HandleItems(currentItem);
        DisablePanel();
    }

    public void DisablePanel()
    {
        if (currentItem != null || nextItem != null || secondNextItem != null)
        {
            hud.transform.Find("SortingPanel").gameObject.SetActive(true);
            player.canInteract = false;
        }
        else
        {
            hud.transform.Find("SortingPanel").gameObject.SetActive(false);
            player.canInteract = true;
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
            // Calculate the position of the target bin
            Vector3 targetPosition = binPositions[binIndex].position;

            // Create a copy of the bottom rectangle
            RectTransform bottomRectCopy = Instantiate(bottomRect, transform);

            // Start the animation
            StartCoroutine(MoveBottomRectToBin(bottomRectCopy, targetPosition));
        }
    }

    // Coroutine to animate the movement of the bottom rectangle to the target position
    private IEnumerator MoveBottomRectToBin(RectTransform rectTransform, Vector3 targetPosition)
    {
        float duration = 0.5f; // Adjust as needed
        float elapsed = 0f;

        Vector3 initialPosition = rectTransform.position;

        while (elapsed < duration)
        {
            // Interpolate position between initial and target position
            rectTransform.position = Vector3.Lerp(initialPosition, targetPosition, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the rectangle reaches the exact target position
        rectTransform.position = targetPosition;

        // Destroy the copy after animation is done
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
