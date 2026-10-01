using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SensSlider : MonoBehaviour
{
    private FPSCamera fpsCamera;
    private Slider slider;

    void Start()
    {
        fpsCamera = Camera.main.GetComponent<FPSCamera>();
        slider = GetComponent<Slider>();
        slider.value = fpsCamera.sens;
    }

    public void OnSensChanged()
    {
        fpsCamera.SetSensitivity(slider.value);
    }
}
