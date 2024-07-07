using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum EnemyState
{
    Idle = 0,
    Moving,
    Attack,
    Wait,
    Death
}

public class Enemy : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField]
    float m_enemyMaxHealth = 0;
    float m_enemyHealth = 5;

    [SerializeField]
    float m_timeToDestroy;

    [SerializeField]
    float m_timeBetweenExplosions = 0.5f;

    [SerializeField]
    TMP_Text m_enemyNameText = null;

    [SerializeField]
    Slider m_slider;


    [Header("Enemy Ai Settings")]
    [SerializeField]
    EnemyState m_currentState;

    [SerializeField]
    Transform[] m_firePoints;

    [SerializeField]
    float m_attackTime = 2f;

    [SerializeField]
    float m_numberOfProjectileRounds = 2f;

    [SerializeField]
    float m_minWaitTime = 2f;

    [SerializeField]
    float m_maxWaitTime = 6f;
    

    [Header("ShipShake Settings")]
    [SerializeField]
    float m_shakeDuration = 0.0f;

    [SerializeField]
    float m_shakeMagnitude = 0.0f;

    [SerializeField]
    Transform[] m_explosionPoints;

    NavMeshAgent m_agent;
    Rigidbody m_rigidbody;
    float m_waitTimer = 0f;
    float m_currentWaitTime;
    bool m_isAttacking = false;

    [HideInInspector]
    public Vector3 m_TargetWaypoint;

    [HideInInspector]
    public bool m_IsEnemyActive = false;
    ObjectPool m_objectPool = null;
    Destructable m_destructable = null;
    SoundManager m_soundManager = null;

    public EnemyInfo EnemyInfo { get; set; }
    public EnemyInfoText EnemyInfoText { get; set; }
    public bool IsInfoDisplayed { get; set; }
    public Vector3 PlayerLastPosition { get; set; }

    bool m_isDead = false;

    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.avoidancePriority = Random.Range(30, 70);
        m_currentState = EnemyState.Idle;
        m_currentWaitTime = Random.Range(m_minWaitTime, m_maxWaitTime);
        m_objectPool = ServiceLocator.GetObjectPool();
        m_destructable = GetComponent<Destructable>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_soundManager = ServiceLocator.GetSoundManager();
        m_enemyHealth = m_enemyMaxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        switch (m_currentState)
        {
            case EnemyState.Idle:
                UpdateIdleState();
                break;
            case EnemyState.Moving:
                UpdateMovingState();
                break;
            case EnemyState.Attack:
                UpdateAttackState();
                break;
            case EnemyState.Wait:
                UpdateWaitState();
                break;
            case EnemyState.Death:
                UpdateWaitState();
                break;
        }
        m_TargetWaypoint.y = transform.position.y;
        UpdateHealthBar();
        UpdateDeathState();
    }

    #region EnemyState

    void UpdateIdleState()
    {
        if (!m_IsEnemyActive)
            return;
        if (m_TargetWaypoint != Vector3.zero)
        {
            m_currentState = EnemyState.Moving;
        }
    }

    void UpdateMovingState()
    {
        if (!m_IsEnemyActive)
            return;
        if (m_agent.enabled)
        {
            m_agent.SetDestination(m_TargetWaypoint);
            if (Vector3.Distance(transform.position, m_TargetWaypoint) < m_agent.stoppingDistance)
            {
                m_currentState = EnemyState.Attack;
            }
        }
    }

    void UpdateAttackState()
    {
        if (!m_isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    void UpdateWaitState()
    {
        m_waitTimer -= Time.deltaTime;
        if (m_waitTimer <= 0f)
        {
            m_currentState = EnemyState.Idle;
        }
    }

    public EnemyState CurrentState
    {
        get { return m_currentState; }
        set { m_currentState = value; }
    }

    void UpdateDeathState()
    {
        if (m_enemyHealth <= 0)
        {
            m_currentState = EnemyState.Death;
        }
    }

    IEnumerator AttackCoroutine()
    {
        m_isAttacking = true;

        for (int i = 0; i < m_numberOfProjectileRounds; i++)
        {
            Fire();
            yield return new WaitForSeconds(m_attackTime);
        }

        m_waitTimer = m_currentWaitTime;
        m_currentState = EnemyState.Wait;
        m_isAttacking = false;
    }

    void Fire()
    {
        for (int i = 0; i < m_firePoints.Length; i++)
        {
            m_soundManager.PlaySound(SoundTag.bullet);
            Destructable muzzle = m_objectPool.SpawnFromPool(
                PoolTag.Muzzle,
                m_firePoints[i].transform.position,
                Quaternion.identity
            );
            Destructable aimBomb = m_objectPool.SpawnFromPool(
                PoolTag.EnemyBomb,
                m_firePoints[i].transform.position,
                Quaternion.identity
            );

            EnemyBullet aimCannon = aimBomb.GetComponent<EnemyBullet>();

            if (aimCannon != null)
                aimCannon.SetTargetPosition(PlayerLastPosition);

            muzzle.DestroyObject();
        }
    }

    #endregion


    #region EnemyHealth

    public IEnumerator ShakeShip(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = originalPosition.x + Random.Range(-magnitude, magnitude);
            float z = originalPosition.z + Random.Range(-magnitude, magnitude);

            transform.position = new Vector3(x, originalPosition.y, z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
    }

    public void ShakeShip()
    {
        StartCoroutine(ShakeShip(m_shakeDuration, m_shakeMagnitude));
    }

    public void TakeDamage(float damage)
    {
        m_enemyHealth -= damage;
        m_currentState = EnemyState.Wait;

        if (m_enemyHealth <= 0 && !m_isDead)
        {
            SpawnLootItem();
            StartCoroutine(EnemyDeath());
            m_isDead = true;
        }
    }

    void UpdateHealthBar()
    {
        m_slider.value = m_enemyHealth / m_enemyMaxHealth;
    }

    IEnumerator EnemyDeath()
    {
        // Disable the enemy
        m_IsEnemyActive = false;
        m_agent.enabled = false;

        for (int i = 0; i < m_explosionPoints.Length; i++)
        {
            Destructable explosion = m_objectPool.SpawnFromPool(
                PoolTag.Explosion,
                m_explosionPoints[i].position,
                Quaternion.identity
            );
            explosion.DestroyObject();
            yield return new WaitForSeconds(m_timeBetweenExplosions); // wait for half a second between explosions
        }

        yield return new WaitForSeconds(m_timeToDestroy);
        Destructable wood = m_objectPool.SpawnFromPool(
            PoolTag.WoodHull,
            transform.position,
            transform.rotation
        );

        wood.DestroyObject();
        m_destructable.DestroyObject();
    }

    void SpawnLootItem()
    {
        Destructable loot = m_objectPool.SpawnFromPool(
            PoolTag.LootItem,
            transform.position,
            transform.rotation
        );
        CargoLoot cargoLoot = loot.GetComponent<CargoLoot>();
        if (cargoLoot != null)
        {
            cargoLoot.SetLoot(
                EnemyInfo.m_RumAmount,
                EnemyInfo.m_IronAmount,
                EnemyInfo.m_WoodAmount
            );
        }
    }

    public void SetEnemyInfo(EnemyInfo enemyInfo, int health, string name)
    {
        EnemyInfo = enemyInfo;
        m_enemyMaxHealth = health;
        m_enemyNameText.text = name;
    }

    public void DeactivateName()
    {
        m_enemyNameText.enabled = false;
    }

    public void ActivateName()
    {
        m_enemyNameText.enabled = true;
    }

    public NavMeshAgent GetAgent()
    {
        return m_agent;
    }

    IEnumerator ReenableNavAgent()
    {
        yield return new WaitForSeconds(1.0f);
        m_agent.enabled = true;
    }

    public void ActivateNavMeshAgent()
    {
        StartCoroutine(ReenableNavAgent());
    }

    void OnDisable()
    {
        m_isDead = false;
    }

    #endregion
}
