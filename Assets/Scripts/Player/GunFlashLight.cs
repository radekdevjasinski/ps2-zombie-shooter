using System.Collections;
using UnityEngine;

public class GunFlashLight : MonoBehaviour
{
    public Light muzzleFlashLight;
    public float flashDuration = 0.05f;
    public float flashIntensity = 10f;

    private WaitForSeconds flashWait;

    void Awake()
    {
        flashWait = new WaitForSeconds(flashDuration);
        muzzleFlashLight.enabled = false;
    }

    public void TriggerFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        muzzleFlashLight.intensity = flashIntensity;
        muzzleFlashLight.enabled = true;

        yield return flashWait;

        muzzleFlashLight.enabled = false;
    }
}
