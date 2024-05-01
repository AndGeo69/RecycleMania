using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour, IShopCustomer
{
    #region Private Members

    private Animator _animator;

    private CharacterController _characterController;

    private float Gravity = 20.0f;

    private Vector3 _moveDirection = Vector3.zero;

    private InventoryItemBase mCurrentItem = null;
    
    private SphereCollider sphereCollider;
    // private HealthBar mHealthBar;

    // private HealthBar mFoodBar;

    // private int startHealth;

    // private int startFood;

    private bool mCanTakeDamage = true;

    #endregion

    #region Public Members

    public float Speed = 5.0f;

    public float RotationSpeed = 240.0f;

    public Inventory Inventory;

    public GameObject Hand;

    public HUD Hud;

    public float JumpSpeed = 7.0f;

    public event EventHandler PlayerDied;

    [Header("Currency")]
    public int currency = 0;

    [Header("Trash collected")]
    public int trashCount = 0;

    public List<InteractableItemBase> itemsInRange = new List<InteractableItemBase>();


    #endregion

    public void IncreaseCurrency(int amount) {
        currency += amount;
    }

    public void IncreaseTrashCount(int amount) {
        trashCount += amount;
    }

    public void ResetTrashCount() {
        trashCount = 0;
    }

    public UnityEvent QuestCompleted;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        // Inventory.ItemUsed += Inventory_ItemUsed;
        Inventory.ItemRemoved += Inventory_ItemRemoved;
        Hud.UpdateCurrency(currency);
        Hud.UpdateTrash(trashCount);

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = 1.3f;
        

        // mHealthBar = Hud.transform.Find("Bars_Panel/HealthBar").GetComponent<HealthBar>();
        // mHealthBar.Min = 0;
        // mHealthBar.Max = Health;
        // startHealth = Health;
        // mHealthBar.SetValue(Health);

        // mFoodBar = Hud.transform.Find("Bars_Panel/FoodBar").GetComponent<HealthBar>();
        // mFoodBar.Min = 0;
        // mFoodBar.Max = Food;
        // startFood = Food;
        // mFoodBar.SetValue(Food);

        // InvokeRepeating("IncreaseHunger", 0, HungerRate);
    }


    #region Inventory

    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        InventoryItemBase item = e.Item;

        GameObject goItem = (item as MonoBehaviour).gameObject;
        goItem.SetActive(true);
        goItem.transform.parent = null;

        if (item == mCurrentItem)
            mCurrentItem = null;

    }

    private void SetItemActive(InventoryItemBase item, bool active)
    {
        GameObject currentItem = (item as MonoBehaviour).gameObject;
        currentItem.SetActive(active);
        currentItem.transform.parent = active ? Hand.transform : null;
    }


    private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        if (e.Item.ItemType != EItemType.Consumable)
        {
            // If the player carries an item, un-use it (remove from player's hand)
            if (mCurrentItem != null)
            {
                SetItemActive(mCurrentItem, false);
            }

            InventoryItemBase item = e.Item;

            // Use item (put it to hand of the player)
            SetItemActive(item, true);

            mCurrentItem = e.Item;
        }

    }

    private int Attack_1_Hash = Animator.StringToHash("Base Layer.Attack_1");

    public bool IsAttacking
    {
        get
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.fullPathHash == Attack_1_Hash)
            {
                return true;
            }
            return false;
        }
    }

    public void DropCurrentItem()
    {
        mCanTakeDamage = false;

        _animator.SetTrigger("tr_drop");

        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;

        Inventory.RemoveItem(mCurrentItem);

        // Throw animation
        Rigidbody rbItem = goItem.AddComponent<Rigidbody>();
        if (rbItem != null)
        {
            rbItem.AddForce(transform.forward * 2.0f, ForceMode.Impulse);

            Invoke("DoDropItem", 0.25f);
        }
    }

    public void DropAndDestroyCurrentItem()
    {
        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;

        Inventory.RemoveItem(mCurrentItem);

        Destroy(goItem);

        mCurrentItem = null;
    }

    public void DoDropItem()
    {
        mCanTakeDamage = true;
        if (mCurrentItem != null)
        {
            // Remove Rigidbody
            Destroy((mCurrentItem as MonoBehaviour).GetComponent<Rigidbody>());

            mCurrentItem = null;

            mCanTakeDamage = true;
        }
    }

    #endregion

    #region Health & Hunger

    // [Tooltip("Amount of health")]
    // public int Health = 100;

    // [Tooltip("Amount of food")]
    // public int Food = 100;

    // [Tooltip("Rate in seconds in which the hunger increases")]
    // public float HungerRate = 0.5f;

    // public void IncreaseHunger()
    // {
    //     Food--;
    //     if (Food < 0)
    //         Food = 0;

    //     mFoodBar.SetValue(Food);

    //     if (Food == 0)
    //     {
    //         CancelInvoke();
    //         Die();
    //     }
    // }

    // public bool IsDead
    // {
    //     get
    //     {
    //         return Health == 0 || Food == 0;
    //     }
    // }

    // public bool CarriesItem(string itemName)
    // {
    //     if (mCurrentItem == null)
    //         return false;

    //     return (mCurrentItem.Name == itemName);
    // }

    // public InventoryItemBase GetCurrentItem()
    // {
    //     return mCurrentItem;
    // }

    // public bool IsArmed
    // {
    //     get
    //     {
    //         if (mCurrentItem == null)
    //             return false;

    //         return mCurrentItem.ItemType == EItemType.Weapon;
    //     }
    // }


    // public void Eat(int amount)
    // {
    //     Food += amount;
    //     if (Food > startFood)
    //     {
    //         Food = startFood;
    //     }

    //     mFoodBar.SetValue(Food);

    // }

    // public void Rehab(int amount)
    // {
    //     Health += amount;
    //     if (Health > startHealth)
    //     {
    //         Health = startHealth;
    //     }

    //     mHealthBar.SetValue(Health);
    // }

    // public void TakeDamage(int amount)
    // {
    //     if (!mCanTakeDamage)
    //         return;

    //     Health -= amount;
    //     if (Health < 0)
    //         Health = 0;

    //     mHealthBar.SetValue(Health);

    //     if (IsDead)
    //     {
    //         Die();
    //     }

    // }


    // private void Die()
    // {
    //     _animator.SetTrigger("death");

    //     if (PlayerDied != null)
    //     {
    //         PlayerDied(this, EventArgs.Empty);
    //     }
    // }

    #endregion


    public void Talk()
    {
        _animator.SetTrigger("tr_talk");
    }

    private bool mIsControlEnabled = true;

    public void EnableControl()
    {
        mIsControlEnabled = true;
    }

    public void DisableControl()
    {
        mIsControlEnabled = false;
    }

    private Vector3 mExternalMovement = Vector3.zero;

    public Vector3 ExternalMovement
    {
        set
        {
            mExternalMovement = value;
        }
    }

    void FixedUpdate()
    {
        // if (!IsDead)
        // {
            // Drop item
            if (mCurrentItem != null && Input.GetKeyDown(KeyCode.R))
            {
                DropCurrentItem();
            }
        // }
    }

    void LateUpdate()
    {
        if (mExternalMovement != Vector3.zero)
        {
            _characterController.Move(mExternalMovement);
        }
    }


    InteractableItemBase GetClosestItem(List<InteractableItemBase> items)
    {
        InteractableItemBase bestTarget = null;

        if (items.Count > 0) {
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach(InteractableItemBase potentialTarget in items)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
        }
     
        return bestTarget;
    }

    #region Update & Interact
    // Update is called once per frame
    void Update()
    {
        if (mIsControlEnabled)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                InteractableItemBase itemBase = GetClosestItem(itemsInRange);
                if (itemBase != null) {
                    // Interact animation - plays event to InteractWithItem
                    itemBase.OnInteractAnimation(_animator);
                }
               
            }

            // Interact with the item
            // if (mInteractItem != null && Input.GetKeyDown(KeyCode.F))
            // {
            //     // Interact animation - plays event to InteractWithItem
            //     mInteractItem.OnInteractAnimation(_animator);
            // }

            // Execute action with item
            if (mCurrentItem != null && Input.GetMouseButtonDown(0))
            {
                // Dont execute click if mouse pointer is over uGUI element
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    // TODO: Logic which action to execute has to come from the particular item
                    _animator.SetTrigger("attack_1");
                }
            }

            // Get Input for axis
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Calculate the forward vector
            Vector3 camForward_Dir = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 move = v * camForward_Dir + h * Camera.main.transform.right;

            if (move.magnitude > 1f) move.Normalize();


            // Calculate the rotation for the player
            move = transform.InverseTransformDirection(move);

            // Get Euler angles
            float turnAmount = Mathf.Atan2(move.x, move.z);

            transform.Rotate(0, turnAmount * RotationSpeed * Time.deltaTime, 0);

            if (_characterController.isGrounded || mExternalMovement != Vector3.zero)
            {
                _moveDirection = transform.forward * move.magnitude;

                _moveDirection *= Speed;

                if (Input.GetButton("Jump"))
                {
                    _animator.SetBool("is_in_air", true);
                    _moveDirection.y = JumpSpeed;

                }
                else
                {
                    _animator.SetBool("is_in_air", false);
                    _animator.SetBool("run", move.magnitude > 0);
                }
            }
            else
            {
                Gravity = 20.0f;
            }


            _moveDirection.y -= Gravity * Time.deltaTime;

            _characterController.Move(_moveDirection * Time.deltaTime);
        }
    }

    public void InteractWithItem() //called by animator
    {
        List<InteractableItemBase> items = itemsInRange;

        if (items.Count > 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                InventoryItemBase item = (InventoryItemBase)items[i];
                if (item != null)
                {
                    item.OnInteract();

                    if (item is InventoryItemBase)
                    {
                        InventoryItemBase inventoryItem = item as InventoryItemBase;
                        if (Inventory.AddItem(item))
                        {
                            item.OnPickup(Hud); //add sound fx onpickup and particles

                            IncreaseTrashCount(1);
                            Hud.UpdateTrash(trashCount);
                            itemsInRange.Remove(item);
                            // Hud.CloseMessagePanel();
                            // mInteractItem = null;
                        }

                    }
                }
            }
        }


        // if (mInteractItem != null)
        // {
        //     mInteractItem.OnInteract();

        //     if (mInteractItem is InventoryItemBase)
        //     {
        //         InventoryItemBase inventoryItem = mInteractItem as InventoryItemBase;
        //         if (Inventory.AddItem(inventoryItem)) {
        //             inventoryItem.OnPickup(Hud); //add sound fx onpickup and particles

        //             IncreaseTrashCount(1);
        //             Hud.UpdateTrash(trashCount);

        //             // Hud.CloseMessagePanel();
        //             mInteractItem = null;
        //         }
                
        //     }
        // }
    }

    private InteractableItemBase mInteractItem = null;

    
    private void OnTriggerEnter(Collider other)
    {
        TryInteraction(other);
    }

    private void TryInteraction(Collider other)
    {
        InteractableItemBase item = other.GetComponent<InteractableItemBase>();

        List<InteractableItemBase> itemsAroundPlayer = other.GetComponents<InteractableItemBase>().ToList();

        if (itemsAroundPlayer.Count > 0) {
            Hud.OpenMessagePanel(itemsAroundPlayer[0]);
            itemsInRange.Add(GetClosestItem(itemsAroundPlayer));
        }


        // if (item != null)
        // {
        //     if (item.CanInteract(other))
        //     {
        //         mInteractItem = item;

        //         Hud.OpenMessagePanel(mInteractItem);
        //     }
        // }
    }

    private void OnTriggerExit(Collider other)
    {
        // InteractableItemBase item = other.GetComponent<InteractableItemBase>();

        InteractableItemBase[] itemsAroundPlayer = other.GetComponents<InteractableItemBase>();

        if (itemsAroundPlayer.Length > 0) {
            Hud.CloseMessagePanel();
            foreach (InteractableItemBase item in itemsAroundPlayer.ToList())
            {
                 itemsInRange.Remove(item);
            }
           
        }


        // if (item != null)
        // {
        //     Hud.CloseMessagePanel();
        //     mInteractItem = null;
        // }
    }

    #region Upgrades
    public bool ReachedMaxUpgrade(Upgrades.UpgradeType upgradeType) {
        bool reachedMax = false;

        switch (upgradeType) {
            case Upgrades.UpgradeType.PickupRange:
                if (Upgrades.GetUpgradeMaxValue(upgradeType) <= sphereCollider.radius) {
                    reachedMax = true;
                }
                break;
            case Upgrades.UpgradeType.MovementSpeed:
                if (Upgrades.GetUpgradeMaxValue(upgradeType) <= Speed) {
                    reachedMax = true;
                }
                break;

            case Upgrades.UpgradeType.CarryingCapacity:
                if (Upgrades.GetUpgradeMaxValue(upgradeType) <= Inventory.getCapacity()) {
                    reachedMax = true;
                }
                break;            
        }

        if (reachedMax) {
            Hud.OpenMessagePanelTimed("Upgrade maxed out!");
        }
        
        return reachedMax;
    }

    public void BoughtUpgrade(Upgrades.UpgradeType upgradeType)
    {
        Debug.Log("bought upgrade " + upgradeType.ToString());
        float value;

        switch (upgradeType)
        {
            case Upgrades.UpgradeType.PickupRange:
                value = Upgrades.GetUpgradeValue(Upgrades.UpgradeType.PickupRange);
                sphereCollider.radius += value;
                break;
            case Upgrades.UpgradeType.MovementSpeed:
                value = Upgrades.GetUpgradeValue(Upgrades.UpgradeType.MovementSpeed);
                Speed += value;
                break;
            case Upgrades.UpgradeType.CarryingCapacity:
                value = Upgrades.GetUpgradeValue(Upgrades.UpgradeType.CarryingCapacity);
                Inventory.IncreaseCapacity((int)Math.Round(value));
                break;

            default:
                break;
        }
        
    }

    public bool TrySpendLeafPointsAmount(int spendAmount) {
        if (currency >= spendAmount) {
            IncreaseCurrency(-spendAmount);
            Hud.UpdateCurrency(currency);
            return true;
        } else {
            Hud.OpenMessagePanelTimed("Can't afford upgrade!");
            return false;
        }
    }
    #endregion


    #endregion
}
