using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueListener : MonoBehaviour
{
    [SerializeField] Slider m_Slider;
    [SerializeField] TextMeshProUGUI m_Text;

    // Start is called before the first frame update
    void Start()
    {
        m_Slider.onValueChanged.AddListener(OnValueChanged);
    }


    void OnDestroy()
    {
        m_Slider.onValueChanged.RemoveListener(OnValueChanged);
    }


    void OnValueChanged(float value)
    {
        m_Text.text = ((int)value).ToString();
    }
}
