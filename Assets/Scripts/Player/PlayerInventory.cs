using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField]
    PlayerInfoUi m_playerInfoUi = null;

    [SerializeField]
    List<Item> m_Items = new List<Item>();

    private void Start()
    {
        UpdateUI();
    }

    public void AddItem(ItemType itemType, int quantity)
    {
        Item item = m_Items.Find(i => i.m_itemType == itemType);
        if (item != null)
        {
            item.m_quantity += quantity;
        }
        else
        {
            m_Items.Add(new Item(itemType, quantity));
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
        Item item = m_Items.Find(i => i.m_itemType == itemType);
        if (item != null && item.m_quantity >= quantity)
        {
            item.m_quantity -= quantity;
            if (item.m_quantity <= 0)
            {
                m_Items.Remove(item);
            }
            UpdateUI();
            return true;
        }

        return false;
    }

    public int GetItemQuantity(ItemType itemType)
    {
        Item item = m_Items.Find(i => i.m_itemType == itemType);
        return item != null ? item.m_quantity : 0;
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
    public ItemType m_itemType;
    public int m_quantity;

    public Item(ItemType itemType, int quantity)
    {
        m_itemType = itemType;
        m_quantity = quantity;
    }
}
