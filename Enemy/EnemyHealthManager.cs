using System.Net.Mail;
using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateHealthBar(float currentValue, float maxValue) {
        slider.gameObject.SetActive(currentValue < maxValue);
        slider.value = currentValue / maxValue;
        Image fillImage = slider.fillRect.GetComponent<Image>();
        fillImage.color = Color.Lerp(Color.red, Color.green, slider.normalizedValue);

        if (slider.value == 0)
            slider.gameObject.SetActive(false);
    }
}
