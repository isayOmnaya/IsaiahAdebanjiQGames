using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour, IDestruct
{
    [Header("Parameter")]
    [SerializeField]
    float m_destructTime = 0.0f;

    [SerializeField]
    PoolTag m_destructTag;

    public bool m_IsUIDestruct = false;
    

    [Header("Destructible Ship Parts")]
    [SerializeField]
    bool m_isWoodenShipPart = false;

    [SerializeField]
    Vector3 m_defaultScale;

    Transform[] m_woodenChildrenTransforms = null;
    Vector3 m_defaultParentScale;
    ObjectPool m_objectPool = null;

    void Start()
    {
        if (m_isWoodenShipPart)
        {
            m_woodenChildrenTransforms = GetComponentsInChildren<Transform>();
            m_defaultParentScale = transform.localScale;
        }
    }

    public void ResetChildrenScale()
    {
        if (m_woodenChildrenTransforms != null)
        {
            for (int i = 0; i < m_woodenChildrenTransforms.Length; i++)
            {
                m_woodenChildrenTransforms[i].localScale = m_defaultScale;
                m_woodenChildrenTransforms[i].position = Vector3.zero;
                m_woodenChildrenTransforms[i].localRotation = Quaternion.identity;
            }
            transform.localScale = m_defaultParentScale;
            transform.position = Vector3.zero;
        }
    }

    public void OnEnable()
    {
        if (m_objectPool == null)
        {
            m_objectPool = ServiceLocator.GetObjectPool();
        }
    }

    public void DestroyObject()
    {
        StartCoroutine(DeactivateGameObjectCoroutine());
    }

    IEnumerator DeactivateGameObjectCoroutine()
    {
        yield return new WaitForSeconds(m_destructTime);
        if (m_isWoodenShipPart)
        {
            ResetChildrenScale();
        }
        m_objectPool.ReturnToPool(m_destructTag, this);
    }

    public PoolTag GetDestructTag()
    {
        return m_destructTag;
    }
}
