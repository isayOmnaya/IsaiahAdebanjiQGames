using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUi : MonoBehaviour
{
    [Header("Player Cargos")]
    [SerializeField]
    TMP_Text m_currentIronTxt = null;

    [SerializeField]
    TMP_Text m_currentRumTxt = null;

    [SerializeField]
    TMP_Text m_currentRumHudDisplayText = null;

    [SerializeField]
    TMP_Text m_currentWoodTxt = null;


    [Header("Craft && Repair UI")]
    [SerializeField]
    TMP_Text m_RequiredIronTxt = null;

    [SerializeField]
    TMP_Text m_RequiredWoodRepairTxt = null;

    [SerializeField]
    TMP_Text m_RequiredWoodCraftTxt = null;
    

    [Header("Parameter")]
    [SerializeField]
    TMP_Text m_statusText = null;

    [SerializeField]
    float m_statusTime = 0.1f;

    [SerializeField]
    float m_statusDisplayTime = .5f;

    [SerializeField]
    GameObject m_repairIndicator = null;

    public void UpdatePlayerCargo(int currentIron, int currentRum, int currentWood)
    {
        m_currentIronTxt.text = currentIron.ToString();
        m_currentRumTxt.text = currentRum.ToString();
        m_currentWoodTxt.text = currentWood.ToString();
        m_currentRumHudDisplayText.text = currentRum.ToString();
    }

    public void UpdateRequiredItemCraft(
        int requiredIron,
        int requiredWoodRepair,
        int requiredWoodCraft
    )
    {
        m_RequiredIronTxt.text = requiredIron.ToString();
        m_RequiredWoodRepairTxt.text = requiredWoodRepair.ToString();
        m_RequiredWoodCraftTxt.text = requiredWoodCraft.ToString();
    }

    IEnumerator ShowStatus(string message)
    {
        yield return new WaitForSeconds(m_statusTime);
        m_statusText.text = message;

        yield return new WaitForSeconds(m_statusDisplayTime);
        m_statusText.text = "";
    }

    public void DisplayStatus(string message)
    {
        StartCoroutine(ShowStatus(message));
    }

    public void ShowRepairIndicator()
    {
        m_repairIndicator.SetActive(true);
    }

    public void CloseRepairIndicator()
    {
        m_repairIndicator.SetActive(false);
    }
}
