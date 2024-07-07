using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy SpawnPoint
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Component")]
    [SerializeField]
    Destructable m_destructable = null;
    

    [Header("Parameter")]
    [SerializeField]
    float m_timeToSpawnEnemy = 3f;

    ObjectPool m_objectPool;
    EnemyManager m_enemyManager;
    bool m_isInitialized = false;

    void Start()
    {
        if (!m_isInitialized)
        {
            m_objectPool = ServiceLocator.GetObjectPool();
            m_enemyManager = ServiceLocator.GetEnemyManager();

            m_isInitialized = true;
        }
    }

    public void Spawn()
    {
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(m_timeToSpawnEnemy);
        Vector3 position = new Vector3(transform.position.x, 0, transform.position.z);
        Destructable enemyObject = m_objectPool.SpawnFromPool(
            PoolTag.Enemy,
            position,
            Quaternion.identity
        );

        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.ActivateNavMeshAgent();

        m_enemyManager.RegisterEnemy(enemy);

        if (m_destructable != null)
        {
            PoolTag enemySpawnerTag = m_destructable.GetDestructTag();
            m_objectPool.ReturnToPool(enemySpawnerTag, m_destructable);
        }
    }
}
