using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTriggerCollider : MonoBehaviour
{
    [SerializeField] private UI_Shop uIShop;
    private void OnTriggerEnter(Collider other) {
        IShopCustomer shopCustomer = other.GetComponent<IShopCustomer>();
        if (shopCustomer != null) {
            uIShop.Show(shopCustomer);
        }
    }

    private void OnTriggerExit(Collider other) {
        IShopCustomer shopCustomer = other.GetComponent<IShopCustomer>();
        if (shopCustomer != null) {
            uIShop.Hide();
        }
    }
}
