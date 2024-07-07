using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Cannon Settings")]
    [SerializeField]
    float m_moveSpeed = 5f;

    [SerializeField]
    float m_explosionDamage = 10f;
    Vector3 m_targetPosition;
    

    [Header("Component")]
    [SerializeField]
    Destructable m_destructable = null;

    [SerializeField]
    Rigidbody m_rigidBody;

    ObjectPool m_objectPool;
    SoundManager m_soundManager = null;

    void Start()
    {
        m_objectPool = ServiceLocator.GetObjectPool();
        m_soundManager = ServiceLocator.GetSoundManager();
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        m_targetPosition = targetPosition;
        Vector3 direction = (m_targetPosition - transform.position).normalized;

        m_rigidBody.velocity = direction * m_moveSpeed;
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, m_targetPosition) < 0.1f)
        {
            Explode();
        }
    }

    void Explode()
    {
        m_soundManager.PlaySound(SoundTag.Explosion);
        Destructable explosion = m_objectPool.SpawnFromPool(
            PoolTag.Explosion,
            transform.position,
            Quaternion.identity
        );
        explosion.DestroyObject();

        m_objectPool.ReturnToPool(PoolTag.EnemyBomb, m_destructable);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            player.TakeDamage(m_explosionDamage);

            Explode();

            Destructable wood = m_objectPool.SpawnFromPool(
                PoolTag.Wood,
                transform.position,
                Quaternion.identity
            );
            player.ShakeShip();

            wood.DestroyObject();
        }
    }
}
