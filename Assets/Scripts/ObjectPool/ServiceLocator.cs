using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static ObjectPool m_objectPoolInstance;
    private static EnemyManager m_enemyManagerInstance;
    private static SoundManager m_soundManagerInstance;
    private static StageLoop m_stageLoopInstance;

    public static event Action OnEnemyManagerRegistered;

    public static void RegisterObjectPool(ObjectPool objectPool)
    {
        m_objectPoolInstance = objectPool;
    }

    public static ObjectPool GetObjectPool()
    {
        return m_objectPoolInstance;
    }

    public static void RegisterEnemyManager(EnemyManager enemyManager)
    {
        m_enemyManagerInstance = enemyManager;
        OnEnemyManagerRegistered?.Invoke();
    }

    public static EnemyManager GetEnemyManager()
    {
        return m_enemyManagerInstance;
    }

    public static void RegisterSoundManager(SoundManager soundManager)
    {
        m_soundManagerInstance = soundManager;
    }

    public static SoundManager GetSoundManager()
    {
        return m_soundManagerInstance;
    }

    public static void RegisterStageLoop(StageLoop stageLoop)
    {
        m_stageLoopInstance = stageLoop;
    }

    public static StageLoop GetStageLoop()
    {
        return m_stageLoopInstance;
    }
}
