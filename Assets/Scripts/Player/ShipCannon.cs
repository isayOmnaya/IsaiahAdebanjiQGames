using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCannon : MonoBehaviour
{
    [Header("Aim Indicator Settings")]
    [SerializeField]
    LineRenderer m_aimIndicator;

    [SerializeField]
    float m_maxAimDistance = 10f;

    [SerializeField]
    float m_minAimDistance = 1f;

    [SerializeField]
    float m_maxAllowedAimAngle = 140;

    [Header("Cannon Settings")]
    [SerializeField]
    Transform[] m_leftCannons;

    [SerializeField]
    Transform[] m_rightCannons;

    [SerializeField]
    float m_fireRate = 1.0f;

    [SerializeField]
    float m_projectileSpeed = 20.0f;

    [Header("Rum Explosion")]
    [SerializeField]
    Transform m_leftRumTransform;

    [SerializeField]
    Transform m_rightRumTransform;

    [SerializeField]
    float m_rumFireRate = 1.0f;

    [SerializeField]
    float m_rumThrowSpeed = 20.0f;

    [Header("Detection Settings")]
    [SerializeField]
    LayerMask m_enemyLayerMask;

    [SerializeField]
    Gradient m_detectColor;

    [SerializeField]
    Gradient m_unDetectedColor;

    [Header("ShipShake Settings")]
    [SerializeField]
    float m_shakeDuration = 0.0f;

    [SerializeField]
    float m_shakeMagnitude = 0.0f;

    [Header("Inventory")]
    [SerializeField]
    PlayerInventory m_playerInventory;

    Camera m_mainCamera;
    Vector3 m_initialScale;
    bool m_isAiming = false;
    ObjectPool m_objectPool = null;
    SoundManager m_soundManager = null;

    float m_nextFireTime = 0.0f;
    float m_aimDistance = 0;

    Vector3 dir;

    void Start()
    {
        m_mainCamera = Camera.main;
        m_initialScale = m_aimIndicator.transform.localScale;
        m_aimIndicator.enabled = false;
        m_objectPool = ServiceLocator.GetObjectPool();
        m_soundManager = ServiceLocator.GetSoundManager();
    }

    void Update()
    {
        HandleAiming();
        if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0) && Time.time > m_nextFireTime)
        {
            FireCannons();
            m_nextFireTime = Time.time + m_fireRate;
        }

        if (Input.GetKeyDown(KeyCode.G) && m_isAiming && Time.time > m_nextFireTime)
        {
            ThrowRum();
            m_nextFireTime = Time.time + m_rumFireRate;
        }
    }

    void HandleAiming()
    {
        if (Input.GetMouseButtonDown(1))
        {
            m_isAiming = true;
            m_aimIndicator.enabled = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            m_isAiming = false;
            m_aimIndicator.enabled = false;
            m_soundManager.StopSound(SoundTag.Aim);
        }

        if (m_isAiming)
        {
            PositionAndScaleAimIndicator();

            if (Input.GetMouseButtonDown(1))
            {
                m_isAiming = true;
                m_aimIndicator.enabled = true;
                m_soundManager.PlaySound(SoundTag.Aim);
            }

            if (Input.GetMouseButtonUp(1))
            {
                m_isAiming = false;
                m_aimIndicator.enabled = false;
            }

            if (m_isAiming)
            {
                PositionAndScaleAimIndicator();

                RaycastHit hit;
                if (
                    Physics.Raycast(
                        transform.position,
                        dir,
                        out hit,
                        m_aimDistance,
                        m_enemyLayerMask
                    )
                )
                {
                    m_aimIndicator.colorGradient = m_detectColor;
                }
                else
                {
                    m_aimIndicator.colorGradient = m_unDetectedColor;
                }
            }
        }
    }

    void PositionAndScaleAimIndicator()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldMousePos = m_mainCamera.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, 0)
        );
        Vector3 aimDirection = worldMousePos - transform.position;

        Vector3 aimDirectionProjected = Vector3.ProjectOnPlane(aimDirection, transform.up);

        // Calculate the angle between ship's forward direction and the aim direction projected on x-z plane
        float angleFromForward = Vector3.SignedAngle(
            transform.forward,
            aimDirectionProjected,
            transform.up
        );

        if (Mathf.Abs(angleFromForward) > m_maxAllowedAimAngle)
        {
            float sign = Mathf.Sign(angleFromForward);
            Quaternion maxAllowedRotation = Quaternion.AngleAxis(
                sign * m_maxAllowedAimAngle,
                transform.up
            );
            aimDirectionProjected = maxAllowedRotation * transform.forward * m_maxAimDistance;
        }

        dir = aimDirectionProjected;

        m_aimDistance = Mathf.Clamp(
            aimDirectionProjected.magnitude,
            m_minAimDistance,
            m_maxAimDistance
        );

        Vector3 aimEndPoint = transform.position + aimDirectionProjected.normalized * m_aimDistance;

        m_aimIndicator.SetPosition(0, transform.position);
        m_aimIndicator.SetPosition(1, aimEndPoint);
    }

    #region CannonSettings

    void FireCannons()
    {
        if (m_isAiming)
        {
            FireFromCannons(
                GetActiveCannons(),
                dir.normalized,
                PoolTag.Bullet,
                SoundTag.bullet,
                m_projectileSpeed
            );
        }
        else
        {
            FireFromCannons(
                m_leftCannons,
                -transform.right,
                PoolTag.Bullet,
                SoundTag.bullet,
                m_projectileSpeed
            );
            FireFromCannons(
                m_rightCannons,
                transform.right,
                PoolTag.Bullet,
                SoundTag.bullet,
                m_projectileSpeed
            );
        }
    }

    Transform[] GetActiveCannons()
    {
        float angle = Vector3.SignedAngle(transform.forward, dir.normalized, Vector3.up);
        return angle > 0 ? m_rightCannons : m_leftCannons;
    }

    void FireFromCannons(
        Transform[] cannons,
        Vector3 direction,
        PoolTag projectileToFire,
        SoundTag projectileSound,
        float projectileSpeed
    )
    {
        RotateCannonsTowardsDirection(cannons, direction);
        FireBulletInDirection(
            cannons,
            direction,
            projectileToFire,
            projectileSound,
            projectileSpeed
        );
    }

    void RotateCannonsTowardsDirection(Transform[] cannons, Vector3 direction)
    {
        foreach (Transform cannon in cannons)
        {
            cannon.rotation = Quaternion.LookRotation(direction, transform.up);
        }
    }

    void FireBulletInDirection(
        Transform[] cannons,
        Vector3 direction,
        PoolTag projectileToFire,
        SoundTag projectileSound,
        float projectileSpeed
    )
    {
        foreach (Transform cannon in cannons)
        {
            FireProjectile(cannon, direction, projectileToFire, projectileSound, projectileSpeed);
        }
    }

    void FireProjectile(
        Transform cannon,
        Vector3 direction,
        PoolTag projectileToFire,
        SoundTag projectileSound,
        float projectileSpeed
    )
    {
        if (projectileToFire == PoolTag.Rum)
        {
            if (!m_playerInventory.RemoveItem(ItemType.Rum, 1))
            {
                return;
            }
        }
        StartCoroutine(ShakeShip(m_shakeDuration, m_shakeMagnitude));

        m_soundManager.PlaySound(projectileSound);
        Destructable muzzle = m_objectPool.SpawnFromPool(
            PoolTag.Muzzle,
            cannon.position,
            cannon.rotation
        );
        Destructable bullet = m_objectPool.SpawnFromPool(
            projectileToFire,
            cannon.position,
            cannon.rotation
        );

        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        if (playerBullet != null)
        {
            float effectiveAimDistance = m_aimDistance == 0 ? m_maxAimDistance : m_aimDistance;
            playerBullet.DeactivateBulletAfterDistance(effectiveAimDistance, transform.position);
        }

        Rigidbody rb = playerBullet.GetRigidbody();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        //disable the muzzle
        muzzle.DestroyObject();
    }

    #endregion



    #region RumThrowingAndExplosion
    
    void ThrowRum()
    {
        if (m_isAiming)
        {
            Vector3 throwDirection = dir.normalized;
            float angle = Vector3.SignedAngle(transform.forward, throwDirection, Vector3.up);
            bool useRightCannons = angle > 0;

            Transform activeCannons = useRightCannons ? m_rightRumTransform : m_leftRumTransform;

            FireProjectile(
                activeCannons,
                throwDirection,
                PoolTag.Rum,
                SoundTag.RumThrow,
                m_rumThrowSpeed
            );
        }
    }

    #endregion


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
}
