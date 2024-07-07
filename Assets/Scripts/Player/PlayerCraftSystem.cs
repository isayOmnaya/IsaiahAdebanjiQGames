using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCraftSystem : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    PlayerInventory m_playerInventory;

    [SerializeField]
    PlayerHealth m_playerHealth = null;

    [SerializeField]
    PlayerInfoUi m_playerInfoUi = null;

    [SerializeField]
    Button m_craftRumButton;

    [SerializeField]
    Button m_repairButton;


    [Header("CraftRum Requirements")]
    [SerializeField]
    int m_woodRequiredCraft = 500;


    [Header("Ship Repair")]
    [SerializeField]
    float m_repairDuration = 5.0f;

    [SerializeField]
    float m_healthAmount = 50.0f;

    [SerializeField]
    int m_ironRequired = 100;

    [SerializeField]
    int m_woodRequiredRepair = 500;

    [SerializeField]
    Image m_healthBarImage;

    const int Rum_AmountAfterCraft = 5;

    public void CraftRum(int woodRequired, int rumAmount)
    {
        if (m_playerInventory.RemoveItem(ItemType.Wood, woodRequired))
        {
            m_playerInventory.AddItem(ItemType.Rum, rumAmount);
        }
        else
        {
            m_playerInfoUi.DisplayStatus("Not enough resources");
        }
    }

    private void Start()
    {
        m_craftRumButton.onClick.AddListener(OnCraftRumButtonClicked);
        m_repairButton.onClick.AddListener(OnRepairButtonClicked);
        m_playerInfoUi.UpdateRequiredItemCraft(
            m_ironRequired,
            m_woodRequiredCraft,
            m_woodRequiredRepair
        );
    }

    public void StartRepair()
    {
        if (
            m_playerInventory.GetItemQuantity(ItemType.Iron) >= m_ironRequired
            && m_playerInventory.GetItemQuantity(ItemType.Wood) >= m_woodRequiredCraft
            && m_playerHealth.GetCurrentHealth() != m_playerHealth.GetMaxHealth()
        )
        {
            StartCoroutine(RepairShip());
        }
        else
        {
            if (m_playerHealth.GetCurrentHealth() == m_playerHealth.GetMaxHealth())
            {
                m_playerInfoUi.DisplayStatus("Ship Not Damaged");
                return;
            }
            m_playerInfoUi.DisplayStatus("Not enough resources");
        }
    }

    IEnumerator RepairShip()
    {
        m_playerInventory.RemoveItem(ItemType.Iron, m_ironRequired);
        m_playerInventory.RemoveItem(ItemType.Wood, m_woodRequiredCraft);
        m_playerInfoUi.ShowRepairIndicator();
        float elapsedTime = 0f;
        float initialHealth = m_playerHealth.GetCurrentHealth();
        float targetHealth = Mathf.Min(
            initialHealth + m_healthAmount,
            m_playerHealth.GetMaxHealth()
        );

        while (elapsedTime < m_repairDuration)
        {
            elapsedTime += Time.deltaTime;
            float healthPercentage = Mathf.Lerp(
                initialHealth / m_playerHealth.GetMaxHealth(),
                targetHealth / m_playerHealth.GetMaxHealth(),
                elapsedTime / m_repairDuration
            );
            m_healthBarImage.fillAmount = healthPercentage;
            m_playerHealth.SetCurrentHealth(healthPercentage * m_playerHealth.GetMaxHealth());
            yield return null;
        }

        m_healthBarImage.fillAmount = targetHealth / m_playerHealth.GetMaxHealth();
        m_playerInfoUi.CloseRepairIndicator();
        m_playerHealth.SetCurrentHealth(targetHealth);
        m_playerInfoUi.DisplayStatus("Ship Repaired");
    }

    void OnCraftRumButtonClicked()
    {
        CraftRum(m_woodRequiredCraft, Rum_AmountAfterCraft);
    }

    void OnRepairButtonClicked()
    {
        StartRepair();
    }
}
