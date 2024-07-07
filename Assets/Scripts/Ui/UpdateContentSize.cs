using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateContentSize : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    VerticalLayoutGroup m_layoutGroup;

    [SerializeField]
    RectTransform m_contentRectTransform;

    [SerializeField]
    float m_spacingBetweenInfo = 10f;

    void Start()
    {
        AdjustContentSize();
    }

    public void AdjustContentSize()
    {
        float totalHeight = 0f;

        for (int i = 0; i < m_layoutGroup.transform.childCount; i++)
        {
            RectTransform childRectTransform = m_layoutGroup.transform.GetChild(i) as RectTransform;
            totalHeight += childRectTransform.rect.height;

            if (i > 0)
            {
                totalHeight += m_spacingBetweenInfo;
            }
        }
        m_contentRectTransform.sizeDelta = new Vector2(
            m_contentRectTransform.sizeDelta.x,
            totalHeight
        );
    }
}
