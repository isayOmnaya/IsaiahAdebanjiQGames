using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPreloader : MonoBehaviour
{
    [SerializeField]
    Camera m_mainCam = null;

    [SerializeField]
    Camera m_eagleVisionCam = null;

    [SerializeField]
    float m_preSwitchDuration = .5f;

    void Awake() 
    {
        StartCoroutine(PreloadCameras());
    }

    IEnumerator PreloadCameras()
    {
        m_mainCam.enabled = true;
        m_eagleVisionCam.enabled = false;

        //enable the eaglecam to preload resources briefly
        m_eagleVisionCam.enabled = true;
        m_mainCam.enabled = false;

        yield return new WaitForSeconds(m_preSwitchDuration);

        m_mainCam.enabled = true;
        m_eagleVisionCam.enabled = false;
    }

}
