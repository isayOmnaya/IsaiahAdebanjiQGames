using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Character
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Sail/Mast")]
    [SerializeField]
    GameObject m_sailMastFrontObj;

    [SerializeField]
    GameObject m_sailMastRearObj;


    [Header("Ship Movement")]
    [SerializeField]
    float m_minSpeed = 2.0f;

    [SerializeField]
    float m_maxSpeed = 10.0f;

    [SerializeField]
    float m_acceleration = 2.0f;

    [SerializeField]
    float m_deceleration = 2.0f;

    [SerializeField]
    float m_reverseSpeed = 5.0f;

    [SerializeField]
    float m_rotateSpeed = 50.0f;


    [Header("Sail Parameters")]
    [SerializeField]
    float m_maxSailHeightt = 1.0f;

    [SerializeField]
    float m_minSailHeight = 0.0f;

    [SerializeField]
    float m_sailMovementSpeed = 0.5f;

    float m_currentSpeed = 0.0f;
    bool m_areSailsUp = false;


    [Header("Components")]
    [SerializeField]
    EnemyManager m_enemyManager = null;

    PlayerHealth m_playerHealth = null;
    PlayerInventory m_playerInventory = null;
    Rigidbody m_rigidbody = null;


    [Header("EagleVision")]
    [SerializeField]
    Camera m_defaultCamera = null;

    [SerializeField]
    Camera m_eagleVisionCamera = null;

    [SerializeField]
    Animator m_eagleCameraFadeAnim;

    [SerializeField]
    GameObject m_eagleVisionIndicator = null;

    [SerializeField]
    float m_timeToFade = 0.1f;
    bool m_isEagleVisionActive = false;


    [Header("Inventory")]
    [SerializeField]
    GameObject m_inventoryPanel = null;

    [SerializeField]
    float m_slowDownTime = 0.5f;


    [Header("Collision Settings")]
    [SerializeField]
    float m_forceMagnitude = 1000;

    [SerializeField]
    float m_speedThreshold = .5f;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_playerHealth = GetComponent<PlayerHealth>();
        m_playerInventory = GetComponent<PlayerInventory>();
        InitializeVisionSettings();
        m_inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (m_playerHealth.GetCurrentHealth() <= 0)
        {
            return;
        }
        HandleMovement();
        HandleSails();
        ToggleVision();
        ToggleInventory();
    }


    #region ShipControl

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (m_currentSpeed == 0.0f)
            {
                m_currentSpeed = m_minSpeed;
            }
            else if (m_currentSpeed > 0.0f)
            {
                m_currentSpeed = Mathf.MoveTowards(
                    m_currentSpeed,
                    m_maxSpeed,
                    m_acceleration * Time.deltaTime
                );
            }
            else if (m_currentSpeed < 0.0f) // Moving reverse, decelerate to switch direction
            {
                m_currentSpeed = Mathf.MoveTowards(
                    m_currentSpeed,
                    0.0f,
                    m_deceleration * Time.deltaTime
                );
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (m_currentSpeed == 0.0f)
            {
                m_currentSpeed = -m_reverseSpeed;
            }
            else if (m_currentSpeed > 0.0f)
            {
                m_currentSpeed = Mathf.MoveTowards(
                    m_currentSpeed,
                    0.0f,
                    m_deceleration * Time.deltaTime
                );
            }
            else if (m_currentSpeed < 0.0f)
            {
                m_currentSpeed = Mathf.MoveTowards(
                    m_currentSpeed,
                    -m_reverseSpeed,
                    m_acceleration * Time.deltaTime
                );
            }
        }
        else
        {
            m_currentSpeed = Mathf.MoveTowards(
                m_currentSpeed,
                0.0f,
                m_deceleration * Time.deltaTime
            );
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
        }

        Vector3 forwardMovement = transform.forward * m_currentSpeed * Time.deltaTime;
        m_rigidbody.MovePosition(m_rigidbody.position + forwardMovement);

        if (Input.GetKey(KeyCode.A))
        {
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * -m_rotateSpeed * Time.deltaTime);
            m_rigidbody.MoveRotation(m_rigidbody.rotation * deltaRotation);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * m_rotateSpeed * Time.deltaTime);
            m_rigidbody.MoveRotation(m_rigidbody.rotation * deltaRotation);
        }
    }

    void HandleSails()
    {
        float targetHeight = m_areSailsUp ? m_maxSailHeightt : m_minSailHeight;

        float currentSailHeightFR = m_sailMastFrontObj.transform.localPosition.y;
        float currentSailHeightFL = m_sailMastRearObj.transform.localPosition.y;

        float newHeight = Mathf.MoveTowards(
            currentSailHeightFR,
            targetHeight,
            m_sailMovementSpeed * Time.deltaTime
        );

        m_sailMastFrontObj.transform.localPosition = new Vector3(
            m_sailMastFrontObj.transform.localPosition.x,
            newHeight,
            m_sailMastFrontObj.transform.localPosition.z
        );
        m_sailMastRearObj.transform.localPosition = new Vector3(
            m_sailMastRearObj.transform.localPosition.x,
            newHeight,
            m_sailMastRearObj.transform.localPosition.z
        );

        if (m_currentSpeed > 0.0f && !m_areSailsUp)
        {
            m_areSailsUp = true;
        }
        else if (m_currentSpeed == 0.0f && m_areSailsUp)
        {
            m_areSailsUp = false;
        }
    }

    #endregion


    #region EagleVisionMode

    

    void EagleVision()
    {
        StartCoroutine(SmoothCameraTransition());
    }

    void InitializeVisionSettings()
    {
        m_eagleVisionIndicator.SetActive(false);
        m_isEagleVisionActive = false;
    }

    void ToggleVision()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            m_eagleCameraFadeAnim.SetTrigger("EV");
            EagleVision();
        }
    }

    IEnumerator SmoothCameraTransition()
    {
        yield return new WaitForSeconds(m_timeToFade);

        m_isEagleVisionActive = !m_isEagleVisionActive;

        m_defaultCamera.enabled = !m_isEagleVisionActive;
        m_eagleVisionCamera.enabled = m_isEagleVisionActive;

        m_eagleVisionIndicator.SetActive(m_isEagleVisionActive);

        if (m_isEagleVisionActive)
        {
            ScanEnvironment();
        }
    }

    void ScanEnvironment()
    {
        m_enemyManager.DisplayEnemyInfo();
    }

    #endregion


    #region Inventory

    void ToggleInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            m_inventoryPanel.SetActive(true);
            Time.timeScale = m_slowDownTime;
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            m_inventoryPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    #endregion


    public Rigidbody GetRigidbody()
    {
        return m_rigidbody;
    }

    public float GetSpeed
    {
        get { return m_currentSpeed; }
        set { m_currentSpeed = value; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();

            Vector3 forceDirection = collision.contacts[0].normal;

            if (enemyRb.velocity.magnitude > m_speedThreshold || 
                m_rigidbody.velocity.magnitude > m_speedThreshold)
            {
                //ServiceLocator.GetSoundManager().PlaySound(SoundTag.ShipCollision);
                m_rigidbody.AddForce(-forceDirection * m_forceMagnitude);
                enemyRb.AddForce(forceDirection * m_forceMagnitude);
            }
        }

        if (collision.gameObject.CompareTag("Loot"))
        {
            CargoLoot cargoLoot = collision.gameObject.GetComponent<CargoLoot>();

            ServiceLocator.GetSoundManager().PlaySound(SoundTag.PickUp);
            m_playerInventory.AddItem(ItemType.Wood, cargoLoot.GetWoodAmount());
            m_playerInventory.AddItem(ItemType.Iron, cargoLoot.GetIronAmount());
            m_playerInventory.AddItem(ItemType.Rum, cargoLoot.GetRumAmount());

            Destructable cargoDestruct = cargoLoot.GetDestructable();
            cargoDestruct.DestroyObject();
        }
    }
}
