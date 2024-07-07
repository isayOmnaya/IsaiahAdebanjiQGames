using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    Camera m_mainCamera;

    [SerializeField]
    Transform m_player = null;

    [SerializeField]
    UIManager m_uiManager = null;


    [Header("Random Enemy Stats")]
    [SerializeField]
    int m_minIronAmount = 10;

    [SerializeField]
    int m_maxIronAmount = 50;

    [SerializeField]
    int m_minRumAmount = 5;

    [SerializeField]
    int m_maxRumAmount = 30;

    [SerializeField]
    int m_minWoodAmount = 20;

    [SerializeField]
    int m_maxWoodAmount = 100;

    [SerializeField]
    int m_minHealth = 100;

    [SerializeField]
    int m_maxHealth = 500;


    [Header("Parameter")]
    int m_maxActiveEnemies = 2;
    int m_totalEnemies = 5;

    [SerializeField]
    Vector3 m_enemyInfoScale;

    [SerializeField]
    float m_initialWaitSpawnTime = 2f;

    int m_enemiesLeft;
    List<Enemy> m_enemiesControlled = new List<Enemy>();
    ObjectPool m_objectPool = null;
    int m_pendingSpawns = 0;
    int m_enemiesKilled = 0;

    void Start()
    {
        ServiceLocator.RegisterEnemyManager(this);
        m_objectPool = ServiceLocator.GetObjectPool();

        m_maxActiveEnemies = GameModeConfiguration.MaxActiveEnemies;
        m_totalEnemies = GameModeConfiguration.TotalEnemies;

        m_enemiesLeft = m_totalEnemies;
        StartCoroutine(WaitTimeToSpawn());
    
        UpdateEnemyCount();
    }

    void Update()
    {
        CheckActiveEnemies();
        AssignWaypointsToEnemies();
    }

    void SpawnInitialEnemies()
    {
        for (int i = 0; i < m_maxActiveEnemies; i++)
        {
            SpawnEnemySpawner();
        }
    }

    void UpdateEnemyCount()
    {
        if (m_uiManager != null)
        {
            m_uiManager.UpdateEnemyCount(m_enemiesKilled, m_totalEnemies);
        }
    }

    IEnumerator WaitTimeToSpawn()
    {
        yield return new WaitForSeconds(m_initialWaitSpawnTime);
        SpawnInitialEnemies();
    }

    void SpawnEnemySpawner()
    {
        if (m_enemiesLeft > 0)
        {
            m_pendingSpawns++;
            Destructable enemySpawnerDestruct = m_objectPool.SpawnFromPool(
                PoolTag.EnemySpawner,
                GetRandomPointInCameraView(),
                Quaternion.identity
            );
            m_enemiesLeft--;
            EnemySpawner enemySpawner = enemySpawnerDestruct.GetComponent<EnemySpawner>();
            if (enemySpawner != null)
            {
                enemySpawner.Spawn();
            }
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        m_enemiesControlled.Add(enemy);
        enemy.CurrentState = EnemyState.Wait;
        m_pendingSpawns--;
        enemy.m_IsEnemyActive = true;

        AssignRandomEnemyInfoToEnemies(enemy);
    }

    void AssignRandomEnemyInfoToEnemies(Enemy enemy)
    {
        EnemyInfo newEnemyInfo = new EnemyInfo();
        newEnemyInfo.GenerateRandomName();
        newEnemyInfo.GenerateRandomStats(
            m_minIronAmount,
            m_maxIronAmount,
            m_minRumAmount,
            m_maxRumAmount,
            m_minWoodAmount,
            m_maxWoodAmount,
            m_minHealth,
            m_maxHealth
        );
        enemy.SetEnemyInfo(newEnemyInfo, newEnemyInfo.m_EnemyHealth, newEnemyInfo.m_EnemyName);
    }

    public void DeregisterEnemy(Enemy enemy)
    {
        m_enemiesControlled.Remove(enemy);
        m_enemiesKilled++;

        var enemyInfo = enemy.m_EnemyInfoText;
        if (enemyInfo != null)
        {
            Destructable destructEnemyInfo = enemyInfo.GetComponent<Destructable>();
            m_objectPool.ReturnToPool(PoolTag.EnemyInfo, destructEnemyInfo);
        }

        if (m_enemiesControlled.Count < m_maxActiveEnemies && m_enemiesLeft > 0)
        {
            SpawnEnemySpawner();
        }
        UpdateEnemyCount();
    }

    void CheckActiveEnemies()
    {
        for (int i = 0; i < m_enemiesControlled.Count; i++)
        {
            Enemy enemy = m_enemiesControlled[i];
            if (!enemy.m_IsEnemyActive)
            {
                DeregisterEnemy(enemy);
            }
        }
    }

    public bool AreAllEnemiesDefeated()
    {
        bool allDefeated =
            m_enemiesLeft <= 0 && m_enemiesControlled.Count == 0 && m_pendingSpawns == 0;
        return allDefeated;
    }

    void AssignWaypointsToEnemies()
    {
        for (int i = 0; i < m_enemiesControlled.Count; i++)
        {
            if (m_enemiesControlled[i].CurrentState == EnemyState.Idle)
            {
                AssignDynamicWaypoint(m_enemiesControlled[i]);
            }
            GetLastPlayerLocation(m_enemiesControlled[i]);
        }
    }

    void AssignDynamicWaypoint(Enemy enemy)
    {
        Vector3 randomPoint = GetRandomPointInCameraView();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
        {
            enemy.m_TargetWaypoint = hit.position;
            enemy.CurrentState = EnemyState.Moving;
        }
        else
        {
            enemy.CurrentState = EnemyState.Wait;
        }
    }

    void GetLastPlayerLocation(Enemy enemy)
    {
        enemy.PlayerLastPosition = m_player.position;
    }

    Vector3 GetRandomPointInCameraView()
    {
        float camHeight = 2f * m_mainCamera.orthographicSize;
        float camWidth = camHeight * m_mainCamera.aspect;

        float minX = m_mainCamera.transform.position.x - (camWidth / 2f) + m_mainCamera.rect.xMin;
        float maxX = m_mainCamera.transform.position.x + (camWidth / 2f) - m_mainCamera.rect.xMax;
        float minY = m_mainCamera.transform.position.z - (camHeight / 2f) + m_mainCamera.rect.yMin;
        float maxY = m_mainCamera.transform.position.z + (camHeight / 2f) - m_mainCamera.rect.yMax;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        return new Vector3(randomX, 0, randomY);
    }

    public void DisplayEnemyInfo()
    {
        for (int i = 0; i < m_enemiesControlled.Count; i++)
        {
            Enemy enemy = m_enemiesControlled[i];

            if (enemy.m_IsEnemyActive && !enemy.IsInfoDisplayed)
            {
                DisplayEnemyInfoText(enemy);
            }

            if (enemy.m_IsEnemyActive)
            {
                enemy.ActivateName();
            }
        }
    }

    void DisplayEnemyInfoText(Enemy enemy)
    {
        Destructable enemyInfoDestruct = m_objectPool.SpawnFromPool(
            PoolTag.EnemyInfo,
            Vector3.zero,
            Quaternion.identity
        );
        EnemyInfoText enemyInfoText = enemyInfoDestruct.GetComponent<EnemyInfoText>();
        enemyInfoText.transform.localScale = m_enemyInfoScale;

        if (enemyInfoText != null)
        {
            enemyInfoText.m_EnemyName.text = enemy.EnemyInfo.m_EnemyName;
            enemyInfoText.m_IronAmount.text = enemy.EnemyInfo.m_IronAmount.ToString();
            enemyInfoText.m_WoodAmount.text = enemy.EnemyInfo.m_WoodAmount.ToString();
            enemyInfoText.m_RumAmount.text = enemy.EnemyInfo.m_RumAmount.ToString();
        }

        enemy.m_EnemyInfoText = enemyInfoText;
        enemy.IsInfoDisplayed = true;
    }

    public void DeactivateEnemyInfo()
    {
        for (int i = 0; i < m_enemiesControlled.Count; i++)
        {
            Enemy enemy = m_enemiesControlled[i];

            if (enemy.m_IsEnemyActive)
            {
                enemy.DeactivateName();
            }
        }
    }
}
