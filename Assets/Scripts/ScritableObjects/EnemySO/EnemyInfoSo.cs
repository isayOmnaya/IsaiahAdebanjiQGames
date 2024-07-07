using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyInfo")]
public class EnemyInfoSo : ScriptableObject
{
    public string m_EnemyName;
    public int m_IronAmount;
    public int m_RumAmount;
    public int m_WoodAmount;
    public int m_EnemyHealth;
}

[System.Serializable]
public class EnemyInfo
{
    public string m_EnemyName;
    public int m_IronAmount;
    public int m_RumAmount;
    public int m_WoodAmount;
    public int m_EnemyHealth;

    public void GenerateRandomName()
    {
        List<string> prefixes = new List<string> { "Black", "Red", "Iron", "Ghost", "Golden" };
        List<string> suffixes = new List<string> { "Skull", "Blade", "Heart", "Hunter", "Wolf" };

        string prefix = prefixes[Random.Range(0, prefixes.Count)];
        string suffix = suffixes[Random.Range(0, suffixes.Count)];

        m_EnemyName = prefix + " " + suffix;
    }

    public void GenerateRandomStats(
        int minIron,
        int maxIron,
        int minRum,
        int maxRum,
        int minWood,
        int maxWood,
        int minHealth,
        int maxHealth
    )
    {
        m_IronAmount = Random.Range(minIron, maxIron + 1);
        m_RumAmount = Random.Range(minRum, maxRum + 1);
        m_WoodAmount = Random.Range(minWood, maxWood + 1);
        m_EnemyHealth = Random.Range(minHealth, maxHealth + 1);
    }
}
