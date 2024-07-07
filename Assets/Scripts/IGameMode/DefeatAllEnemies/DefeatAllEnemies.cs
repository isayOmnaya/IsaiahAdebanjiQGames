using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatAllEnemies : IGameMode
{
    EnemyManager m_enemyManager;

    public void Initialize()
    {
        m_enemyManager = ServiceLocator.GetEnemyManager();
    }

    public bool CheckWinCondition()
    {
        return m_enemyManager.AreAllEnemiesDefeated();
    }

    public bool CheckGameOverCondition()
    {
        return false;
    }
}
