using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField]
    PlayerInfoUi m_playerInfoUi = null;

    [SerializeField]
    List<Item> m_items = new List<Item>();

    private void Start()
    {
        UpdateUI();
    }

    public void AddItem(ItemType itemType, int quantity)
    {
        Item item = m_items.Find(i => i.m_ItemType == itemType);
        if (item != null)
        {
            item.m_Quantity += quantity;
        }
        else
        {
            m_items.Add(new Item(itemType, quantity));
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        int currentIron = GetItemQuantity(ItemType.Iron);
        int currentRum = GetItemQuantity(ItemType.Rum);
        int currentWood = GetItemQuantity(ItemType.Wood);

        m_playerInfoUi.UpdatePlayerCargo(currentIron, currentRum, currentWood);
    }

    public bool RemoveItem(ItemType itemType, int quantity)
    {
        Item item = m_items.Find(i => i.m_ItemType == itemType);
        if (item != null && item.m_Quantity >= quantity)
        {
            item.m_Quantity -= quantity;
            if (item.m_Quantity <= 0)
            {
                m_items.Remove(item);
            }
            UpdateUI();
            return true;
        }

        return false;
    }

    public int GetItemQuantity(ItemType itemType)
    {
        Item item = m_items.Find(i => i.m_ItemType == itemType);
        return item != null ? item.m_Quantity : 0;
    }
}

public enum ItemType
{
    Iron,
    Wood,
    Rum
}

[System.Serializable]
public class Item
{
    public ItemType m_ItemType;
    public int m_Quantity;

    public Item(ItemType itemType, int quantity)
    {
        m_ItemType = itemType;
        m_Quantity = quantity;
    }
}
