using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TextMeshProUGUI m_ButtonText;
    [SerializeField] Color m_NormalTextColor;
    [SerializeField] Color m_PressedTextColor;
    [SerializeField] Color m_SelectedTextColor;

    private bool m_Selected;
    private bool m_Pressed;

    private GameObject currentSelected => EventSystem.current.currentSelectedGameObject;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_ButtonText.color = m_PressedTextColor;
        m_Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_ButtonText.color = currentSelected == gameObject ? m_SelectedTextColor : m_NormalTextColor;
        m_Pressed = false;
    }

    void Update()
    {
        if (m_Pressed)
            return;

        if (currentSelected == gameObject && !m_Selected)
        {
            m_ButtonText.color = m_SelectedTextColor;
            m_Selected = true;
        }
        else if (currentSelected != gameObject && m_Selected)
        {
            m_ButtonText.color = m_NormalTextColor;
            m_Selected = false;
        }
    }

}
