using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LowHealthEffect : MonoBehaviour
{
    public Volume volume;
    public Image image;
    public PlayerHealth playerHealth;

    [Header("Low Health Settings")]
    [SerializeField] float maxVignetteIntensity = 0.8f;
    [SerializeField] float maxImageAlpha = 0.5f;

    private Vignette vignette;

    void Awake()
    {
        if (!volume.profile.TryGet(out vignette))
        {
            Debug.LogError("Volume profile has no Vignette override; low health vignette is disabled.", this);
        }
    }

    void OnEnable()
    {
        playerHealth.HealthFractionChanged += ApplyHealthFraction;
    }

    void OnDisable()
    {
        playerHealth.HealthFractionChanged -= ApplyHealthFraction;
    }

    void Start()
    {
        ApplyHealthFraction(playerHealth.HealthFraction);
    }

    private void ApplyHealthFraction(float healthFraction)
    {
        float missingHealthFraction = 1f - healthFraction;

        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(0f, maxVignetteIntensity, missingHealthFraction);
        }

        Color imageColor = image.color;
        imageColor.a = Mathf.Lerp(0f, maxImageAlpha, missingHealthFraction);
        image.color = imageColor;
    }
}
