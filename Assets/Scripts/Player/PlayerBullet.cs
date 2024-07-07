using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Bullet
/// </summary>
public class PlayerBullet : MonoBehaviour, IDestruct
{
    [Header("Component")]
    [SerializeField]
    Rigidbody m_rigidBody = null;

    [SerializeField]
    Destructable m_destructable = null;
	

    [Header("Parameter")]
    [SerializeField]
    string m_enemyTag;

    [SerializeField]
    float m_bulletDamage = 0;

    [SerializeField]
    bool m_canRotate = false;

    [SerializeField]
    float m_rotateSpeed = 20;

    ObjectPool m_objectPool = null;
    SoundManager m_soundManager = null;

    bool m_hitEnemy = false;

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (m_canRotate)
        {
            transform.Rotate(m_rotateSpeed * Time.deltaTime, 0, 0);
        }
    }

    public void DeactivateBulletAfterDistance(float distance, Vector3 shipPosition)
    {
        StartCoroutine(DeactivateBulletCoroutine(distance, shipPosition));
    }

    IEnumerator DeactivateBulletCoroutine(float distance, Vector3 shipPosition)
    {
        while (Mathf.Abs(transform.position.x - shipPosition.x) <= Mathf.Abs(distance))
        {
            yield return null;
        }
        DestroyObject();
    }

    public void OnEnable()
    {
        if (m_objectPool == null)
        {
            m_objectPool = ServiceLocator.GetObjectPool();
            m_soundManager = ServiceLocator.GetSoundManager();
        }
    }

    public void DestroyObject()
    {
        if (!m_hitEnemy)
        {
            m_soundManager.PlaySound(SoundTag.Explosion);
            Destructable explode = m_objectPool.SpawnFromPool(
                PoolTag.Explosion,
                transform.position,
                Quaternion.identity
            );
            explode.DestroyObject();
        }
        m_objectPool.ReturnToPool(m_destructable.GetDestructTag(), m_destructable);
    }

    public Rigidbody GetRigidbody()
    {
        return m_rigidBody;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == m_enemyTag)
        {
            m_hitEnemy = true;
            m_soundManager.PlaySound(SoundTag.Explosion);
            //explode
            Destructable explode = m_objectPool.SpawnFromPool(
                PoolTag.Explosion,
                transform.position,
                Quaternion.identity
            );

            //spawn woodenpart
            Destructable wood = m_objectPool.SpawnFromPool(
                PoolTag.Wood,
                transform.position,
                Quaternion.identity
            );

            //shake the ship
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.TakeDamage(m_bulletDamage);
            enemy.ShakeShip();

            //destroy bullet
            explode.DestroyObject();
            wood.DestroyObject();

            m_hitEnemy = false;
            DestroyObject();
        }
    }
}
