using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOpen : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    Animator m_animator = null;

    bool m_isOpen = false;

    public void ToggleMenu()
    {
        if (m_isOpen)
        {
            // Close the menu
            m_animator.ResetTrigger("Open");
            m_animator.CrossFade("Close", 0.1f);
            m_isOpen = false;
        }
        else
        {
            m_animator.ResetTrigger("Close");
            m_animator.CrossFade("Open", 0.1f);
            m_isOpen = true;
        }
    }
}
