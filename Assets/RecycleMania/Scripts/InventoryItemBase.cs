using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    Glass,
    Aluminium,
    Paper,
    Organic_Waste,
    Plastic,
    Default,
    Consumable,
    Weapon,
}

public class InteractableItemBase : MonoBehaviour, IInteractable
{
    public string Name;

    public Sprite Image;

    public string InteractText = "Press F to pickup";

    public EItemType ItemType;

    public virtual void OnInteractAnimation(Animator animator)
    {
        animator.SetTrigger("tr_pickup");
    }

    public virtual void OnInteract()
    {
    }

    public virtual bool CanInteract(Collider other)
    {
        return true;   
    }

    public void Interact()
    {
        Debug.Log("interacted");
    }
}

public class InventoryItemBase : InteractableItemBase
{
    public InventorySlot Slot
    {
        get; set;
    }

    public virtual void OnDrop()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000))
        {
            gameObject.SetActive(true);
            gameObject.transform.position = hit.point;
            gameObject.transform.eulerAngles = DropRotation;
        }
    }

    public virtual void OnPickup(HUD hud)
    {
        SimpleSoundPlayer.PlayRandomSound(new string[]{"pick_up", "pick_up_2", "pick_up_3"});
        Destroy(gameObject.GetComponent<Rigidbody>());
        gameObject.SetActive(false);
        if (hud != null) {
            hud.CloseMessagePanel();
        }
    }

    public Vector3 PickPosition;

    public Vector3 PickRotation;

    public Vector3 DropRotation;

    public bool UseItemAfterPickup = false;


}
