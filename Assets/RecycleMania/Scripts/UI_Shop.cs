using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;

public class UI_Shop : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;
    private IShopCustomer customer;

    private void Awake() {
        container = transform.Find("Container");
        shopItemTemplate = container.Find("ShopItemTemplate");
        shopItemTemplate.gameObject.SetActive(false);
    }

    private void Start() {
        CreateUpgradeButton(Upgrades.UpgradeType.CarryingCapacity, 0);
        CreateUpgradeButton(Upgrades.UpgradeType.MovementSpeed, 1);
        CreateUpgradeButton(Upgrades.UpgradeType.PickupRange, 2);
        Hide();
    }

    private void CreateUpgradeButton(Upgrades.UpgradeType upgrade, int positionIndex) {
        Transform shopUpgradeTransfrom = Instantiate(shopItemTemplate, container);
        RectTransform shopUpgradeRectTransfrom = shopUpgradeTransfrom.GetComponent<RectTransform>();

        float shopUpgradeHeight = 140f;
        shopUpgradeRectTransfrom.anchoredPosition = new Vector2(0, -shopUpgradeHeight * positionIndex);

        shopUpgradeTransfrom.Find("UpgradeNameText").GetComponent<TextMeshProUGUI>().SetText(upgrade.ToString());
        shopUpgradeTransfrom.Find("PriceText").GetComponent<TextMeshProUGUI>().SetText(Upgrades.GetCost(upgrade).ToString());

        shopUpgradeTransfrom.GetComponent<Button_UI>().ClickFunc = () => {
            TryBuy(upgrade);
        };
        shopUpgradeTransfrom.gameObject.SetActive(true);
    }

    private void TryBuy(Upgrades.UpgradeType upgrade)
    {
        if (!customer.ReachedMaxUpgrade(upgrade) && customer.TrySpendLeafPointsAmount(Upgrades.GetCost(upgrade))) {
            customer.BoughtUpgrade(upgrade);
        } else {
            Debug.Log("cannt afford to buy: " + upgrade);
        }
    }

    public void Show(IShopCustomer customer) {
        this.customer = customer;
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

}
