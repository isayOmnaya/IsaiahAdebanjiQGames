using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoLoot : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    Destructable m_destructable = null;
    int m_rumAmount = 0;
    int m_ironAmount = 0;
    int m_woodAmount = 9;

    public void SetLoot(int rum, int iron, int wood)
    {
        m_rumAmount = rum;
        m_ironAmount = iron;
        m_woodAmount = wood;
    }

    public int GetRumAmount()
    {
        return m_rumAmount;
    }

    public int GetIronAmount()
    {
        return m_ironAmount;
    }

    public int GetWoodAmount()
    {
        return m_woodAmount;
    }

    public Destructable GetDestructable()
    {
        return m_destructable;
    }
}
