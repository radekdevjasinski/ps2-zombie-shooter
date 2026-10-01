using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HPSlider : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
    }

    void OnEnable()
    {
        playerHealth.HealthFractionChanged += ShowHealthFraction;
    }

    void OnDisable()
    {
        playerHealth.HealthFractionChanged -= ShowHealthFraction;
    }

    void Start()
    {
        ShowHealthFraction(playerHealth.HealthFraction);
    }

    private void ShowHealthFraction(float healthFraction)
    {
        slider.value = healthFraction;
    }
}
