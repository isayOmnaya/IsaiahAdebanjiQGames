using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField]
    List<Pool> m_pools;

    [SerializeField]
    Transform m_uiTransformParent = null;

    Dictionary<PoolTag, Queue<Destructable>> m_poolDictionary;

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        m_poolDictionary = new Dictionary<PoolTag, Queue<Destructable>>();

        for (int poolIndex = 0; poolIndex < m_pools.Count; poolIndex++)
        {
            Pool pool = m_pools[poolIndex];
            Queue<Destructable> objectPool = new Queue<Destructable>();

            for (int i = 0; i < pool.m_Size; i++)
            {
                Destructable obj = Instantiate(pool.m_Prefab).GetComponent<Destructable>();
                obj.gameObject.SetActive(false);
                objectPool.Enqueue(obj);

                // if its a ui parents it under a canvas
                obj.transform.SetParent(obj.m_IsUIDestruct ? m_uiTransformParent : transform);
            }

            m_poolDictionary.Add(pool.m_Tag, objectPool);
        }

        ServiceLocator.RegisterObjectPool(this);
    }

    public Destructable SpawnFromPool(PoolTag tag, Vector3 position, Quaternion rotation)
    {
        if (!m_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Queue<Destructable> objectPool = m_poolDictionary[tag];
        Destructable objectToSpawn = null;

        // Check if there are inactive objects available
        if (objectPool.Count > 0)
        {
            objectToSpawn = objectPool.Dequeue();
        }
        else
        {
            // If no inactive objects are available, instantiate a new one
            Pool pool = m_pools.Find(p => p.m_Tag == tag);
            if (pool != null)
            {
                objectToSpawn = Instantiate(pool.m_Prefab);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                objectToSpawn.transform.SetParent(transform);
            }
        }

        // Activate and position the spawned object
        if (objectToSpawn != null)
        {
            objectToSpawn.gameObject.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
        }

        return objectToSpawn;
    }

    public void ReturnToPool(PoolTag tag, Destructable objectToReturn)
    {
        if (!m_poolDictionary.ContainsKey(tag))
        {
            return;
        }

        objectToReturn.gameObject.SetActive(false);
        m_poolDictionary[tag].Enqueue(objectToReturn);
    }
}


[System.Serializable]
public class Pool
{
    public PoolTag m_Tag;
    public Destructable m_Prefab;
    public int m_Size;
}

public enum PoolTag
{
    Enemy = 0,
    Bullet,
    Muzzle,
    Explosion,
    EnemyInfo,
    WoodHull,
    Wood,
    EnemySpawner,
    EnemyBomb,
    Rum,
    LootItem,
}
