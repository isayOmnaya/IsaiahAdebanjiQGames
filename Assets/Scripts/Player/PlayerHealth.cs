using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField]
    float m_maxHealth = 100.0f;
    float m_currentHealth;

    [SerializeField]
    float m_timeToDestroy;

    [Header("UI Elements")]
    [SerializeField]
    Image m_healthBarImage;

    [Header("ShipShake Settings")]
    [SerializeField]
    float m_shakeDuration = 0.0f;

    [SerializeField]
    float m_shakeMagnitude = 0.0f;

    [SerializeField]
    Transform[] m_explosionPoints;

    ObjectPool m_objectPool = null;

    private void Start()
    {
        m_currentHealth = m_maxHealth;
        UpdateHealthBar();
        m_objectPool = ServiceLocator.GetObjectPool();
    }

    private void Update()
    {
        UpdateHealthBar();

        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10.0f);
        }
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth -= damage;
        if (m_currentHealth < 0)
        {
            m_currentHealth = 0;
            GameEvents.GameLost();
            StartCoroutine(EnemyDeath());
        }
        UpdateHealthBar();
    }

    public void Heal(float healAmount)
    {
        m_currentHealth += healAmount;
        if (m_currentHealth > m_maxHealth)
        {
            m_currentHealth = m_maxHealth;
        }
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        m_healthBarImage.fillAmount = m_currentHealth / m_maxHealth;
    }

    public float GetCurrentHealth()
    {
        return m_currentHealth;
    }

    public float GetMaxHealth()
    {
        return m_maxHealth;
    }

    public void SetCurrentHealth(float health)
    {
        m_currentHealth = health;
        UpdateHealthBar();
    }

    IEnumerator ShakeShip(float duration, float magnitude)
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

    IEnumerator EnemyDeath()
    {
        ShakeShip();
        for (int i = 0; i < m_explosionPoints.Length; i++)
        {
            Destructable explosion = m_objectPool.SpawnFromPool(
                PoolTag.Explosion,
                m_explosionPoints[i].position,
                Quaternion.identity
            );
            explosion.DestroyObject();
            yield return new WaitForSeconds(0.5f); // wait for half a second between explosions
        }

        yield return new WaitForSeconds(m_timeToDestroy);
        Destructable wood = m_objectPool.SpawnFromPool(
            PoolTag.WoodHull,
            transform.position,
            transform.rotation
        );

        wood.DestroyObject();

        //endGame
        GameEvents.GameLost();
        gameObject.SetActive(false);

    }

    public void ShakeShip()
    {
        StartCoroutine(ShakeShip(m_shakeDuration, m_shakeMagnitude));
    }
}
